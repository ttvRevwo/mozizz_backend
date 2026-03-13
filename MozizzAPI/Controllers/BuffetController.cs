using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MozizzAPI.DTOS;
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

        [HttpGet("AllItems")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _context.BuffetItems
                .OrderBy(i => i.Category).ThenBy(i => i.Name)
                .Select(i => new { i.ItemId, i.Name, i.Description, i.Price, i.Category, i.Img, i.IsAvailable })
                .ToListAsync();
            return Ok(items);
        }

        [HttpPost("NewItem")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> NewItem([FromForm] BuffetItemDto dto, IFormFile? imageFile)
        {
            var item = new BuffetItem
            {
                Name = dto.Name!,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category ?? "snack",
                IsAvailable = dto.IsAvailable ?? true
            };

            if (imageFile != null)
            {
                using var stream = imageFile.OpenReadStream();
                var safeName = dto.Name?.Replace(" ", "_").ToLower() ?? "item";
                var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                {
                    File = new FileDescription(imageFile.FileName, stream),
                    Folder = "buffet",
                    PublicId = $"{safeName}_{DateTime.Now.Ticks}",
                    Overwrite = true
                });
                item.Img = uploadResult.PublicId + "." + uploadResult.Format;
            }

            _context.BuffetItems.Add(item);
            await _context.SaveChangesAsync();
            return Ok(new { item.ItemId, uzenet = "Termék hozzáadva!" });
        }

    }
}
