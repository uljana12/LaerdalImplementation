using LaerdalImplementation.Domain.Enums;

namespace LaerdalImplementation.Api.Models;

/// <summary>
/// The JSON body expected by <c>PATCH /api/organizations/{id}</c>.
/// Only mutable fields are exposed — code and parent hierarchy cannot change
/// after creation to preserve referential integrity.
/// </summary>
public class UpdateOrganizationRequest
{
    /// <summary>New display name. Required.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>New organization type.</summary>
    public OrganizationType Type { get; set; }

    /// <summary>Set to <c>false</c> to soft-delete the organization.</summary>
    public bool IsActive { get; set; } = true;
}
