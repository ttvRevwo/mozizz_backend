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




        [HttpGet("TopMovies")]
        public async Task<IActionResult> GetTopMovies()
        {
            var topMovies = await _context.Reservedseats
                .Include(rs => rs.Reservation)
                    .ThenInclude(r => r.Showtime)
                        .ThenInclude(s => s.Movie)
                .GroupBy(rs => rs.Reservation.Showtime.Movie.Title)
                .Select(g => new
                {
                    FilmCim = g.Key,
                    JegyekSzama = g.Count(),
                    Bevetel = g.Count() * 2500
                })
                .OrderByDescending(x => x.JegyekSzama)
                .Take(3)
                .ToListAsync();

            return Ok(topMovies);
        }

        [HttpGet("ShowtimeOccupancy")]
        public async Task<IActionResult> GetOccupancy()
        {
            var today = DateTime.Today;

            var allShowtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Reservations)
                    .ThenInclude(r => r.Reservedseats)
                .OrderByDescending(s => s.ShowDate) 
                .ThenByDescending(s => s.ShowTime1)
                .ToListAsync();

            var report = new
            {
                AktivVetitesek = allShowtimes
                    .Where(s => s.ShowDate >= today)
                    .Select(s => new
                    {
                        Film = s.Movie.Title,
                        Idopont = s.ShowDate.ToShortDateString() + " " + s.ShowTime1,
                        EladottJegyek = s.Reservations.Sum(r => r.Reservedseats.Count),
                        Telitettseg = Math.Round((double)s.Reservations.Sum(r => r.Reservedseats.Count) / 50 * 100, 2) + "%"
                    }),

                ArchivVetitesek = allShowtimes
                    .Where(s => s.ShowDate < today)
                    .Select(s => new
                    {
                        Film = s.Movie.Title,
                        Idopont = s.ShowDate.ToShortDateString() + " " + s.ShowTime1,
                        EladottJegyek = s.Reservations.Sum(r => r.Reservedseats.Count),
                        Telitettseg = Math.Round((double)s.Reservations.Sum(r => r.Reservedseats.Count) / 50 * 100, 2) + "%"
                    })
            };

            return Ok(report);
        }


        [HttpGet("MonthlyRevenue")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            var data = await _context.Reservations
                .Include(r => r.Reservedseats)
                .Where(r => r.Status == "Confirmed")
                .GroupBy(r => new { r.ReservationDate.Year, r.ReservationDate.Month })
                .Select(g => new
                {
                    Ev = g.Key.Year,
                    Honap = g.Key.Month,
                    Bevetel = g.Sum(r => r.Reservedseats.Count) * 2500,
                    EladottJegyek = g.Sum(r => r.Reservedseats.Count),
                    FoglalasokSzama = g.Count()
                })
                .OrderByDescending(x => x.Ev)
                .ThenByDescending(x => x.Honap)
                .Take(12)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("YearlyRevenue")]
        public async Task<IActionResult> GetYearlyRevenue()
        {
            var data = await _context.Reservations
                .Include(r => r.Reservedseats)
                .Where(r => r.Status == "Confirmed")
                .GroupBy(r => r.ReservationDate.Year)
                .Select(g => new
                {
                    Ev = g.Key,
                    Bevetel = g.Sum(r => r.Reservedseats.Count) * 2500,
                    EladottJegyek = g.Sum(r => r.Reservedseats.Count),
                    FoglalasokSzama = g.Count()
                })
                .OrderByDescending(x => x.Ev)
                .ToListAsync();

            return Ok(data);
        }




    }
}
