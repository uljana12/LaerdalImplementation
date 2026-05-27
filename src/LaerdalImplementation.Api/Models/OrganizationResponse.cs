using System;
using System.Collections.Generic;
using LaerdalImplementation.Domain.Enums;

namespace LaerdalImplementation.Api.Models;

/// <summary>
/// The JSON shape returned by all organization endpoints.
/// Keeping this separate from <c>OrganizationDto</c> (the Application layer type) means
/// the HTTP contract can be versioned or shaped independently of the internal representation.
/// The <see cref="Children"/> list is recursive, so a single response carries the full
/// org subtree without requiring additional round-trips.
/// </summary>
public class OrganizationResponse
{
    /// <summary>Unique identifier of the organization.</summary>
    public Guid Id { get; set; }

    /// <summary>ID of the parent organization, or <c>null</c> for a root org.</summary>
    public Guid? ParentId { get; set; }

    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Short identifier code (uppercase, unique within parent scope).</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>The kind of organization: Hospital, Department, or TrainingCenter.</summary>
    public OrganizationType Type { get; set; }

    /// <summary>Optional reference to an external identity system.</summary>
    public string? ExternalId { get; set; }

    /// <summary>Whether the organization is currently active (false = soft-deleted).</summary>
    public bool IsActive { get; set; }

    /// <summary>UTC timestamp when the organization was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>UTC timestamp of the most recent update.</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Direct child organizations, each with their own children included recursively.
    /// </summary>
    public List<OrganizationResponse> Children { get; set; } = new();
}
