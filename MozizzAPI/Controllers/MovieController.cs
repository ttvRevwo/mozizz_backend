using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : Controller
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

        [HttpGet("MovieById")]
        public IActionResult GetMovieById(int id)
        {
            using (var context = new MozizzContext())
            {
                try
                {
                    Movie valasz = context.Movies.FirstOrDefault(u => u.MovieId == id);
                    if (valasz != null)
                    {
                        return Ok(valasz);
                    }
                    else
                    {
                        Movie hiba = new Movie()
                        {
                            MovieId = -1,
                            Title = "Nincs ilyen azonosítójú Film!"
                        };
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba az adatok betöltése során: {ex.Message}");
                }
            }
        }
        [HttpDelete("DelMovie")]
        public IActionResult DeleteMovie(int id)
        {
            using (var context = new MozizzContext())
            {
                try
                {
                    var movie = context.Movies.FirstOrDefault(m => m.MovieId == id);
                    
                    if (movie == null)
                    {
                        return NotFound("Nincs ilyen Film!");
                    }

                    var showtimes = context.Showtimes.Where(s => s.MovieId == movie.MovieId).ToList();
                    
                    foreach (var showtime in showtimes)
                    {
                        var reservations = context.Reservations.Where(r => r.ShowtimeId == showtime.ShowtimeId).ToList();
                        
                        foreach (var reservation in reservations)
                        {
                            var tickets = context.Tickets.Where(t => t.ReservationId == reservation.ReservationId).ToList();
                            
                            foreach (var ticket in tickets)
                            {
                                context.Tickets.Remove(ticket);
                            }

                            var seats = context.Reservedseats.Where(rs => rs.ReservationId == reservation.ReservationId).ToList();
                            
                            foreach (var seat in seats)
                            {
                                context.Reservedseats.Remove(seat);
                            }
                            context.Reservations.Remove(reservation);
                        }
                        context.Showtimes.Remove(showtime);
                    }
                    context.Movies.Remove(movie);
                    context.SaveChanges();
                    
                    return Ok("A film és minden kapcsolódó adat sikeresen törölve.");
                }
                catch (Exception ex)
                {
                    return BadRequest("Hiba a törlés során: " + ex.Message);
                }
            }
        }



        [HttpPost("NewMovie")]
        public IActionResult NewMovie(Movie movie)
        {
            using (var context = new MozizzContext())

                try
                {
                    context.Movies.Add(movie);
                    context.SaveChanges();
                    return Ok("Sikeres rögzítés!");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a rögzítés közben: {ex.Message}");
                }
        }

        [HttpPut("ModifyMovie")]
        public IActionResult ModifyMovie(Movie movie)
        {
            using (var context = new MozizzContext())
            {
                try
                {
                    if (context.Movies.Select(u => u.MovieId).Contains(movie.MovieId))
                    {
                        context.Movies.Update(movie);
                        context.SaveChanges();
                    }
                    ;
                    return Ok("Sikeres módosítás.");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a módosítás közben: {ex.Message}");
                }
            }
        }
    }
}
