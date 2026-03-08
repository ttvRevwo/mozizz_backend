using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MozizzAPI.Models;
using MozizzAPI.Services;
using Mysqlx.Crud;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly MozizzContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public ReservationController(MozizzContext context, IConfiguration configuration, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [Authorize]
        [HttpGet("MyReservations/{userId}")]
        public async Task<IActionResult> GetMyHistory(int userId)
        {
           
            var history = await _context.Reservations
                .Include(r => r.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.Reservedseats)
                    .ThenInclude(rs => rs.Seat)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReservationDate) 
                .Select(r => new {
                    r.ReservationId,
                    FilmCim = r.Showtime.Movie.Title,
                    Datum = r.Showtime.ShowDate.ToShortDateString(),
                    Idopont = r.Showtime.ShowTime1,
                    Statusz = r.Status,
                    Szekek = string.Join(", ", r.Reservedseats.Select(rs => rs.Seat.SeatNumber)),
                    Vegosszeg = r.Reservedseats.Count * 2500,
                    LefoglaltvaEkkor = r.ReservationDate
                })
                .ToListAsync();

            if (!history.Any())
            {
                return NotFound(new { uzenet = "Még nincsenek foglalásaid." });
            }

            return Ok(history);
        }


        [Authorize]
        [HttpDelete("Cancel/{reservationId}")]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Showtime).ThenInclude(s => s.Movie)
                .Include(r => r.User)
                .Include(r => r.Tickets)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null) return NotFound(new { uzenet = "A foglalás nem található." });

            DateTime showDateTime = reservation.Showtime.ShowDate.Date + reservation.Showtime.ShowTime1;
            if (showDateTime < DateTime.Now.AddHours(2))
            {
                return BadRequest(new { uzenet = "Ezt a foglalást már nem tudod lemondani!" });
            }

            string userEmail = reservation.User.Email;
            string movieTitle = reservation.Showtime.Movie.Title;
            string showTime = showDateTime.ToString("yyyy.MM.dd. HH:mm");

            try
            {
                if (reservation.Tickets != null && reservation.Tickets.Any())
                {
                    _context.Tickets.RemoveRange(reservation.Tickets);
                }
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();

                SendCancellationEmail(userEmail, movieTitle, showTime);
                _emailService.SendEmail(
                    reservation.UserId,
                    userEmail,
                    "Cancellation",
                    "Foglalás lemondás visszaigazolása",
                    "A foglalásod lemondás sikeresen rögzítve lett" 
                );

                return Ok(new { uzenet = "Foglalás törölve és visszaigazoló e-mail elküldve!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { hiba = "Hiba történt: " + ex.Message });
            }
        }

        private void SendCancellationEmail(string targetEmail, string movieTitle, string showTime)
        {
            try
            {
                var emailConfig = _configuration.GetSection("EmailSettings");
                string senderEmail = emailConfig["Email"];
                string appPassword = emailConfig["Password"];

                var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, appPassword)
                };

                string body = $@"
            <div style='font-family: Arial; border: 1px solid #eee; padding: 20px; max-width: 600px;'>
                <h2 style='color: #E50914;'>Foglalás lemondva ❌</h2>
                <p>Kedves Vásárlónk!</p>
                <p>Visszaigazoljuk, hogy a <b>{movieTitle}</b> ({showTime}) című filmre szóló foglalásodat sikeresen töröltük.</p>
                <p>A székek felszabadításra kerültek. Reméljük, hamarosan újra nálunk mozizol!</p>
                <hr>
                <p style='font-size: 12px; color: #888;'>Ez egy automatikus üzenet a Mozizz Cinema rendszeréből.</p>
            </div>";

                var message = new MailMessage(senderEmail, targetEmail, "Foglalás lemondás visszaigazolása", body);
                message.IsBodyHtml = true;
                smtp.Send(message);
            }
            catch (Exception ex) { Console.WriteLine("E-mail hiba: " + ex.Message); }
        }

    }
}