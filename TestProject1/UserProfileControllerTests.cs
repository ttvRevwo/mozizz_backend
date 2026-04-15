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
     
    }
}
