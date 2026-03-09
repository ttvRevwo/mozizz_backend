using System.Net;
using System.Net.Mail;
using MozizzAPI.Models;

namespace MozizzAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly MozizzContext _context;

        public EmailService(IConfiguration configuration, MozizzContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public void SendEmail(int userId, string targetEmail, string emailType, string subject, string body)
        {
            var log = new Emaillog
            {
                UserId = userId,
                EmailType = emailType,
                Subject = subject,
                Body = body,
                SentAt = DateTime.Now
            };

            try
            {
                var emailConfig = _configuration.GetSection("EmailSettings");
                string senderEmail = emailConfig["Email"];
                string appPassword = emailConfig["Password"];

                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, appPassword)
                };

                using var message = new MailMessage(senderEmail, targetEmail, subject, body) { IsBodyHtml = true };

                
                log.Status = "Sent";
            }
            catch (Exception ex)
            {
                log.Status = "Failed";
                Console.WriteLine("Hiba az e-mail küldésekor: " + ex.Message);
            }
            finally
            {
                _context.Emaillogs.Add(log);
                _context.SaveChanges();
            }
        }


    }
}
