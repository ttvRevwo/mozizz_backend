using Microsoft.AspNetCore.Mvc;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MozizzContext _context;

        public UserController(MozizzContext context)
        {
            _context = context;
        }

        [HttpGet("User")]
        public IActionResult GetAllUsers()
        {
            try
            {
                return Ok(_context.Users.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new List<User> { new User { UserId = -1, Name = $"Hiba: {ex.Message}" } });
            }
        }

        [HttpPost("NewUser")]
        public IActionResult NewUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return Ok("Sikeres rögzítés!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba: {ex.Message}");
            }
        }

        [HttpDelete("DeleteUser")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(f => f.UserId == id);
            if (user == null) return BadRequest("Nincs ilyen felhasználó!");

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok("Törölve.");
        }
    }
}