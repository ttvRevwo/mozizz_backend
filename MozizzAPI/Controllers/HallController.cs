using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MozizzAPI.Models;
using Org.BouncyCastle.Bcpg.OpenPgp;

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

        [HttpGet("HallById/{id}")]

        public IActionResult GetHallById(int id)
        {
            try
            {
                var terem = _context.Halls.FirstOrDefault(t => t.HallId == id);
                if(terem == null) return NotFound("Nincs ilyen terem!");
                return Ok(terem);
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteHall/{id}")]
        public IActionResult DelethallById(int id)
        {
            try
            {
                var terem = _context.Halls.FirstOrDefault(t=>t.HallId == id);
                if(terem == null) return NotFound("Nincs ilyen terem!");
                _context.Halls.Remove(terem);
                _context.SaveChanges();
                return Ok("Sikeres törlés!");

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ModifyHall")]
        public IActionResult ModifyHall(Hall hall)
        {
            try
            {
               var terem = _context.Halls.FirstOrDefault(t => t.HallId == hall.HallId);
                if(terem == null) return NotFound("Nincs ilyen terem!");
                terem.Name = hall.Name;
                terem.Location = hall.Location;
                terem.SeatingCapacity = hall.SeatingCapacity;
                _context.SaveChanges();
                return Ok("Sikeres módosítás!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
