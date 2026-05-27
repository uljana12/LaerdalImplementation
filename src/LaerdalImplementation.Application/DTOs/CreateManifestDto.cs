namespace LaerdalImplementation.Application.DTOs;

/// <summary>
/// Carries the input data needed to create a new manifest draft.
/// Flows from the API layer through to the manifest command handler.
/// </summary>
public class CreateManifestDto
{
    /// <summary>Display name for the manifest. Must not be blank.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional human-readable description of what this manifest covers.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional initial JSON content following the courses/activities structure.
    /// Defaults to an empty object if not provided; content can be added before publishing.
    /// </summary>
    public string? Content { get; set; }
}
