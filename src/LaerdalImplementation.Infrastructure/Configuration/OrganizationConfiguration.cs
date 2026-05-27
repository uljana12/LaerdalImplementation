using LaerdalImplementation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaerdalImplementation.Infrastructure.Configuration;

/// <summary>
/// Configures the EF Core mapping for <see cref="Organization"/> to the
/// <c>Organizations</c> table. Keeping this configuration in the Infrastructure layer
/// (rather than as data annotations on the entity) means the Domain layer has zero
/// dependency on EF Core.
/// </summary>
public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    /// <summary>
    /// Applies column types, constraints, indexes, and relationships for the
    /// <c>Organizations</c> table.
    /// </summary>
    /// <param name="builder">Fluent builder for the Organization entity type.</param>
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        builder.HasKey(o => o.Id);

        // ValueGeneratedNever: the GUID is created in C# (Organization.Create),
        // so EF must not ask the database to generate or override it.
        builder.Property(o => o.Id).ValueGeneratedNever();
        builder.Property(o => o.Name).IsRequired().HasMaxLength(255);
        builder.Property(o => o.Code).IsRequired().HasMaxLength(50);
        builder.Property(o => o.Type).IsRequired();
        builder.Property(o => o.ExternalId).HasMaxLength(255);
        builder.Property(o => o.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt).IsRequired();

        // Unique constraint: Code + ParentId enforces the business rule that codes
        // must be unique within the same parent scope at the database level.
        // Note: SQL Server treats NULL != NULL in unique indexes, so two root orgs
        // (ParentId = NULL) with the same code won't be caught by this constraint —
        // the application-level check in CreateOrganizationCommandHandler covers that gap.
        builder.HasIndex(o => new { o.Code, o.ParentId })
            .IsUnique()
            .HasDatabaseName("UQ_Code_ParentId");

        // Self-referencing FK with Restrict delete: SQL Server will block deleting
        // a parent org that still has children, matching the CanBeDeleted() domain rule.
        builder.HasOne(o => o.Parent)
            .WithMany(o => o.Children)
            .HasForeignKey(o => o.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cascade deletes manifests when an org is removed.
        // In practice, soft-delete (IsActive = false) is used first,
        // so hard deletes are rare.
        builder.HasMany(o => o.Manifests)
            .WithOne(m => m.Organization)
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
