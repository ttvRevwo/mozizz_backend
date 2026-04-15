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
    }
}
