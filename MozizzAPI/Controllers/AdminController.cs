using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly MozizzContext _context;

        public AdminController(MozizzContext context)
        {
            _context = context;
        }

        [HttpGet("DailyReport")]
        public async Task<IActionResult> GetDailyReport()
        {
            var today = DateTime.Today;

            var stats = await _context.Reservations
      .Include(r => r.Reservedseats)
      .Where(r => r.ReservationDate.Date == today && r.Status == "Confirmed")
      .ToListAsync();

            var totalRevenue = stats.Sum(r => r.Reservedseats.Count * 2500);
            var totalTickets = stats.Sum(r => r.Reservedseats.Count);

            return Ok(new
            {
                Datum = today.ToShortDateString(),
                MaiBevetel = totalRevenue + " Ft",
                EladottJegyek = totalTickets + " db",
                FoglalasokSzama = stats.Count
            });
        }
    }
}
