
using Microsoft.AspNetCore.Http;
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
        
        [HttpPost("RegisterRequest")]
        public IActionResult RegisterRequest([FromBody] RegisterDto dto)
        {
            using (var context = new MozizzContext())
            {
                try
                {
                    
                    if (context.Users.Any(u => u.Email == dto.Email))
                    {
                        return BadRequest("Ez az email cím már foglalt!");
                    }

                    string verificationCode = new Random().Next(100000, 999999).ToString();
                    var expiry = DateTime.Now.AddMinutes(5);

                    var existing = context.UserVerifications.FirstOrDefault(v => v.Email == dto.Email);
                    if (existing != null)
                    {
                        existing.Code = verificationCode;
                        existing.ExpiresAt = expiry;
                    }
                    else
                    {
                        context.UserVerifications.Add(new UserVerification
                        {
                            Email = dto.Email,
                            Code = verificationCode,
                            ExpiresAt = expiry
                        });
                    }

                    context.SaveChanges();
                    SendGmail(dto.Email, verificationCode);

                    return Ok("Kód elküldve!");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }


        [HttpPost("VerifyAndRegister")]
        public IActionResult VerifyAndRegister(string email, string code, [FromBody] RegisterDto dto)
        {
            using (var context = new MozizzContext())
            {
                var auth = context.UserVerifications.FirstOrDefault(v => v.Email == email && v.Code == code);

                if (auth != null && auth.ExpiresAt > DateTime.Now)
                {
                    
                    var finalUser = new User
                    {
                        Name = dto.Name,
                        Email = dto.Email,
                        PasswordHash = dto.Password, 
                        RoleId = 2 
                    };

                    context.Users.Add(finalUser); 
                    context.UserVerifications.Remove(auth); 
                    context.SaveChanges(); 

                    return Ok("Sikeres registracio!");
                }
                return BadRequest("Hibás kód!");
            }
        }

       
        private void SendGmail(string targetEmail, string code)
        {
            var fromAddress = new MailAddress("dinofordrive@gmail.com", "Mozizz Rendszer");
            var toAddress = new MailAddress(targetEmail);
            const string fromPassword = "#############################"; // Google App Password!
            const string subject = "Mozizz - Regisztrációs kód";
            string body = $@"
                <h3>Üdvözlünk a Mozizz alkalmazásban!</h3>
                <p>A regisztrációd befejezéséhez kérjük használd az alábbi 6 jegyű kódot:</p>
                <h2 style='color:blue;'>{code}</h2>
                <p>A kód 5 percig érvényes.</p>
                <p>Ha nem te indítottad a regisztrációt, kérjük hagyd figyelmen kívül ezt az üzenetet.</p>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
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