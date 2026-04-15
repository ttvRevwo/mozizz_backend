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
    public class ShowtimeControllerTests
    {
        MozizzContext _context = null!;
        ShowtimeController _controller = null!;
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

            _context.Movies.Add(new Movie
            {
                MovieId = 1,
                Title = "Inception",
                Genre = "Sci-Fi",
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
            _context.Showtimes.AddRange(
                new Showtime
                {
                    ShowtimeId = 1,
                    MovieId = 1,
                    HallId = 1,
                    ShowDate = DateTime.Today.AddDays(1),
                    ShowTime1 = new TimeSpan(18, 0, 0),
                    CreatedAt = DateTime.Now
                },
                new Showtime
                {
                    ShowtimeId = 2,
                    MovieId = 1,
                    HallId = 1,
                    ShowDate = DateTime.Today.AddDays(2),
                    ShowTime1 = new TimeSpan(20, 30, 0),
                    CreatedAt = DateTime.Now
                }
            );
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            _controller = new ShowtimeController(_context);
        }
        [TestMethod]
        public void GetAllShowtimes_ReturnsOkWithList()
        {
            var result = _controller.GetAllShowtimes();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.IsNotNull(okResult.Value);
        }

        [TestMethod]
        public void GetByMovie_LetezoMovieId_ReturnsOk()
        {
            var result = _controller.GetByMovie(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetByMovie_NemLetezoMovieId_ReturnsUresLista()
        {
            var result = _controller.GetByMovie(999);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetById_LetezoId_ReturnsOk()
        {
            var result = _controller.GetById(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetById_NemLetezoId_ReturnsNotFound()
        {
            var result = _controller.GetById(999);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFound = (NotFoundObjectResult)result;
            Assert.AreEqual("A vetítés nem található.", notFound.Value);
        }
    }
}
