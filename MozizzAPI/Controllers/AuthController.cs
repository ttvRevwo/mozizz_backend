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
                var expiredCodes = _context.UserVerifications.Where(v => v.ExpiresAt < DateTime.Now);
                _context.UserVerifications.RemoveRange(expiredCodes);
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

            if (auth.ExpiresAt < DateTime.Now)
            {
                _context.UserVerifications.Remove(auth);
                _context.SaveChanges();
                return BadRequest("A kód lejárt! Kérj újat.");
            }

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
            string senderEmail = emailConfig["Email"];
            string appPassword = emailConfig["Password"];

            var fromAddress = new MailAddress(senderEmail, "Mozizz Cinema");
            var toAddress = new MailAddress(targetEmail);

            const string subject = "Regisztrációs kód - Mozizz";

            // AZ ÚJ FORMÁZOTT SZÖVEG:
            string body = $@"
                <div style='font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; border-radius: 10px; max-width: 500px;'>
                    <h3 style='color: #333;'>Üdvözlünk a Mozizz alkalmazásban!</h3>
                    <p>A regisztrációd befejezéséhez kérjük használd az alábbi 6 jegyű kódot:</p>
                    <div style='background-color: #f4f4f4; padding: 15px; text-align: center; border-radius: 5px;'>
                        <h2 style='color: blue; letter-spacing: 5px; margin: 0;'>{code}</h2>
                    </div>
                    <p style='color: #666; font-size: 14px; margin-top: 20px;'>A kód 5 percig érvényes.</p>
                    <hr style='border: 0; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px;'>Ha nem te indítottad a regisztrációt, kérjük hagyd figyelmen kívül ezt az üzenetet.</p>
                </div>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail, appPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}