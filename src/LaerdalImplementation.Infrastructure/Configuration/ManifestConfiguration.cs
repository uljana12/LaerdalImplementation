using LaerdalImplementation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaerdalImplementation.Infrastructure.Configuration;

/// <summary>
/// Configures the EF Core mapping for <see cref="Manifest"/> to the
/// <c>Manifests</c> table. Defined in Infrastructure to keep the Domain entity
/// free of any EF Core dependencies.
/// </summary>
public class ManifestConfiguration : IEntityTypeConfiguration<Manifest>
{
    /// <summary>
    /// Applies column types, constraints, indexes, and relationships for the
    /// <c>Manifests</c> table.
    /// </summary>
    /// <param name="builder">Fluent builder for the Manifest entity type.</param>
    public void Configure(EntityTypeBuilder<Manifest> builder)
    {
        builder.ToTable("Manifests");

        builder.HasKey(m => m.Id);

        // GUID is created in C# (Manifest.CreateDraft), not by the database.
        builder.Property(m => m.Id).ValueGeneratedNever();
        builder.Property(m => m.OrganizationId).IsRequired();
        builder.Property(m => m.Name).IsRequired().HasMaxLength(255);
        builder.Property(m => m.Description).HasMaxLength(1000);
        builder.Property(m => m.Version).IsRequired().HasMaxLength(20);
        builder.Property(m => m.Status).IsRequired();

        // Content is a JSON blob (nvarchar(max)). Stored as-is; no column type override needed.
        builder.Property(m => m.Content).IsRequired();
        builder.Property(m => m.PublishedAt);
        builder.Property(m => m.CreatedAt).IsRequired();
        builder.Property(m => m.UpdatedAt).IsRequired();

        // Unique constraint: prevents publishing the same version number twice for
        // the same organization (e.g., cannot have two "1.0.0" manifests under org X).
        builder.HasIndex(m => new { m.OrganizationId, m.Version })
            .IsUnique()
            .HasDatabaseName("UQ_Manifest_OrgVersion");

        // Composite index on (OrganizationId, Status) because the most frequent query
        // is "give me the Published manifest for org X" — used by the training app on
        // every learner session start. Without this index that query would do a full table scan.
        builder.HasIndex(m => new { m.OrganizationId, m.Status })
            .HasDatabaseName("IX_Manifest_OrgStatus");

        // FK back to Organization; handled via OrganizationConfiguration's HasMany,
        // but declared here for clarity.
        builder.HasOne(m => m.Organization)
            .WithMany(o => o.Manifests)
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
