using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeController : ControllerBase
    {

        private readonly MozizzContext _context;

        public ShowtimeController(MozizzContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllShowtimes")]
        public IActionResult GetAllShowtimes()
        {
            try
            {
                var showtimes = _context.Showtimes
                    .Include(s => s.Movie)
                    .Include(s => s.Hall)
                    .Select(s => new {
                        s.ShowtimeId,
                        MovieTitle = s.Movie.Title,
                        HallName = s.Hall.Name,
                        Date = s.ShowDate.ToShortDateString(),
                        Time = s.ShowTime1.ToString()
                    })
                    .ToList();

                return Ok(showtimes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { hiba = ex.Message });
            }
        }


        [HttpGet("GetByMovie/{movieId}")]
        public IActionResult GetByMovie(int movieId)
        {
            try
            {
                var result = _context.Showtimes
                    .Where(s => s.MovieId == movieId) 
                    .Include(s => s.Hall)
                    .Select(s => new {
                        s.ShowtimeId,
                        s.ShowDate,
                        s.ShowTime1,
                        HallName = s.Hall.Name
                    })
                    .OrderBy(s => s.ShowDate)
                    .ThenBy(s => s.ShowTime1)
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var showtime = _context.Showtimes
                    .Include(s => s.Movie)
                    .Include(s => s.Hall)
                    .FirstOrDefault(s => s.ShowtimeId == id);

                if (showtime == null) return NotFound("A vetítés nem található.");

                return Ok(new
                {
                    showtime.ShowtimeId,
                    MovieTitle = showtime.Movie.Title,
                    HallName = showtime.Hall.Name,
                    Date = showtime.ShowDate,
                    Time = showtime.ShowTime1
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("NewShowtime")]
        public IActionResult Create(Showtime showtime)
        {
            try
            {
                _context.Showtimes.Add(showtime);
                _context.SaveChanges();
                return Ok(new { uzenet = "Vetítés sikeresen rögzítve!", id = showtime.ShowtimeId });
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a mentés során: {ex.Message}");
            }
        }
    }
}
