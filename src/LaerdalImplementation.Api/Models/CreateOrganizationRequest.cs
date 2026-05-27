using System;
using LaerdalImplementation.Domain.Enums;

namespace LaerdalImplementation.Api.Models;

/// <summary>
/// The JSON body expected by <c>POST /api/organizations</c>.
/// This is the API layer's own contract — separate from <c>CreateOrganizationDto</c>
/// in the Application layer — so that HTTP-specific concerns (e.g., validation
/// attributes, serialization aliases) can evolve independently of the use-case logic.
/// </summary>
public class CreateOrganizationRequest
{
    /// <summary>Display name for the new organization. Required.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short identifier code. Must be unique within the same parent scope. Required.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>The kind of organization: Hospital (0), Department (1), or TrainingCenter (2).</summary>
    public OrganizationType Type { get; set; }

    /// <summary>
    /// ID of an existing parent organization, or omit/null for a root-level organization.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Optional reference to an identity in an external system (e.g., OIDC directory entry).
    /// </summary>
    public string? ExternalId { get; set; }
}
