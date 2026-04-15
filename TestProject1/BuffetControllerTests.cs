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
    public class BuffetControllerTests
    {
        private MozizzContext _context = null!;
        private BuffetController _controller = null!;
    }
}
