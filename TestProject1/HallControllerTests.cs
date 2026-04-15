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


        [TestMethod]
        public void NewHall_ErvenyesAdat_ReturnsOkEsElmenti()
        {
            var ujTerem = new Hall { Name = "Terem 3", Location = "Pince", SeatingCapacity = 30 };

            var result = _controller.NewHall(ujTerem);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual("Sikeres létrehozás!", okResult.Value);
            Assert.AreEqual(3, _context.Halls.Count());
        }

        [TestMethod]
        public void ModifyHall_LetezoTerem_ReturnsOkEsModosit()
        {
            var modositott = new Hall { HallId = 1, Name = "Módosított Terem", Location = "Emelet", SeatingCapacity = 120 };

            var result = _controller.ModifyHall(modositott);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual("Sikeres módosítás!", okResult.Value);

            var updatedHall = _context.Halls.Find(1);
            Assert.IsNotNull(updatedHall);
            Assert.AreEqual("Módosított Terem", updatedHall.Name);
            Assert.AreEqual(120, updatedHall.SeatingCapacity);
        }

        [TestMethod]
        public void ModifyHall_NemLetezoTerem_ReturnsNotFound()
        {
            var nemLetezo = new Hall { HallId = 999, Name = "Nem létező", Location = "Sehol", SeatingCapacity = 0 };

            var result = _controller.ModifyHall(nemLetezo);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFound = (NotFoundObjectResult)result;
            Assert.AreEqual("Nincs ilyen terem!", notFound.Value);
        }

        [TestMethod]
        public void DeleteHall_LetezoId_ReturnsOkEsTorli()
        {
            var result = _controller.DelethallById(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual("Sikeres törlés!", okResult.Value);
            Assert.IsNull(_context.Halls.Find(1));
            Assert.AreEqual(1, _context.Halls.Count());
        }

        [TestMethod]
        public void DeleteHall_NemLetezoId_ReturnsNotFound()
        {
            var result = _controller.DelethallById(999);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFound = (NotFoundObjectResult)result;
            Assert.AreEqual("Nincs ilyen terem!", notFound.Value);
        }

    }
}
