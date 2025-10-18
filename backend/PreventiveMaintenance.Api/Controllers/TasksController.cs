using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PreventiveMaintenance.Api.Data;
using PreventiveMaintenance.Api.Models;

namespace PreventiveMaintenance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TasksController(AppDbContext db) => _db = db;

        // PUBLIC_INTERFACE
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] string? status = null, [FromQuery] int? assetId = null)
        {
            var query = _db.MaintenanceTasks.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(t => t.Status == status);
            if (assetId.HasValue)
                query = query.Where(t => t.AssetId == assetId.Value);

            var list = await query
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => t.ToDto())
                .ToListAsync();

            return Ok(list);
        }

        // PUBLIC_INTERFACE
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TaskDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _db.MaintenanceTasks.FindAsync(id);
            if (t == null) return NotFound();
            return Ok(t.ToDto());
        }

        // PUBLIC_INTERFACE
        [HttpPost]
        [ProducesResponseType(typeof(TaskDto), 201)]
        public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
        {
            var t = new MaintenanceTask
            {
                Title = dto.Title,
                Description = dto.Description,
                AssetId = dto.AssetId,
                AssignedTo = dto.AssignedTo,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "pending" : dto.Status,
                Priority = string.IsNullOrWhiteSpace(dto.Priority) ? "medium" : dto.Priority,
                DueDate = dto.DueDate,
                CreatedAt = DateTime.UtcNow
            };
            _db.MaintenanceTasks.Add(t);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = t.Id }, t.ToDto());
        }

        // PUBLIC_INTERFACE
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TaskDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] TaskCreateDto dto)
        {
            var t = await _db.MaintenanceTasks.FindAsync(id);
            if (t == null) return NotFound();
            t.Title = dto.Title;
            t.Description = dto.Description;
            t.AssetId = dto.AssetId;
            t.AssignedTo = dto.AssignedTo;
            t.Status = dto.Status;
            t.Priority = dto.Priority;
            t.DueDate = dto.DueDate;
            await _db.SaveChangesAsync();
            return Ok(t.ToDto());
        }

        // PUBLIC_INTERFACE
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _db.MaintenanceTasks.FindAsync(id);
            if (t == null) return NotFound();
            _db.MaintenanceTasks.Remove(t);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
