using System;
using System.Linq;
using MozizzAPI.Controllers;
using MozizzAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    [TestClass]
    public class TicketControllerTests
    {
        MozizzContext _context = null!;
        TicketController _controller = null!;
        private MozizzContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MozizzContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new MozizzContext(options);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = CreateInMemoryContext();

            _context.Userroles.Add(new Userrole { RoleId = 1, RoleName = "User" });
            _context.Users.Add(new User
            {
                UserId = 1,
                Name = "Teszt Felhasználó",
                Email = "teszt@teszt.hu",
                PasswordHash = "hash",
                RoleId = 1,
                CreatedAt = DateTime.Now
            });

            _context.Movies.Add(new Movie
            {
                MovieId = 1,
                Title = "Inception",
                Duration = 148,
                CreatedAt = DateTime.Now
            });
            _context.Halls.Add(new Hall
            {
                HallId = 1,
                Name = "Terem 1",
                Location = "Fszt.",
                SeatingCapacity = 100
            });

            _context.Seats.Add(new Seat
            {
                SeatId = 1,
                HallId = 1,
                SeatNumber = "A1",
                IsVip = false
            });

            _context.Showtimes.Add(new Showtime
            {
                ShowtimeId = 1,
                MovieId = 1,
                HallId = 1,
                ShowDate = DateTime.Today.AddDays(1),
                ShowTime1 = new TimeSpan(18, 0, 0),
                CreatedAt = DateTime.Now
            });

            _context.Reservations.Add(new Reservation
            {
                ReservationId = 1,
                UserId = 1,
                ShowtimeId = 1,
                ReservationDate = DateTime.Now,
                Status = "confirmed"
            });

            _context.Reservedseats.Add(new Reservedseat
            {
                ReservedSeatId = 1,
                ReservationId = 1,
                SeatId = 1
            });

            _context.Tickets.Add(new Ticket
            {
                TicketId = 1,
                ReservationId = 1,
                TicketCode = "TESZT-KOD-001",
                IssuedDate = DateTime.Now,
                IsUsed = false
            });

            _context.Tickets.Add(new Ticket
            {
                TicketId = 2,
                ReservationId = 1,
                TicketCode = "HASZNALT-KOD-002",
                IssuedDate = DateTime.Now,
                IsUsed = true
            });

            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            _controller = new TicketController(_context);
        }

        [TestMethod]
        public void ValidateTicket_ErvenyesKod_ReturnsOkEsMarkolja()
        {
            var result = _controller.ValidateTicket("TESZT-KOD-001");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            _context.ChangeTracker.Clear();
            var ticket = _context.Tickets.First(t => t.TicketCode == "TESZT-KOD-001");
            Assert.IsTrue(ticket.IsUsed);
        }

        [TestMethod]
        public void ValidateTicket_MarHasznaltKod_ReturnsBadRequest()
        {
            var result = _controller.ValidateTicket("HASZNALT-KOD-002");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void ValidateTicket_NemLetezoKod_ReturnsNotFound()
        {
            var result = _controller.ValidateTicket("NEM-LETEZO-KOD-999");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
    }
}