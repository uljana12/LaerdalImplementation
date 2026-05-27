using System;
using System.Collections.Generic;
using System.Linq;
using LaerdalImplementation.Domain.Enums;

namespace LaerdalImplementation.Domain.Entities;

/// <summary>
/// Represents a hospital, department, or training centre that can be provisioned
/// and configured through the Laerdal Implementer. Organizations form a parent-child
/// hierarchy via <see cref="ParentId"/>; a null parent means a root-level organization.
/// </summary>
public class Organization
{
    /// <summary>Gets or sets the unique identifier for this organization.</summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the parent organization, or <c>null</c> if this is a root org.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>Gets or sets the display name of the organization.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the short identifier code. Unique within the same parent scope.
    /// Stored in uppercase.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Gets or sets whether this is a hospital, department, or training centre.</summary>
    public OrganizationType Type { get; set; }

    /// <summary>
    /// Gets or sets an optional reference to an identity in an external system
    /// (e.g., an OIDC directory or enterprise HR system).
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Gets or sets whether the organization is active. Inactive organizations are
    /// soft-deleted — they remain in the database to preserve audit trails and
    /// foreign key relationships with manifests.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the UTC timestamp when this organization was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the UTC timestamp of the most recent update.</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Navigation property to the parent organization, if one exists.</summary>
    public Organization? Parent { get; set; }

    /// <summary>Navigation property to all direct child organizations.</summary>
    public ICollection<Organization> Children { get; set; } = new List<Organization>();

    /// <summary>Navigation property to all manifests owned by this organization.</summary>
    public ICollection<Manifest> Manifests { get; set; } = new List<Manifest>();

    /// <summary>
    /// Creates a new, active organization after validating the required fields.
    /// Using a factory method rather than a public constructor ensures the entity
    /// is always constructed in a valid state.
    /// </summary>
    /// <param name="name">Display name — must not be blank.</param>
    /// <param name="code">Short identifier code — must not be blank; stored as uppercase.</param>
    /// <param name="type">The kind of organization.</param>
    /// <param name="parentId">ID of the parent org, or <c>null</c> for a root organization.</param>
    /// <param name="externalId">Optional reference to an external identity system.</param>
    /// <returns>A newly created, persisted-ready <see cref="Organization"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when name or code is empty.</exception>
    public static Organization Create(string name, string code, OrganizationType type, Guid? parentId = null, string? externalId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Organization code is required.", nameof(code));

        return new Organization
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Code = code.Trim().ToUpper(),
            Type = type,
            ParentId = parentId,
            ExternalId = externalId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates mutable fields on an existing organization and stamps <see cref="UpdatedAt"/>.
    /// </summary>
    /// <param name="name">New display name — must not be blank.</param>
    /// <param name="type">New organization type.</param>
    /// <param name="isActive">Whether the organization should remain active.</param>
    /// <exception cref="ArgumentException">Thrown when name is empty.</exception>
    public void Update(string name, OrganizationType type, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required.", nameof(name));

        Name = name.Trim();
        Type = type;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks whether this organization can be safely removed. A parent org cannot
    /// be deleted while it still has active children, because that would orphan them.
    /// </summary>
    /// <returns><c>true</c> if there are no active child organizations; otherwise <c>false</c>.</returns>
    public bool CanBeDeleted()
    {
        return !Children.Any(c => c.IsActive);
    }
}
