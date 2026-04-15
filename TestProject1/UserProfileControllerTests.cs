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
        [TestMethod]
        public async Task GetProfile_LetezoUser_VisszaadjaAProfilt()
        {
            var result = await _controller.GetProfile(1);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetProfile_NemLetezoUser_NotFoundHibado()
        {
            var result = await _controller.GetProfile(99);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task UpdateProfile_LetezoUser_FrissitiAzAdatokat()
        {
            var frissitettAdatok = new User { Name = "Új Név", Phone = "+36209999999" };

            var result = await _controller.UpdateProfile(1, frissitettAdatok);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var user = await _context.Users.FindAsync(1);
            Assert.IsNotNull(user);
            Assert.AreEqual("Új Név", user.Name);
            Assert.AreEqual("+36209999999", user.Phone);
        }

        [TestMethod]
        public async Task ChangePassword_HelyesRegiJelszo_SikeresModositas()
        {
            var jelszoDto = new PasswordChangeDto
            {
                OldPassword = "regiJelszo123",
                NewPassword = "UjTitkosJelszo456"
            };

            var result = await _controller.ChangePassword(1, jelszoDto);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var user = await _context.Users.FindAsync(1);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("UjTitkosJelszo456", user.PasswordHash));
        }

        [TestMethod]
        public async Task ChangePassword_RosszRegiJelszo_BadRequestetAd()
        {
            var jelszoDto = new PasswordChangeDto
            {
                OldPassword = "hibas_regi_jelszo_probalkozas",
                NewPassword = "UjTitkosJelszo456"
            };

            var result = await _controller.ChangePassword(1, jelszoDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}
