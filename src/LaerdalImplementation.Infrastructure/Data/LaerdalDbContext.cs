using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace LaerdalImplementation.Infrastructure.Data;

/// <summary>
/// The EF Core database context for the Laerdal Implementer.
/// Acts as the single "window" through which all database reads and writes flow.
/// <para>
/// Each HTTP request receives its own scoped instance of this context (registered
/// via <c>AddDbContext</c> in <c>Program.cs</c>), so changes are isolated per request
/// and rolled back automatically if the request fails before <c>SaveChangesAsync</c>.
/// </para>
/// </summary>
public class LaerdalDbContext : DbContext
{
    /// <summary>Provides access to the <c>Organizations</c> table.</summary>
    public DbSet<Organization> Organizations { get; set; } = null!;

    /// <summary>Provides access to the <c>Manifests</c> table.</summary>
    public DbSet<Manifest> Manifests { get; set; } = null!;

    /// <summary>
    /// Initializes the context with the options (connection string, provider, etc.)
    /// configured via <c>AddDbContext</c> in the DI container.
    /// </summary>
    /// <param name="options">EF Core options, including the SQL Server connection string.</param>
    public LaerdalDbContext(DbContextOptions<LaerdalDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Applies fluent configuration for each entity type. Separating configuration
    /// into dedicated classes (rather than using data annotations on the entities)
    /// keeps the Domain layer free of any EF Core dependencies.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the EF model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
        modelBuilder.ApplyConfiguration(new ManifestConfiguration());
    }
}
