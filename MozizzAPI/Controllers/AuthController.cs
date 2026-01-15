using Microsoft.AspNetCore.Mvc;
using MozizzAPI.DTOS; 
using MozizzAPI.Models;
using System.Net;
using System.Net.Mail;

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

        [HttpPost("RegisterRequest")]
        public IActionResult RegisterRequest([FromBody] RegisterDto dto)
        {
            try
            {
                if (_context.Users.Any(u => u.Email == dto.Email))
                    return BadRequest("Ez az email már regisztrálva van!");

                string code = new Random().Next(100000, 999999).ToString();
                var expiry = DateTime.Now.AddMinutes(5);

                var existing = _context.UserVerifications.FirstOrDefault(v => v.Email == dto.Email);
                if (existing != null)
                {
                    existing.Code = code;
                    existing.ExpiresAt = expiry;
                    _context.UserVerifications.Update(existing);
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

            if (auth == null || auth.ExpiresAt < DateTime.Now)
                return BadRequest("Hibás vagy lejárt kód!");

            var finalUser = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = dto.Password, 
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
            var fromAddress = new MailAddress(emailConfig["Email"], "Mozizz Cinema");
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(emailConfig["Email"], emailConfig["Password"])
            };

            using (var message = new MailMessage(fromAddress, new MailAddress(targetEmail))
            {
                Subject = "Regisztrációs kód",
                Body = $"A kódod: {code}",
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}