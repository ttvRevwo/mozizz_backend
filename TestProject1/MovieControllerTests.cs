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
    }
}
