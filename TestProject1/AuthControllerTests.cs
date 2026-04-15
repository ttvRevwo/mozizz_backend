using System;
using System.Collections.Generic;
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
    public class AuthControllerTests
    {
        private MozizzContext _context = null!;
        private AuthController _controller = null!;

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
                    { "Jwt:Key", "nagyon_hosszu_titkos_kulcs_ami_legalabb_32_karakter" },
                    { "Jwt:Issuer", "TestIssuer" },
                    { "Jwt:Audience", "TestAudience" }
                }).Build();

            var user = new User
            {
                UserId = 1,
                Name = "Teszt Elek",
                Email = "teszt@teszt.hu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("titkosJelszo123"),
                Role = new Userrole { RoleId = 1, RoleName = "User" }
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            _controller = new AuthController(_context, config);
        }
    }
}
