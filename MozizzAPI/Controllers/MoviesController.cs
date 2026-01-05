using Microsoft.AspNetCore.Mvc;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : Controller
    {
        [HttpGet("GetMovies")]
        public IActionResult GetAllMovies()
        {
            using (var context = new MozizzContext())
            {
                try
                {
                    List<Movie> filmek = context.Movies.ToList();
                    return Ok(filmek);
                }
                catch (Exception ex)
                {
                    List<Movie> valasz = new List<Movie>();
                    Movie hiba = new Movie()
                    {
                        MovieId = -1,
                        Description = $"Hiba a betöltés során: {ex.Message}"
                    };
                    valasz.Add(hiba);
                    return BadRequest(valasz);
                }
            }
        }
    }
}
