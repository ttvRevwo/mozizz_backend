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
    }
}
