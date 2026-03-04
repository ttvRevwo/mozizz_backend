using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly MozizzContext _context;

        public TicketController(MozizzContext context)
        {
            _context = context;
        }

        [HttpGet("ValidateTicket/{ticketCode}")]
        public IActionResult ValidateTicket(string ticketCode)
        {
            try
            {
                
                var ticketInfo = _context.Tickets
                    .Include(t => t.Reservation)
                        .ThenInclude(r => r.Showtime)
                            .ThenInclude(s => s.Movie)
                    .Include(t => t.Reservation)
                        .ThenInclude(r => r.Reservedseats)
                            .ThenInclude(rs => rs.Seat)
                    .FirstOrDefault(t => t.TicketCode == ticketCode);

                if (ticketInfo == null)
                {
                    return NotFound(new { uzenet = "Érvénytelen jegy! Ez a kód nem létezik a rendszerben." });
                }

               
                var seatNumbers = string.Join(", ", ticketInfo.Reservation.Reservedseats.Select(rs => rs.Seat.SeatNumber));

             
                return Ok(new
                {
                    uzenet = "Érvényes jegy! Jó szórakozást!",
                    film = ticketInfo.Reservation.Showtime.Movie.Title,
                    idopont = ticketInfo.Reservation.Showtime.ShowDate.ToShortDateString() + " " + ticketInfo.Reservation.Showtime.ShowTime1,
                    szekek = seatNumbers,
                    vasarloId = ticketInfo.Reservation.UserId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { hiba = "Hiba történt az ellenőrzés során: " + ex.Message });
            }
        }

        [Authorize]
        [HttpGet("MyTickets/{userId}")]
        public IActionResult GetMyTickets(int userId)

        {
            var myTickets = _context.Tickets
                .Include(t => t.Reservation)
                .Where(t => t.Reservation.UserId == userId)
                .Select(t => new {
                    t.TicketCode,
                    t.IssuedDate,
                    Status = t.Reservation.Status

                })
                .ToList();
            return Ok(myTickets);

        }




    }
}
