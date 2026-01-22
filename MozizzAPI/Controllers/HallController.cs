using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MozizzAPI.Models;

namespace MozizzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallController : ControllerBase
    {
        private readonly MozizzContext _context;

        public HallController(MozizzContext context)
        {
            _context = context;
        }


        [HttpGet("GetAllHall")]
        public IActionResult GetAllHall()
        {
            try
            {
                var termek = _context.Halls.ToList();
                return Ok(termek);
            }
            catch (Exception ex)
            {
                return BadRequest(new { üzenet = "Hiba a lekérdezés során", hiba = ex.Message });
            }

        }
    }
}
