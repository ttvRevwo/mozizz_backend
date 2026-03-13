using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuffetController : ControllerBase
    {
        private readonly MozizzContext _context;
        private readonly Cloudinary _cloudinary;


        public BuffetController(MozizzContext context, IConfiguration config)
        {
            _context = context;
            var account = new Account(
                config["CloudinarySettings:CloudName"],
                config["CloudinarySettings:ApiKey"],
                config["CloudinarySettings:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("Items")]
        public async Task<IActionResult> GetItems()
        {
            var items = await _context.BuffetItems
                .Where(i => i.IsAvailable)
                .OrderBy(i => i.Category).ThenBy(i => i.Name)
                .Select(i => new { i.ItemId, i.Name, i.Description, i.Price, i.Category, i.Img })
                .ToListAsync();
            return Ok(items);
        }


    }
}
