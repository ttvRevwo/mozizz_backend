using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MozizzAPI.DTOS;
using MozizzAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MozizzContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(MozizzContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AdminTest")]
        public IActionResult AdminTest()
        {
            return Ok("Szia Admin! Sikeresen beléptél a titkos végpontra!"); 
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Email == dto.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    return BadRequest("Hibás email cím vagy jelszó!");
                }

                var jwtSettings = _configuration.GetSection("Jwt");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Customer")
                };

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    message = "Sikeres bejelentkezés!",
                    token = tokenString,
                    userId = user.UserId,
                    name = user.Name,
                    role = user.Role?.RoleName ?? "Customer"
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a bejelentkezés során: {ex.Message}");
            }
        }

        [HttpPost("RegisterRequest")]
        public IActionResult RegisterRequest([FromBody] RegisterDto dto)
        {
            try
            {
                var expiredCodes = _context.UserVerifications.Where(v => v.ExpiresAt < DateTime.UtcNow);
                _context.UserVerifications.RemoveRange(expiredCodes);

                if (_context.Users.Any(u => u.Email == dto.Email))
                    return BadRequest("Ez az email már regisztrálva van!");

                string code = new Random().Next(100000, 999999).ToString();
                var expiry = DateTime.UtcNow.AddMinutes(5);

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
                SendGmail(dto.Email, code);

                return Ok("Kód elküldve!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("VerifyAndRegister")]
        public IActionResult VerifyAndRegister(string email, string code, [FromBody] RegisterDto dto)
        {
            var auth = _context.UserVerifications.FirstOrDefault(v => v.Email == email && v.Code == code);

            if (auth == null || auth.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Hibás vagy lejárt kód!");

            var finalUser = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                RoleId = 2
            };

            _context.Users.Add(finalUser);
            _context.UserVerifications.Remove(auth);
            _context.SaveChanges();

            return Ok("Sikeres regisztráció!");
        }

        private void SendGmail(string targetEmail, string code)
        {
            var emailConfig = _configuration.GetSection("EmailSettings");
            string senderEmail = emailConfig["Email"];
            string appPassword = emailConfig["Password"];

            var fromAddress = new MailAddress(senderEmail, "Mozizz Cinema");
            var toAddress = new MailAddress(targetEmail);

            string body = $@"
                <div style='font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; border-radius: 10px; max-width: 500px;'>
                    <h3 style='color: #333;'>Üdvözlünk a Mozizz alkalmazásban!</h3>
                    <p>A regisztrációd befejezéséhez kérjük használd az alábbi 6 jegyű kódot:</p>
                    <div style='background-color: #f4f4f4; padding: 15px; text-align: center; border-radius: 5px;'>
                        <h2 style='color: blue; letter-spacing: 5px; margin: 0;'>{code}</h2>
                    </div>
                    <p style='color: #666; font-size: 14px; margin-top: 20px;'>A kód 5 percig érvényes.</p>
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
                Subject = "Regisztrációs kód - Mozizz",
                Body = body,
                IsBodyHtml = true
            };

            smtp.Send(message);
        }
    }
}