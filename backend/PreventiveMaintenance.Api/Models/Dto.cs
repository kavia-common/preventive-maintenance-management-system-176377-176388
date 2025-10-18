namespace PreventiveMaintenance.Api.Models
{
    public record AssetDto(int Id, string Name, string Code, string? Location, string? Description);
    public record AssetCreateDto(string Name, string Code, string? Location, string? Description);

    public record TaskDto(int Id, string Title, string? Description, int AssetId, int? AssignedTo, string Status, string Priority, DateTime? DueDate);
    public record TaskCreateDto(string Title, string? Description, int AssetId, int? AssignedTo, string Status, string Priority, DateTime? DueDate);

    public record ScheduleDto(int Id, int AssetId, string Frequency, DateTime NextRun, DateTime? LastRun, string? Notes);
    public record ScheduleCreateDto(int AssetId, string Frequency, DateTime NextRun, DateTime? LastRun, string? Notes);

    public static class Mapping
    {
        public static AssetDto ToDto(this Asset a) => new(a.Id, a.Name, a.Code, a.Location, a.Description);
        public static TaskDto ToDto(this MaintenanceTask t) => new(t.Id, t.Title, t.Description, t.AssetId, t.AssignedTo, t.Status, t.Priority, t.DueDate);
        public static ScheduleDto ToDto(this Schedule s) => new(s.Id, s.AssetId, s.Frequency, s.NextRun, s.LastRun, s.Notes);
    }
}
