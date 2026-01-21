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
              
                return BadRequest(new List<Movie> {
                    new Movie { MovieId = -1, Title = "Hiba", Description = ex.Message }
                });
            }
        }

        [HttpGet("MovieById/{id}")]
        public IActionResult GetMovieById(int id)
        {
            try
            {
                var film = _context.Movies.FirstOrDefault(m => m.MovieId == id);
                if (film == null) return NotFound("A kért film nem található.");

                return Ok(film);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPost("NewMovie")]
        public IActionResult NewMovie([FromBody] Movie movie)
        {
            try
            {
                _context.Movies.Add(movie);
                _context.SaveChanges();
                return Ok(new { üzenet = "Sikeres rögzítés!", id = movie.MovieId });
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a rögzítés közben: {ex.Message}");
            }
        }

       
        [HttpDelete("DeleteMovie/{id}")]
        public IActionResult DeleteMovie(int id)
        {
            try
            {
                var movie = _context.Movies.FirstOrDefault(m => m.MovieId == id);
                if (movie == null) return NotFound("Nincs ilyen film az adatbázisban.");

                _context.Movies.Remove(movie);
                _context.SaveChanges();
                return Ok("A film és a hozzá kapcsolódó adatok törölve.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a törlés során: {ex.Message}");
            }
        }

       
        [HttpPut("ModifyMovie")]
        public IActionResult ModifyMovie([FromBody] Movie movie)
        {
            try
            {
                var létezik = _context.Movies.Any(m => m.MovieId == movie.MovieId);
                if (!létezik) return NotFound("Nem található a módosítani kívánt film.");

                _context.Movies.Update(movie);
                _context.SaveChanges();
                return Ok("A film adatai sikeresen frissítve.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a módosítás közben: {ex.Message}");
            }
        }
    }
}