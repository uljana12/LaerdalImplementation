using System;
using LaerdalImplementation.Domain.Enums;

namespace LaerdalImplementation.Application.DTOs;

/// <summary>
/// Carries the input data needed to create a new organization.
/// Flows from the API layer (deserialized from the HTTP request body) through to
/// <see cref="Commands.CreateOrganizationCommandHandler"/>.
/// </summary>
public class CreateOrganizationDto
{
    /// <summary>Display name for the organization. Must not be blank.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short identifier code. Must be unique within the same parent scope.
    /// Stored as uppercase.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>The kind of organization being created.</summary>
    public OrganizationType Type { get; set; }

    /// <summary>
    /// ID of the parent organization, or <c>null</c> to create a root-level organization.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Optional reference to an identity in an external system (e.g., OIDC directory).
    /// </summary>
    public string? ExternalId { get; set; }
}
