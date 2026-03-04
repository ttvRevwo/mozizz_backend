using Microsoft.EntityFrameworkCore;
using MozizzAPI.Models;

namespace MozizzAPI.Services
{
    public class BookingCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookingCleanupService> _logger;

        public BookingCleanupService(IServiceScopeFactory scopeFactory, ILogger<BookingCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("A Jegy-Takarító elindult");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<MozizzContext>();

                        var expirationTime = DateTime.Now.AddMinutes(-15);

                        var expiredBookings = await context.Reservations
                            .Include(r => r.Reservedseats)
                            .Where(r => r.Status == "Pending" && r.ReservationDate < expirationTime)
                            .ToListAsync();

                        if (expiredBookings.Any())
                        {
                            _logger.LogWarning($"{expiredBookings.Count} lejárt foglalás törlése folyamatban");

                            context.Reservations.RemoveRange(expiredBookings);
                            await context.SaveChangesAsync();

                            _logger.LogInformation("A lejárt székek felszabadítva!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Hiba a takarítás közben: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}