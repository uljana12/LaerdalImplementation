using LaerdalImplementation.Application.Commands;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Enums;
using LaerdalImplementation.Domain.Repositories;
using LaerdalImplementation.Infrastructure.Data;
using LaerdalImplementation.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------------
// Services
// --------------------------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Port — Railway (and most cloud platforms) set a PORT env var.
// Fall back to 5000 for local development.
var port = Environment.GetEnvironmentVariable("PORT") ?? "5050";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Database — SQLite, path relative to working directory.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=laerdal.db";

builder.Services.AddDbContext<LaerdalDbContext>(options =>
    options.UseSqlite(connectionString));

// MediatR — scans the Application assembly and auto-registers all IRequestHandler
// implementations, so commands/queries are wired up without manual registration.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(CreateOrganizationCommand).Assembly));

// Repositories — bind the Domain interfaces to their Infrastructure implementations.
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IManifestRepository, ManifestRepository>();

// CORS — allow the React frontend from any origin so Vercel, local dev,
// and any future host all work without reconfiguration.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// --------------------------------------------------------------------------
// Middleware pipeline
// --------------------------------------------------------------------------

var app = builder.Build();

// Migrate and seed on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LaerdalDbContext>();
    db.Database.Migrate();
    await SeedDemoDataAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

// --------------------------------------------------------------------------
// Demo seed — creates a realistic org hierarchy if the database is empty,
// so the hosted demo always has something to show without manual setup.
// --------------------------------------------------------------------------

static async Task SeedDemoDataAsync(LaerdalDbContext db)
{
    if (await db.Organizations.AnyAsync()) return;

    var now = DateTime.UtcNow;

    var cuh    = new Organization { Id = Guid.NewGuid(), Name = "Copenhagen University Hospital", Code = "CUH",  Type = OrganizationType.Hospital,       IsActive = true, CreatedAt = now, UpdatedAt = now };
    var righ   = new Organization { Id = Guid.NewGuid(), Name = "Rigshospitalet",                  Code = "RIG",  Type = OrganizationType.Hospital,       IsActive = true, CreatedAt = now, UpdatedAt = now };

    var card   = new Organization { Id = Guid.NewGuid(), Name = "Cardiology",                      Code = "CARD", Type = OrganizationType.Department,     IsActive = true, ParentId = cuh.Id,  CreatedAt = now, UpdatedAt = now };
    var ed     = new Organization { Id = Guid.NewGuid(), Name = "Emergency Department",             Code = "ED",   Type = OrganizationType.Department,     IsActive = true, ParentId = cuh.Id,  CreatedAt = now, UpdatedAt = now };
    var icu    = new Organization { Id = Guid.NewGuid(), Name = "Intensive Care Unit",              Code = "ICU",  Type = OrganizationType.Department,     IsActive = true, ParentId = cuh.Id,  CreatedAt = now, UpdatedAt = now };
    var tc     = new Organization { Id = Guid.NewGuid(), Name = "CPR Training Center",              Code = "CPRTC",Type = OrganizationType.TrainingCenter, IsActive = true, ParentId = cuh.Id,  CreatedAt = now, UpdatedAt = now };

    var neuro  = new Organization { Id = Guid.NewGuid(), Name = "Neurology",                       Code = "NEUR", Type = OrganizationType.Department,     IsActive = true, ParentId = righ.Id, CreatedAt = now, UpdatedAt = now };
    var onco   = new Organization { Id = Guid.NewGuid(), Name = "Oncology",                        Code = "ONCO", Type = OrganizationType.Department,     IsActive = true, ParentId = righ.Id, CreatedAt = now, UpdatedAt = now };

    db.Organizations.AddRange(cuh, righ, card, ed, icu, tc, neuro, onco);
    await db.SaveChangesAsync();
}
