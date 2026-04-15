using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozizzAPI.Controllers;
using MozizzAPI.Models;
using MozizzAPI.DTOS;

namespace TestProject1
{
    [TestClass]
    public class UserProfileControllerTests
    {
        private MozizzContext _context = null!;
        private IConfiguration _config = null!;
        private UserProfileController _controller = null!;

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

            _context.Users.Add(new User
            {
                UserId = 1,
                Name = "Eredeti Név",
                Email = "teszt@teszt.hu",
                Phone = "+36301234567",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("regiJelszo123"),
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            _controller = new UserProfileController(_context, _config);
        }
    }
}
