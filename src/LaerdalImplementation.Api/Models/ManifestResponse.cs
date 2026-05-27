using System;
using LaerdalImplementation.Domain.Enums;

namespace LaerdalImplementation.Api.Models;

/// <summary>
/// The JSON shape returned by manifest endpoints.
/// Separate from <c>ManifestDto</c> in the Application layer so the HTTP contract
/// can evolve independently of the internal representation.
/// </summary>
public class ManifestResponse
{
    /// <summary>Unique identifier of the manifest.</summary>
    public Guid Id { get; set; }

    /// <summary>ID of the organization this manifest belongs to.</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional human-readable description.</summary>
    public string? Description { get; set; }

    /// <summary>Semantic version string (e.g., "1.2.0").</summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>Current lifecycle status: Draft, Published, or Archived.</summary>
    public ManifestStatus Status { get; set; }

    /// <summary>
    /// JSON blob describing the courses and learning activities.
    /// Included in full so the training app can consume it in a single response.
    /// </summary>
    public string Content { get; set; } = "{}";

    /// <summary>UTC timestamp when the manifest was published, or <c>null</c> if still a draft.</summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>UTC timestamp when the manifest was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>UTC timestamp of the most recent update.</summary>
    public DateTime UpdatedAt { get; set; }
}
