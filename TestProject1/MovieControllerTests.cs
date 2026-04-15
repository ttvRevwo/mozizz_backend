using System;
using System.Collections.Generic;
using MozizzAPI.Controllers;
using MozizzAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    [TestClass]
    public class MovieControllerTests
    {
        MozizzContext _context = null!;
        MovieController _controller = null!;
    }
}
