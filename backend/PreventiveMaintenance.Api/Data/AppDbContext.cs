using Microsoft.EntityFrameworkCore;
using PreventiveMaintenance.Api.Models;

namespace PreventiveMaintenance.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users => Set<User>();
        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<MaintenanceTask> MaintenanceTasks => Set<MaintenanceTask>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<TaskHistory> TaskHistory => Set<TaskHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.Code)
                .IsUnique();

            modelBuilder.Entity<MaintenanceTask>()
                .HasOne(t => t.Asset)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaintenanceTask>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TaskHistory>()
                .HasOne(h => h.Task)
                .WithMany(t => t.History)
                .HasForeignKey(h => h.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskHistory>()
                .HasOne(h => h.PerformedByUser)
                .WithMany()
                .HasForeignKey(h => h.PerformedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Asset)
                .WithMany(a => a.Schedules)
                .HasForeignKey(s => s.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
