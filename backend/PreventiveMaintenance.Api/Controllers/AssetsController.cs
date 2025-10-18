using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PreventiveMaintenance.Api.Data;
using PreventiveMaintenance.Api.Models;

namespace PreventiveMaintenance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AssetsController(AppDbContext db) => _db = db;

        // PUBLIC_INTERFACE
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AssetDto>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] string? q = null)
        {
            var query = _db.Assets.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(a => a.Name.Contains(q) || a.Code.Contains(q));
            }
            var list = await query.OrderBy(a => a.Name).Select(a => a.ToDto()).ToListAsync();
            return Ok(list);
        }

        // PUBLIC_INTERFACE
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AssetDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var a = await _db.Assets.FindAsync(id);
            if (a == null) return NotFound();
            return Ok(a.ToDto());
        }

        // PUBLIC_INTERFACE
        [HttpPost]
        [ProducesResponseType(typeof(AssetDto), 201)]
        public async Task<IActionResult> Create([FromBody] AssetCreateDto dto)
        {
            var a = new Asset
            {
                Name = dto.Name,
                Code = dto.Code,
                Location = dto.Location,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };
            _db.Assets.Add(a);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = a.Id }, a.ToDto());
        }

        // PUBLIC_INTERFACE
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(AssetDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] AssetCreateDto dto)
        {
            var a = await _db.Assets.FindAsync(id);
            if (a == null) return NotFound();
            a.Name = dto.Name;
            a.Code = dto.Code;
            a.Location = dto.Location;
            a.Description = dto.Description;
            await _db.SaveChangesAsync();
            return Ok(a.ToDto());
        }

        // PUBLIC_INTERFACE
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _db.Assets.FindAsync(id);
            if (a == null) return NotFound();
            _db.Assets.Remove(a);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
