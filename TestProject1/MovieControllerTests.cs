using System;
using System.Collections.Generic;
using MozizzAPI.Controllers;
using MozizzAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    [TestClass]
    public class MovieControllerTests
    {
        MozizzContext _context = null!;
        MovieController _controller = null!;
    
    private MozizzContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MozizzContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new MozizzContext(options);
        }

        private IConfiguration CreateFakeConfig()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "CloudinarySettings:CloudName", "fake_cloud" },
                    { "CloudinarySettings:ApiKey",    "123456789" },
                    { "CloudinarySettings:ApiSecret", "fake_secret" }
                })
                .Build();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = CreateInMemoryContext();

            _context.Movies.AddRange(
                new Movie
                {
                    MovieId = 1,
                    Title = "Inception",
                    Genre = "Sci-Fi",
                    Duration = 148,
                    Rating = "PG-13",
                    CreatedAt = DateTime.Now
                },
                new Movie
                {
                    MovieId = 2,
                    Title = "Titanic",
                    Genre = "Drama",
                    Duration = 194,
                    Rating = "PG-13",
                    CreatedAt = DateTime.Now
                }
            );
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            _controller = new MovieController(_context, CreateFakeConfig());
        }


        [TestMethod]
        public void GetAllMovies_ReturnsOkWithList()
        {
            var result = _controller.GetAllMovies();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            var filmek = okResult.Value as List<Movie>;
            Assert.IsNotNull(filmek);
            Assert.AreEqual(2, filmek.Count);
        }

        [TestMethod]
        public void GetAllMovies_UresAdatbazis_ReturnsUresLista()
        {
            _context.Movies.RemoveRange(_context.Movies);
            _context.SaveChanges();

            var result = _controller.GetAllMovies();

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var filmek = ((OkObjectResult)result).Value as List<Movie>;
            Assert.IsNotNull(filmek);
            Assert.AreEqual(0, filmek.Count);
        }



        [TestMethod]
        public void GetMovieById_LetezoId_ReturnsOk()
        {
            var result = _controller.GetMovieById(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var film = ((OkObjectResult)result).Value as Movie;
            Assert.IsNotNull(film);
            Assert.AreEqual("Inception", film.Title);
            Assert.AreEqual("Sci-Fi", film.Genre);
        }

        [TestMethod]
        public void GetMovieById_NemLetezoId_ReturnsNotFound()
        {
            var result = _controller.GetMovieById(999);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFound = (NotFoundObjectResult)result;
            Assert.AreEqual("A kért film nem található.", notFound.Value);
        }
    }
}
