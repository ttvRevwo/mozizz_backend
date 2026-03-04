using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting; // <-- EZ HIÁNYZOTT
using MozizzAPI.Models;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MozizzAPI.Services
{
    public class PreShowNotifierService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public PreShowNotifierService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<MozizzContext>();
                    var now = DateTime.Now;

                    var upcomingReservations = await context.Reservations
                        .Include(r => r.User)
                        .Include(r => r.Showtime)
                            .ThenInclude(s => s.Movie)
                        .Where(r => r.Status == "confirmed" && r.IsReminderSent != true && r.Showtime.ShowDate.Date == now.Date)
                        .ToListAsync(stoppingToken);

                    foreach (var reservation in upcomingReservations)
                    {

                        DateTime showtimeStart = reservation.Showtime.ShowDate.Date.Add(reservation.Showtime.ShowTime1);


                        if (showtimeStart <= now.AddHours(2) && showtimeStart > now)
                        {

                            SendReminderEmail(reservation.UserId, reservation.User.Email, reservation.Showtime.Movie.Title, showtimeStart.ToString("HH:mm"), context);


                            reservation.IsReminderSent = true;
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }


        
        private void SendReminderEmail(int userId, string targetEmail, string movieTitle, string time, MozizzContext context)
        {
            var log = new Emaillog
            {
                UserId = userId,
                EmailType = "Reminder",
                Subject = $"Hamarosan kezdődik: {movieTitle}",
                SentAt = DateTime.Now
            };

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

                log.Body = body; 

                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, appPassword)
                };

                using var message = new MailMessage(senderEmail, targetEmail, log.Subject, body) { IsBodyHtml = true };
                smtp.Send(message);

               
                log.Status = "Sent";
                Console.WriteLine($"[LOG] E-mail sikeresen elküldve neki: {targetEmail}");
            }
            catch (Exception ex)
            {
              
                log.Status = "Failed";
                Console.WriteLine($"[LOG] Hiba az e-mail küldésekor: {ex.Message}");
            }
            finally
            {
                context.Emaillogs.Add(log);
            }
        }
    }
}
