using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MozizzAPI.DTOS;
using MozizzAPI.Models;
using MozizzAPI.Services;
using QRCoder;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly MozizzContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public BookingController(MozizzContext context, IConfiguration configuration, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;

        }


        [HttpGet("GetSeatsForShowtime")]
        public IActionResult GetSeatsForShowtime(int showtimeId)
        {
            try
            {

                var showtime = _context.Showtimes.Find(showtimeId);
                if (showtime == null) return NotFound("A vetítés nem található.");


                var allSeats = _context.Seats
                    .Where(s => s.HallId == showtime.HallId)
                    .ToList();


                var reservedSeatIds = _context.Reservedseats
                    .Where(rs => rs.Reservation.ShowtimeId == showtimeId)
                    .Select(rs => rs.SeatId)
                    .ToList();

                var result = allSeats.Select(s => new
                {
                    s.SeatId,
                    s.SeatNumber,
                    s.IsVip,
                    IsReserved = reservedSeatIds.Contains(s.SeatId)
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateBooking")]
        public IActionResult CreateBooking([FromBody] BookingDto dto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var alreadyReserved = _context.Reservedseats
                        .Any(rs => rs.Reservation.ShowtimeId == dto.ShowtimeId && dto.SeatIds.Contains(rs.SeatId));

                    if (alreadyReserved)
                        return BadRequest(new { hiba = "Sajnáljuk, időközben valaki már lefoglalta az egyik választott széket." });

                    var reservation = new Reservation
                    {
                        UserId = dto.UserId,
                        ShowtimeId = dto.ShowtimeId,
                        ReservationDate = DateTime.Now,
                        Status = "pending" 
                    };

                    _context.Reservations.Add(reservation);
                    _context.SaveChanges();

                    foreach (var seatId in dto.SeatIds)
                    {
                        var reservedSeat = new Reservedseat
                        {
                            ReservationId = reservation.ReservationId,
                            SeatId = seatId
                        };
                        _context.Reservedseats.Add(reservedSeat);
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    return Ok(new
                    {
                        message = "Székek ideiglenesen lefoglalva! 15 perced van befejezni a fizetést.",
                        reservationId = reservation.ReservationId
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(new { hiba = $"Hiba a foglalás során: {ex.Message}" });
                }
            }
        }

        [HttpPost("ConfirmBooking/{reservationId}")]
        public IActionResult ConfirmBooking(int reservationId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var reservation = _context.Reservations
                        .Include(r => r.Reservedseats)
                            .ThenInclude(rs => rs.Seat)
                        .Include(r => r.User)
                        .Include(r => r.Showtime)
                            .ThenInclude(s => s.Movie)
                        .FirstOrDefault(r => r.ReservationId == reservationId);

                    if (reservation == null)
                        return NotFound(new { hiba = "A foglalás nem található." });

                    if (reservation.Status == "confirmed")
                        return BadRequest(new { hiba = "Ezt a foglalást már korábban kifizették/véglegesítették." });

                    if (reservation.ReservationDate.AddMinutes(15) < DateTime.Now)
                    {
                        return BadRequest(new { hiba = "A fizetési idő (15 perc) lejárt. Kérjük, kezdd újra a foglalást." });
                    }

                    reservation.Status = "confirmed";

                    string egyediJegyKod = Guid.NewGuid().ToString();
                    var ujJegy = new Ticket
                    {
                        ReservationId = reservation.ReservationId,
                        TicketCode = egyediJegyKod
                    };
                    _context.Tickets.Add(ujJegy);

                    _context.SaveChanges();
                    transaction.Commit();

                    string seatsString = string.Join(", ", reservation.Reservedseats.Select(rs => rs.Seat.SeatNumber));
                    string showDateStr = reservation.Showtime.ShowDate.ToShortDateString() + " " + reservation.Showtime.ShowTime1.ToString();

                    SendTicketEmail(reservation.User.Email, reservation.Showtime.Movie.Title, showDateStr, seatsString, egyediJegyKod);
                    _emailService.SendEmail(
                     reservation.UserId,
                     reservation.User.Email,
                     "Cancellation",
                     "Foglalás lemondás visszaigazolása",
                     "A foglalásod lemondás sikeresen rögzítve lett."
                 );


                    return Ok(new { message = "Sikeres fizetés! A jegyet és a QR kódot elküldtük e-mailben." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(new { hiba = $"Hiba a véglegesítés során: {ex.Message}" });
                }
            }
        }

        [HttpGet("GetUserReservations/{userId}")]
        public IActionResult GetUserReservations(int userId)
        {
            var res = _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.Reservedseats)
                    .ThenInclude(rs => rs.Seat)
                .Select(r => new
                {
                    r.ReservationId,
                    MovieTitle = r.Showtime.Movie.Title,
                    Date = r.Showtime.ShowDate,
                    Time = r.Showtime.ShowTime1,
                    Seats = r.Reservedseats.Select(rs => rs.Seat.SeatNumber).ToList(),
                    r.Status
                })
                .ToList();

            return Ok(res);
        }

        [HttpGet("AvailableSeats/{showtimeId}")]
        public IActionResult GetAvailableSeats(int showtimeId)
        {
            try
            {

                var showtime = _context.Showtimes
                    .Include(s => s.Hall)
                    .FirstOrDefault(s => s.ShowtimeId == showtimeId);

                if (showtime == null)
                    return NotFound(new { hiba = "A megadott vetítés nem található." });


                var allSeatsInHall = _context.Seats
                    .Where(s => s.HallId == showtime.HallId)
                    .Select(s => new { s.SeatId, s.SeatNumber, s.IsVip })
                    .ToList();


                var reservedSeatIds = _context.Reservedseats
                    .Include(rs => rs.Reservation)
                    .Where(rs => rs.Reservation.ShowtimeId == showtimeId && rs.Reservation.Status == "confirmed")
                    .Select(rs => rs.SeatId)
                    .ToList();


                var seatMap = allSeatsInHall.Select(seat => new
                {
                    SeatId = seat.SeatId,
                    SeatNumber = seat.SeatNumber,
                    IsVip = seat.IsVip,
                    IsReserved = reservedSeatIds.Contains(seat.SeatId)
                }).ToList();


                return Ok(new
                {
                    ShowtimeId = showtimeId,
                    HallName = showtime.Hall.Name,
                    TotalCapacity = allSeatsInHall.Count,
                    ReservedCount = reservedSeatIds.Count,
                    FreeCount = allSeatsInHall.Count - reservedSeatIds.Count,
                    Seats = seatMap
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { hiba = "Hiba a székek lekérdezésekor: " + ex.Message });
            }
        }

        private void SendTicketEmail(string targetEmail, string movieTitle, string showTime, string seats, string ticketCode)
        {
            try
            {
                var emailConfig = _configuration.GetSection("EmailSettings");
                string senderEmail = emailConfig["Email"];
                string appPassword = emailConfig["Password"];

                var fromAddress = new MailAddress(senderEmail, "Mozizz Cinema");
                var toAddress = new MailAddress(targetEmail);

                string qrCodeUrl = $"https://quickchart.io/qr?text={ticketCode}&size=250";

                
                string body = $@"
            <div style='font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; border-radius: 10px; max-width: 600px; margin: auto;'>
                <div style='background-color: #E50914; color: white; padding: 10px; text-align: center; border-radius: 8px 8px 0 0;'>
                    <h2>Sikeres mozijegy foglalás! 🍿</h2>
                </div>
                <div style='padding: 20px; background-color: #fafafa;'>
                    <p>Kedves Vásárlónk,</p>
                    <p>A fizetés sikeresen megtörtént. Íme a jegyed részletei:</p>
                    <ul style='list-style-type: none; padding: 0;'>
                        <li style='margin-bottom: 10px;'>🎬 <b>Film:</b> {movieTitle}</li>
                        <li style='margin-bottom: 10px;'>🕒 <b>Időpont:</b> {showTime}</li>
                        <li style='margin-bottom: 10px;'>🪑 <b>Szék(ek):</b> {seats}</li>
                    </ul>
                    <p style='text-align: center; margin-top: 30px;'><b>A belépéshez mutasd be ezt a QR kódot a mozi bejáratánál:</b></p>
                    <div style='text-align: center; margin: 20px 0;'>
                        <img src='{qrCodeUrl}' alt='Jegy QR Kód' style='width: 250px; height: 250px; border: 2px solid #ccc; border-radius: 10px;' />
                    </div>
                    <p style='text-align: center; color: #666; font-size: 12px;'>Jegy azonosító: {ticketCode}</p>
                </div>
            </div>";
                

                using var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, appPassword)
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = $"Mozijegyed: {movieTitle}",
                    Body = body,
                    IsBodyHtml = true
                };

                smtp.Send(message);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba az email küldésekor: " + ex.Message);
            }
        }
    }
}
