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
  
                var ticket = _context.Tickets
                    .Include(t => t.Reservation)
                        .ThenInclude(r => r.Showtime)
                            .ThenInclude(s => s.Movie)
                    .Include(t => t.Reservation)
                        .ThenInclude(r => r.Reservedseats)
                            .ThenInclude(rs => rs.Seat)
                    .FirstOrDefault(t => t.TicketCode == ticketCode);

               
                if (ticket == null)
                {
                    return NotFound(new { uzenet = "Érvénytelen jegy! Ez a kód nem létezik." });
                }

                
                if (ticket.IsUsed == true)
                {
                    return BadRequest(new
                    {
                        uzenet = "Ezt a jegyet már FELHASZNÁLTÁK!",
                        idopont = ticket.IssuedDate 
                    });
                }

           
                ticket.IsUsed = true;
                _context.SaveChanges(); 

                var seatNumbers = string.Join(", ", ticket.Reservation.Reservedseats.Select(rs => rs.Seat.SeatNumber));

                return Ok(new
                {
                    uzenet = "Érvényes jegy! Jó szórakozást!",
                    film = ticket.Reservation.Showtime.Movie.Title,
                    idopont = ticket.Reservation.Showtime.ShowDate.ToShortDateString() + " " + ticket.Reservation.Showtime.ShowTime1,
                    szekek = seatNumbers
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { hiba = "Hiba történt: " + ex.Message });
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
                    t.IsUsed,
                    Status = t.Reservation.Status
                })
                .ToList();
            return Ok(myTickets);

        }




    }
}
