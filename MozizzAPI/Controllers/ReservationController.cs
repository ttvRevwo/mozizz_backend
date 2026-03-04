using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly MozizzContext _context;

        public ReservationController(MozizzContext context)
        {
            _context = context;
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

    }
}
