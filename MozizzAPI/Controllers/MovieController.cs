using Microsoft.AspNetCore.Mvc;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : Controller
    {
        private readonly MozizzContext _context;

        public MovieController(MozizzContext context)
        {
            _context = context;
        }

        [HttpGet("GetMovies")]
        public IActionResult GetAllMovies()
        {
            try
            {
                var filmek = _context.Movies.ToList();
                return Ok(filmek);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<Movie> { new Movie { MovieId = -1, Description = $"Hiba: {ex.Message}" } });
            }
        }

        [HttpGet("MovieById")]
        public IActionResult GetMovieById(int id)
        {
            var valasz = _context.Movies.FirstOrDefault(u => u.MovieId == id);
            return valasz != null ? Ok(valasz) : BadRequest("Nincs ilyen film.");
        }

        [HttpDelete("DeleteMovie")]
        public IActionResult DeleteMovie(int id)
        {
            try
            {
                var movie = _context.Movies.FirstOrDefault(f => f.MovieId == id);
                if (movie == null) return BadRequest("Nincs ilyen film.");

                _context.Movies.Remove(movie);
                _context.SaveChanges();
                return Ok("Sikeres törlés.");
            }
            catch (Exception ex)
            {
                return BadRequest("Hiba: " + ex.Message);
            }
        }

        [HttpPost("NewMovie")]
        public IActionResult NewMovie(Movie movie)
        {
            try
            {
                _context.Movies.Add(movie);
                _context.SaveChanges();
                return Ok("Sikeres rögzítés!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba: {ex.Message}");
            }
        }
    }
}