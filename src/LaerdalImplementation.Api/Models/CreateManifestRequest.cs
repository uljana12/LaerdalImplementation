namespace LaerdalImplementation.Api.Models;

/// <summary>
/// The JSON body expected by <c>POST /api/organizations/{orgId}/manifests</c>.
/// Creates a new manifest in Draft status for the specified organization.
/// </summary>
public class CreateManifestRequest
{
    /// <summary>Display name for the manifest. Required.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional human-readable description of what this manifest covers.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional initial JSON content in the courses/activities format.
    /// Can be left empty and populated later before publishing.
    /// </summary>
    public string? Content { get; set; }
}
