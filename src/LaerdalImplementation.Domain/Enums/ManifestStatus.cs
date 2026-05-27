namespace LaerdalImplementation.Domain.Enums;

/// <summary>
/// Represents the lifecycle stage of a <see cref="Entities.Manifest"/>.
/// Transitions flow in one direction only: Draft → Published → Archived.
/// Stored as an integer in the database (0, 1, 2).
/// </summary>
public enum ManifestStatus
{
    /// <summary>
    /// The manifest is being edited and has not yet been released to learners.
    /// Draft manifests can be freely modified or deleted.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// The manifest has been released. Its content is immutable — any edits require
    /// creating a new manifest version. Only one manifest per organization may be
    /// in this state at a time.
    /// </summary>
    Published = 1,

    /// <summary>
    /// The manifest was previously published but has since been superseded by a newer
    /// version. Archived manifests are retained so that in-flight learner sessions
    /// can still reference their pinned snapshot.
    /// </summary>
    Archived = 2
}
