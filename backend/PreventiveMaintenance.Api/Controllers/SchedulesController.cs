using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PreventiveMaintenance.Api.Data;
using PreventiveMaintenance.Api.Models;

namespace PreventiveMaintenance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SchedulesController(AppDbContext db) => _db = db;

        // PUBLIC_INTERFACE
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ScheduleDto>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] int? assetId = null)
        {
            var query = _db.Schedules.AsQueryable();
            if (assetId.HasValue)
                query = query.Where(s => s.AssetId == assetId.Value);

            var list = await query
                .OrderBy(s => s.NextRun)
                .Select(s => s.ToDto())
                .ToListAsync();

            return Ok(list);
        }

        // PUBLIC_INTERFACE
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ScheduleDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _db.Schedules.FindAsync(id);
            if (s == null) return NotFound();
            return Ok(s.ToDto());
        }

        // PUBLIC_INTERFACE
        [HttpPost]
        [ProducesResponseType(typeof(ScheduleDto), 201)]
        public async Task<IActionResult> Create([FromBody] ScheduleCreateDto dto)
        {
            var s = new Schedule
            {
                AssetId = dto.AssetId,
                Frequency = dto.Frequency,
                NextRun = dto.NextRun,
                LastRun = dto.LastRun,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };
            _db.Schedules.Add(s);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = s.Id }, s.ToDto());
        }

        // PUBLIC_INTERFACE
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ScheduleDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] ScheduleCreateDto dto)
        {
            var s = await _db.Schedules.FindAsync(id);
            if (s == null) return NotFound();
            s.AssetId = dto.AssetId;
            s.Frequency = dto.Frequency;
            s.NextRun = dto.NextRun;
            s.LastRun = dto.LastRun;
            s.Notes = dto.Notes;
            await _db.SaveChangesAsync();
            return Ok(s.ToDto());
        }

        // PUBLIC_INTERFACE
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _db.Schedules.FindAsync(id);
            if (s == null) return NotFound();
            _db.Schedules.Remove(s);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
