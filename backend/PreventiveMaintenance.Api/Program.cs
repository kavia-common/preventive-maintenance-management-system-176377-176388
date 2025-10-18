using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PreventiveMaintenance.Api.Data;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" })
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

// Ensure SQLite database file exists and apply schema if new
var dbPath = connStr?.Replace("Data Source=", "") ?? "maintenance.db";
var dbDir = Path.GetDirectoryName(dbPath);
if (!string.IsNullOrWhiteSpace(dbDir) && !Directory.Exists(dbDir))
{
    Directory.CreateDirectory(dbDir);
}

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(connStr);
});

var app = builder.Build();

// Apply schema from SQL if database is empty
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Ensure the database file exists
    db.Database.EnsureCreated();

    // Create tables by executing schema.sql if main tables missing
    var hasUsers = false;
    try
    {
        hasUsers = db.Database.ExecuteSqlRaw("SELECT 1 FROM sqlite_master WHERE type='table' AND name='users';") == 1;
    }
    catch { /* ignore */ }

    if (!hasUsers)
    {
        try
        {
            var baseDir = AppContext.BaseDirectory;
            var schemaPath = Path.GetFullPath(Path.Combine(baseDir, "../../../../../preventive-maintenance-management-system-176377-176386/maintenance_database/schema.sql"));
            if (File.Exists(schemaPath))
            {
                var sql = File.ReadAllText(schemaPath);
                foreach (var stmt in sql.Split(';'))
                {
                    var s = stmt.Trim();
                    if (!string.IsNullOrEmpty(s))
                    {
                        db.Database.ExecuteSqlRaw(s);
                    }
                }
            }

            var seedPath = Path.GetFullPath(Path.Combine(baseDir, "../../../../../preventive-maintenance-management-system-176377-176386/maintenance_database/seed.sql"));
            if (File.Exists(seedPath))
            {
                var sql = File.ReadAllText(seedPath);
                foreach (var stmt in sql.Split(';'))
                {
                    var s = stmt.Trim();
                    if (!string.IsNullOrEmpty(s))
                    {
                        db.Database.ExecuteSqlRaw(s);
                    }
                }
            }
        }
        catch
        {
            // ignore bootstrapping errors to avoid breaking startup in preview
        }
    }
}

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");
app.MapControllers();

// Welcome and health endpoints
app.MapGet("/", () => Results.Ok(new { message = "Preventive Maintenance API - Welcome" }))
    .WithTags("Health")
    .WithName("RootWelcome");

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithTags("Health")
    .WithName("HealthCheck");

app.Run();
