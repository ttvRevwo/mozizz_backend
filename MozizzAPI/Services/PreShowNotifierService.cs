using Microsoft.EntityFrameworkCore;
using MozizzAPI.Models;
using System.Net.Mail;
using System.Net;

namespace MozizzAPI.Services
{
    public class PreShowNotifierService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public PreShowNotifierService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }




        private void SendReminderEmail(string targetEmail, string movieTitle, string time)
        {
            try
            {
                var emailConfig = _configuration.GetSection("EmailSettings");
                string senderEmail = emailConfig["Email"];
                string appPassword = emailConfig["Password"];

                string body = $@"
                    <div style='font-family: Arial; padding: 20px; border: 1px solid #ddd; border-radius: 10px; max-width: 500px;'>
                        <h2 style='color: #E50914; text-align: center;'>Készítsd a popcornt! 🍿</h2>
                        <p>Szia!</p>
                        <p>Emlékeztetünk, hogy a <b>{movieTitle}</b> című filmed hamarosan (<b>{time}</b>-kor) kezdődik.</p>
                        <p>Kérjük, érkezz meg időben, hogy át tudd venni az esetlegesen vásárolt rágcsálnivalókat.</p>
                        <p>Jó szórakozást kíván a Mozizz csapata!</p>
                    </div>";

                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, appPassword)
                };

                using var message = new MailMessage(senderEmail, targetEmail, $"Hamarosan kezdődik: {movieTitle}", body) { IsBodyHtml = true };
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba az emlékeztető e-mail küldésekor: " + ex.Message);
            }
        }
    }
}
