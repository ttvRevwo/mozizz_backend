using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MozizzAPI.DTOS;
using MozizzAPI.Models;
using QRCoder;
using Microsoft.Extensions.Configuration;
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

        public BookingController(MozizzContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

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

                    
                    string egyediJegyKod = Guid.NewGuid().ToString();
                    var ujJegy = new Ticket
                    {
                        ReservationId = reservation.ReservationId,
                        TicketCode = egyediJegyKod
                    };
                    _context.Tickets.Add(ujJegy);
                    _context.SaveChanges(); 

                    transaction.Commit();

                    
                    var user = _context.Users.Find(dto.UserId);
                    var showtime = _context.Showtimes.Include(s => s.Movie).FirstOrDefault(s => s.ShowtimeId == dto.ShowtimeId);
                    var bookedSeats = _context.Seats.Where(s => dto.SeatIds.Contains(s.SeatId)).Select(s => s.SeatNumber).ToList();

                    string seatsString = string.Join(", ", bookedSeats); 
                    string showDateStr = showtime.ShowDate.ToShortDateString() + " " + showtime.ShowTime1.ToString();

                   

                    return Ok(new { message = "Sikeres foglalás! Az e-mailt kiküldtük.", reservationId = reservation.ReservationId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest($"Hiba a foglalás során: {ex.Message}");
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


        
        
    }
}
