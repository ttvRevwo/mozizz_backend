using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}
