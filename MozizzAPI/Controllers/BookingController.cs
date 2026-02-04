using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly MozizzContext _context;

        public BookingController(MozizzContext context)
        {
            _context = context;
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
    }
}
