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

        [HttpDelete("DelUser")]
        public IActionResult DeleteUser(int id)
        {
            using (var context = new MozizzContext())
            {
                try
                {

                    if (context.Users.Select(u => u.Id).Contains(id))
                    {
                        User torlendo = new User { Id = id };
                        context.Users.Remove(torlendo);
                        context.SaveChanges();
                        return Ok("Sikeres törlés!");
                    }
                    else
                    {
                        return NotFound("Nincs ilyen felhasználó!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a törlés közben: {ex.Message}");
                }
            }
        }

    }
}
