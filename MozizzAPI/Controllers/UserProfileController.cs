using System.Net;
using System.Net.Mail;
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
        private readonly IConfiguration _configuration;

        public UserProfileController(MozizzContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

        [HttpPost("ForgotPassword")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
                if (user == null)
                    return BadRequest("Nem található felhasználó ezzel az email címmel!");

                var expiredCodes = _context.UserVerifications.Where(v => v.ExpiresAt < DateTime.UtcNow);
                _context.UserVerifications.RemoveRange(expiredCodes);

                string code = new Random().Next(100000, 999999).ToString();
                var expiry = DateTime.UtcNow.AddMinutes(10);

                var existing = _context.UserVerifications.FirstOrDefault(v => v.Email == dto.Email);
                if (existing != null)
                {
                    existing.Code = code;
                    existing.ExpiresAt = expiry;
                }
                else
                {
                    _context.UserVerifications.Add(new UserVerification { Email = dto.Email, Code = code, ExpiresAt = expiry });
                }

                _context.SaveChanges();
                SendPasswordResetEmail(dto.Email, code, user.Name);

                return Ok("Jelszóvisszaállító kód elküldve!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba: {ex.Message}");
            }
        }

        private void SendPasswordResetEmail(string targetEmail, string code, string name)
        {
            var emailConfig = _configuration.GetSection("EmailSettings");
            string senderEmail = emailConfig["Email"];
            string appPassword = emailConfig["Password"];

            var fromAddress = new MailAddress(senderEmail, "Mozizz Cinema");
            var toAddress = new MailAddress(targetEmail);

            string body = $@"
                <div style='font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; border-radius: 10px; max-width: 500px;'>
                    <h3 style='color: #333;'>Jelszó visszaállítás - Mozizz</h3>
                    <p>Kedves {name}!</p>
                    <p>A jelszavad visszaállításához használd az alábbi 6 jegyű kódot:</p>
                    <div style='background-color: #f4f4f4; padding: 15px; text-align: center; border-radius: 5px;'>
                        <h2 style='color: #E50914; letter-spacing: 5px; margin: 0;'>{code}</h2>
                    </div>
                    <p style='color: #666; font-size: 14px; margin-top: 20px;'>A kód 10 percig érvényes.</p>
                    <p style='color: #666; font-size: 12px;'>Ha nem te kérted, hagyd figyelmen kívül ezt az emailt.</p>
                </div>";

            using var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, appPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Jelszó visszaállítás - Mozizz",
                Body = body,
                IsBodyHtml = true
            };

            smtp.Send(message);
        }
    }

}
