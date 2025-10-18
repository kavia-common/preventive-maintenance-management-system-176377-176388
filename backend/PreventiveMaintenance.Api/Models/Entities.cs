using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PreventiveMaintenance.Api.Models
{
    public class User
    {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Role { get; set; } = "technician";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Asset
    {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Code { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<MaintenanceTask> Tasks { get; set; } = new List<MaintenanceTask>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }

    public class MaintenanceTask
    {
        [Key] public int Id { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        [ForeignKey(nameof(Asset))] public int AssetId { get; set; }
        public Asset? Asset { get; set; }

        public int? AssignedTo { get; set; }
        public User? AssignedToUser { get; set; }

        [Required] public string Status { get; set; } = "pending";
        [Required] public string Priority { get; set; } = "medium";
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TaskHistory> History { get; set; } = new List<TaskHistory>();
    }

    public class Schedule
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(Asset))] public int AssetId { get; set; }
        public Asset? Asset { get; set; }
        [Required] public string Frequency { get; set; } = "monthly";
        [Required] public DateTime NextRun { get; set; } = DateTime.UtcNow.AddDays(30);
        public DateTime? LastRun { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class TaskHistory
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(Task))] public int TaskId { get; set; }
        public MaintenanceTask? Task { get; set; }
        [Required] public string Action { get; set; } = "created";
        public string? Details { get; set; }
        public int? PerformedBy { get; set; }
        public User? PerformedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
