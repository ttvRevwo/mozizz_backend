using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozizzAPI.Controllers;
using MozizzAPI.Models;

namespace TestProject1
{
    [TestClass]
    public class UserControllerTests
    {
        private MozizzContext _context = null!;
        private UserController _controller = null!;
        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MozizzContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new MozizzContext(options);

            _context.Users.AddRange(
                new User { UserId = 1, Name = "Admin User", Email = "admin@teszt.hu", PasswordHash = "kamu_hash_1" },
                new User { UserId = 2, Name = "Sima User", Email = "user@teszt.hu", PasswordHash = "kamu_hash_2" }
            );

            _context.UserVerifications.AddRange(
             new UserVerification { Id = 1, Email = "admin@teszt.hu", Code = "111111", ExpiresAt = DateTime.Now.AddMinutes(-10) },
             new UserVerification { Id = 2, Email = "user@teszt.hu", Code = "222222", ExpiresAt = DateTime.Now.AddMinutes(10) }
         );

            _context.SaveChanges();

            _controller = new UserController(_context);
        }

        [TestMethod]
        public void GetAllUsers_VisszaadjaAzOsszesFelhasznalot()
        {
            var result = _controller.GetAllUsers();

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            var users = okResult.Value as List<User>;

            Assert.IsNotNull(users);
            Assert.AreEqual(2, users.Count);
        }

        [TestMethod]
        public void GetUserById_LetezoUser_OkEredmenytAd()
        {
            var result = _controller.GetUserById(1);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            var user = okResult.Value as User;

            Assert.IsNotNull(user);
            Assert.AreEqual("Admin User", user.Name);
        }

        [TestMethod]
        public void GetUserById_NemLetezoUser_NotFoundotAd()
        {
           var result = _controller.GetUserById(99);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void DeleteUser_LetezoUser_TorliAzAdatbazisbol()
        {
            var result = _controller.DeleteUser(2);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(1, _context.Users.Count());
        }

        [TestMethod]
        public void CleanupCodes_CsakALejartKodokatTorli()
        {
            var result = _controller.CleanupCodes();

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(1, _context.UserVerifications.Count());

            var megmaradtKod = _context.UserVerifications.First();
            Assert.AreEqual(2, megmaradtKod.Id);
        }
    }
}
