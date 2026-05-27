namespace LaerdalImplementation.Api.Models;

/// <summary>
/// The JSON body expected by <c>POST /api/organizations/{orgId}/manifests/{manifestId}/publish</c>.
/// </summary>
public class PublishManifestRequest
{
    /// <summary>
    /// Which semantic version component to increment: "major", "minor", or "patch".
    /// <list type="bullet">
    ///   <item><c>major</c> — breaking restructure of courses or activities.</item>
    ///   <item><c>minor</c> — new courses or activities added.</item>
    ///   <item><c>patch</c> — small corrections (titles, durations, typos).</item>
    /// </list>
    /// </summary>
    public string VersionBump { get; set; } = "minor";
}
