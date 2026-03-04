using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MozizzAPI.DTOS;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly MozizzContext _context;

        public UserProfileController(MozizzContext context)
        {
            _context = context;
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var user = await _context.Users
                .Select(u => new { u.UserId, u.Name, u.Email, u.Phone, u.CreatedAt }).FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound("Felhasználó nem található.");
            return Ok(user);
        }

        [HttpPut("Update/{userId}")]
        public async Task<IActionResult> UpdateProfile(int userId, [FromBody] User updateData)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.Name = updateData.Name;
            user.Phone = updateData.Phone;

            await _context.SaveChangesAsync();
            return Ok(new { uzenet = "Profil sikeresen frissítve!" });
        }

        [HttpPost("ChangePassword/{userId}")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] PasswordChangeDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            {
                return BadRequest("A jelenlegi jelszó nem megfelelő!");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { uzenet = "Jelszó sikeresen megváltoztatva!" });
        }
    }
}
