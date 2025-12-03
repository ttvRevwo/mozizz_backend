using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("User")]
        public IActionResult GetAllUsers()
        {
            using (var context = new MozizzContext())
            {
                try
                {
                    List<User> felhasznalok = context.Users.ToList();
                    return Ok(felhasznalok);
                }
                catch (Exception ex)
                {
                    List<User> valasz = new List<User>();
                    User hiba = new User()
                    {
                        UserId = -1,
                        Name = $"Hiba a betöltés során: {ex.Message}"
                    };
                    valasz.Add(hiba);
                    return BadRequest(valasz);
                }
            }
        }

        [HttpGet("UserById")]
        public IActionResult GetUserById(int id)
        {
            using (var context = new MozizzContext())
            {
                try
                {
                    User valasz = context.Users.FirstOrDefault(u => u.UserId == id);
                    if (valasz != null)
                    {
                        return Ok(valasz);
                    }
                    else
                    {
                        User hiba = new User()
                        {
                            UserId = -1,
                            Name = "Nincs ilyen azonosítójú felhasználó!"
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
    }
}
