using System;
using System.Collections.Generic;
using LaerdalImplementation.Domain.Enums;

namespace LaerdalImplementation.Application.DTOs;

/// <summary>
/// Read model for an organization, returned by queries and commands.
/// Includes a recursive <see cref="Children"/> collection so callers receive the
/// full org subtree in a single response without needing extra requests.
/// <para>
/// Using a dedicated DTO (rather than exposing the domain entity directly) keeps
/// the API contract stable when the entity's internal structure changes, and avoids
/// circular-reference issues during JSON serialization.
/// </para>
/// </summary>
public class OrganizationDto
{
    /// <summary>Unique identifier of the organization.</summary>
    public Guid Id { get; set; }

    /// <summary>ID of the parent organization, or <c>null</c> for a root org.</summary>
    public Guid? ParentId { get; set; }

    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Short identifier code (uppercase).</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>The kind of organization.</summary>
    public OrganizationType Type { get; set; }

    /// <summary>Optional reference to an external identity system.</summary>
    public string? ExternalId { get; set; }

    /// <summary>Whether the organization is currently active.</summary>
    public bool IsActive { get; set; }

    /// <summary>UTC timestamp when the organization was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>UTC timestamp of the most recent update.</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Direct child organizations, each with their own children recursively.</summary>
    public List<OrganizationDto> Children { get; set; } = new();
}
