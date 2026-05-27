using System;
using LaerdalImplementation.Domain.Enums;

namespace LaerdalImplementation.Domain.Entities;

/// <summary>
/// A versioned, auditable bundle that describes the structure and locations of course
/// content (courses, learning activities) for an organization's training needs.
/// <para>
/// Manifests follow a strict lifecycle: <see cref="ManifestStatus.Draft"/> →
/// <see cref="ManifestStatus.Published"/> → <see cref="ManifestStatus.Archived"/>.
/// Once published, a manifest is immutable — content changes require a new version.
/// This ensures learners mid-session always see a consistent snapshot of their content.
/// </para>
/// </summary>
public class Manifest
{
    /// <summary>Gets or sets the unique identifier for this manifest.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the ID of the organization this manifest belongs to.</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>Gets or sets the display name of the manifest.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets an optional human-readable description of what this manifest covers.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the semantic version string (e.g., "1.2.0"). Versions are assigned
    /// at publish time and are unique per organization.
    /// </summary>
    public string Version { get; set; } = "0.1.0";

    /// <summary>Gets or sets the current lifecycle status of the manifest.</summary>
    public ManifestStatus Status { get; set; } = ManifestStatus.Draft;

    /// <summary>
    /// Gets or sets the course content as a JSON string. The JSON structure describes
    /// courses and their nested learning activities. Stored as a blob to avoid complex
    /// joins when the training app fetches the full manifest.
    /// </summary>
    public string Content { get; set; } = "{}";

    /// <summary>
    /// Gets or sets the UTC timestamp when this manifest was published, or <c>null</c>
    /// if it is still a draft. Set once and never updated after publishing.
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>Gets or sets the UTC timestamp when this manifest was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the UTC timestamp of the most recent update.</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Navigation property to the owning organization.</summary>
    public Organization Organization { get; set; } = null!;

    /// <summary>
    /// Creates a new manifest in <see cref="ManifestStatus.Draft"/> status.
    /// All manifests must start as drafts; they cannot be created in a published state.
    /// </summary>
    /// <param name="organizationId">The ID of the owning organization.</param>
    /// <param name="name">Display name — must not be blank.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="content">Optional initial JSON content; defaults to an empty object.</param>
    /// <returns>A new draft <see cref="Manifest"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when name is empty.</exception>
    public static Manifest CreateDraft(Guid organizationId, string name, string? description = null, string? content = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Manifest name is required.", nameof(name));

        return new Manifest
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Name = name.Trim(),
            Description = description?.Trim(),
            Status = ManifestStatus.Draft,
            Version = "0.1.0",
            Content = content ?? "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Transitions this manifest from <see cref="ManifestStatus.Draft"/> to
    /// <see cref="ManifestStatus.Published"/> and stamps <see cref="PublishedAt"/>.
    /// After this call the manifest is immutable — no further content changes are allowed;
    /// new edits must be made on a new manifest version.
    /// </summary>
    /// <param name="newVersion">The semantic version string to assign (e.g., "1.0.0").</param>
    /// <exception cref="InvalidOperationException">Thrown if the manifest is already published.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="newVersion"/> is blank.</exception>
    public void Publish(string newVersion)
    {
        if (Status == ManifestStatus.Published)
            throw new InvalidOperationException("Manifest is already published.");

        if (string.IsNullOrWhiteSpace(newVersion))
            throw new ArgumentException("Version is required.", nameof(newVersion));

        Status = ManifestStatus.Published;
        Version = newVersion;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions this manifest to <see cref="ManifestStatus.Archived"/>. Archiving
    /// happens automatically when a newer manifest is published for the same organization.
    /// Archived manifests are retained so in-flight learner sessions can still reference them.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the manifest is already archived.</exception>
    public void Archive()
    {
        if (Status == ManifestStatus.Archived)
            throw new InvalidOperationException("Manifest is already archived.");

        Status = ManifestStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }
}
