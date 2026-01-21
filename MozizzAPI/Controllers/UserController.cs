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

   
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var felhasznalok = _context.Users.ToList();
                return Ok(felhasznalok);
            }
            catch (Exception ex)
            {
                return BadRequest(new { üzenet = "Hiba a lekérdezés során", hiba = ex.Message });
            }
        }

      
        [HttpGet("UserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == id);
                if (user == null) return NotFound("Nincs ilyen felhasználó!");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        
        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(f => f.UserId == id);
                if (user == null) return NotFound("Nincs ilyen felhasználó!");

                _context.Users.Remove(user);
                _context.SaveChanges();
                return Ok("Felhasználó sikeresen törölve.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a törlés közben: {ex.Message}");
            }
        }

        
        [HttpPut("ModifyUser")]
        public IActionResult ModifyUser(User user)
        {
            try
            {
                var létezik = _context.Users.Any(u => u.UserId == user.UserId);
                if (!létezik) return NotFound("Nem található a módosítani kívánt felhasználó.");

                _context.Users.Update(user);
                _context.SaveChanges();
                return Ok("Sikeres módosítás.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a módosítás közben: {ex.Message}");
            }
        }

        
        [HttpDelete("CleanupVerificationCodes")]
        public IActionResult CleanupCodes()
        {
            try
            {
                var lejártak = _context.UserVerifications.Where(v => v.ExpiresAt < DateTime.Now);
                int darab = lejártak.Count();
                _context.UserVerifications.RemoveRange(lejártak);
                _context.SaveChanges();
                return Ok($"{darab} lejárt kód törölve az adatbázisból.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}