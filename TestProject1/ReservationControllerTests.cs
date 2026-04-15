using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozizzAPI.Controllers;
using MozizzAPI.Models;
using MozizzAPI.Services;


namespace TestProject1
{
    [TestClass]
    public class ReservationControllerTests
    {
        private MozizzContext _context = null!;
        private IConfiguration _config = null!;
        private ReservationController _controller = null!;
        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MozizzContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new MozizzContext(options);

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "EmailSettings:Email", "teszt@teszt.hu" },
                    { "EmailSettings:Password", "kamujelszo" }
                }).Build();

            _context.Users.Add(new User { UserId = 1, Name = "Teszt User", Email = "teszt@teszt.hu", PasswordHash = "kamu_jelszo_hash" });
            _context.Movies.Add(new Movie { MovieId = 1, Title = "Teszt Film" });
            _context.Halls.Add(new Hall { HallId = 1, Name = "Teszt Terem" });
            _context.Seats.Add(new Seat { SeatId = 1, HallId = 1, SeatNumber = "A1" });

            _context.Showtimes.AddRange(
                new Showtime
                {
                    ShowtimeId = 1,
                    MovieId = 1,
                    HallId = 1,
                    ShowDate = DateTime.Today.AddDays(2),
                    ShowTime1 = new TimeSpan(18, 0, 0)
                },
                new Showtime
                {
                    ShowtimeId = 2,
                    MovieId = 1,
                    HallId = 1,
                    ShowDate = DateTime.Today,
                    ShowTime1 = DateTime.Now.AddHours(1).TimeOfDay
                }
            );

            _context.Reservations.AddRange(
                new Reservation { ReservationId = 1, UserId = 1, ShowtimeId = 1, ReservationDate = DateTime.Now, Status = "Confirmed" },
                new Reservation { ReservationId = 2, UserId = 1, ShowtimeId = 2, ReservationDate = DateTime.Now, Status = "Confirmed" }
            );

            _context.Reservedseats.Add(new Reservedseat { ReservedSeatId = 1, ReservationId = 1, SeatId = 1 });

            _context.SaveChanges();

            var fakeEmailService = new EmailService(_config, _context);
            _controller = new ReservationController(_context, _config, fakeEmailService);
        }
        [TestMethod]
        public async Task GetMyHistory_LetezoFoglalasokkal_VisszaadjaAListat()
        {
            var result = await _controller.GetMyHistory(1);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = (OkObjectResult)result;
            var history = okResult.Value as IEnumerable<object>;

            Assert.IsNotNull(history);
            Assert.AreEqual(2, history.Count());
        }

        [TestMethod]
        public async Task GetMyHistory_NincsFoglalasa_NotFoundotAd()
        {
            var result = await _controller.GetMyHistory(99);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task CancelReservation_KettoOranBelulVan_BadRequestetAd()
        {
            var result = await _controller.CancelReservation(2);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var badRequest = (BadRequestObjectResult)result;
            Assert.IsTrue(badRequest.Value!.ToString()!.Contains("már nem tudod lemondani"));
        }

        [TestMethod]
        public async Task CancelReservation_NemLetezoFoglalas_NotFoundotAd()
        {
            var result = await _controller.CancelReservation(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

    }
}
