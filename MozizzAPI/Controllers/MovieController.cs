using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : Controller
    {
        private readonly MozizzContext _context; 
        private readonly Cloudinary _cloudinary;
        public MovieController(MozizzContext context, IConfiguration config)
        {
            _context = context;

            var account = new Account(
                config["CloudinarySettings:CloudName"],
                config["CloudinarySettings:ApiKey"],
                config["CloudinarySettings:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
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
        public async Task<IActionResult> NewMovie([FromForm] Movie movie, IFormFile? imageFile)
        {
            try
            {
                if (imageFile != null)
                {
                    using var stream = imageFile.OpenReadStream();
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imageFile.FileName, stream),
                        Folder = "movies",
                        UseFilename = true,      
                        UniqueFilename = false,  
                        Overwrite = true        
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    movie.Img = uploadResult.PublicId + "." + uploadResult.Format;
                }

                _context.Movies.Add(movie);
                _context.SaveChanges();
                return Ok(new { üzenet = "Sikeres rögzítés!", fajlnev = movie.Img });
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba: {ex.Message}");
            }
        }


        [HttpDelete("DeleteMovie/{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.MovieId == id);
            if (movie == null) return NotFound();

            
            var publicId = movie.Img.Split('.')[0];
            var deletionParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deletionParams);

           
            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return Ok("Törölve mindenhonnan.");
        }


        [HttpPut("ModifyMovie")]
        public async Task<IActionResult> ModifyMovie([FromForm] Movie movie, IFormFile? imageFile)
        {
            try
            {
                
                var m = _context.Movies.FirstOrDefault(x => x.MovieId == movie.MovieId);
                if (m == null) return NotFound("Nem található a módosítani kívánt film.");

                
                m.Title = movie.Title;
                m.Description = movie.Description;

                if (imageFile != null)
                {
                    
                    if (!string.IsNullOrEmpty(m.Img))
                    {
                        
                        var oldPublicId = m.Img.Contains(".") ? m.Img.Split('.')[0] : m.Img;
                        await _cloudinary.DestroyAsync(new DeletionParams(oldPublicId));
                    }

                   
                    using var stream = imageFile.OpenReadStream();
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imageFile.FileName, stream),
                        Folder = "movies",
                        UseFilename = true,     
                        UniqueFilename = false,  
                        Overwrite = true         
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                   
                    m.Img = uploadResult.PublicId + "." + uploadResult.Format;
                }

                _context.Movies.Update(m);
                _context.SaveChanges();
                return Ok(new { üzenet = "Sikeres módosítás!", uj_fajlnev = m.Img });
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a módosítás közben: {ex.Message}");
            }
        }
    }
}