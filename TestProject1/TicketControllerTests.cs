using System;
using System.Linq;
using MozizzAPI.Controllers;
using MozizzAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    [TestClass]
    public class TicketControllerTests
    {
        MozizzContext _context = null!;
        TicketController _controller = null!;
    }
}
