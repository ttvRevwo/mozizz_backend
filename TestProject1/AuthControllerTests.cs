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
    }
}
