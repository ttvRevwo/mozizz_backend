using System;
using System.Collections.Generic;
using System.Linq;
using MozizzAPI.Controllers;
using MozizzAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    [TestClass]
    public class HallControllerTests
    {
        MozizzContext _context = null!;
        HallController _controller = null!;

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

            _context.Halls.AddRange(
                new Hall { HallId = 1, Name = "Terem 1", Location = "Fszt.", SeatingCapacity = 100 },
                new Hall { HallId = 2, Name = "Terem 2", Location = "Emelet", SeatingCapacity = 50 }
            );
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            _controller = new HallController(_context);
        }

        [TestMethod]
        public void GetAllHall_ReturnsOkWithList()
        {
            var result = _controller.GetAllHall();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            var halls = okResult.Value as List<Hall>;
            Assert.IsNotNull(halls);
            Assert.AreEqual(2, halls.Count);
        }

        [TestMethod]
        public void GetHallById_LetezoId_ReturnsOk()
        {
            var result = _controller.GetHallById(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            var hall = okResult.Value as Hall;
            Assert.IsNotNull(hall);
            Assert.AreEqual("Terem 1", hall.Name);
            Assert.AreEqual(100, hall.SeatingCapacity);
        }


    }
}
