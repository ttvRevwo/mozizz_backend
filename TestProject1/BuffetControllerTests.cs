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

namespace TestProject1
{
    [TestClass]
    public class BuffetControllerTests
    {
        private MozizzContext _context = null!;
        private BuffetController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MozizzContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new MozizzContext(options);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "CloudinarySettings:CloudName", "kamu_cloud" },
                    { "CloudinarySettings:ApiKey", "12345" },
                    { "CloudinarySettings:ApiSecret", "titkos_secret" }
                }).Build();

            _context.BuffetItems.AddRange(
                new BuffetItem { ItemId = 1, Name = "Sós Popcorn", Price = 1500, IsAvailable = true, Category = "Snack" },
                new BuffetItem { ItemId = 2, Name = "Elfogyott Kóla", Price = 800, IsAvailable = false, Category = "Ital" }
            );
            _context.SaveChanges();

            _controller = new BuffetController(_context, config);
        }
    }
}
