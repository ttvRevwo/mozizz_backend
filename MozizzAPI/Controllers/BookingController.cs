using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MozizzAPI.DTOS;
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
                        return BadRequest("Sajnáljuk, időközben valaki már lefoglalta az egyik választott széket.");

                    var reservation = new Reservation
                    {
                        UserId = dto.UserId,
                        ShowtimeId = dto.ShowtimeId,
                        ReservationDate = DateTime.Now,
                        Status = "confirmed"
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

                    return Ok(new { message = "Sikeres foglalás!", reservationId = reservation.ReservationId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest($"Hiba a foglalás során: {ex.Message}");
                }
            }
        }
    }
}
