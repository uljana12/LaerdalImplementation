using System;

namespace LaerdalImplementation.Domain.ValueObjects;

/// <summary>
/// Represents a semantic version (Major.Minor.Patch) for a manifest.
/// Encapsulates the version bumping rules so callers can't construct an
/// invalid version string and the logic is not scattered across handlers.
/// </summary>
public record ManifestVersion
{
    /// <summary>Breaking change — reset Minor and Patch to 0.</summary>
    public int Major { get; }

    /// <summary>Backwards-compatible new content — reset Patch to 0.</summary>
    public int Minor { get; }

    /// <summary>Small corrections that don't change the content structure.</summary>
    public int Patch { get; }

    /// <param name="major">Must be ≥ 0.</param>
    /// <param name="minor">Must be ≥ 0.</param>
    /// <param name="patch">Must be ≥ 0.</param>
    public ManifestVersion(int major, int minor, int patch)
    {
        if (major < 0 || minor < 0 || patch < 0)
            throw new ArgumentException("Version components must be non-negative.");

        Major = major;
        Minor = minor;
        Patch = patch;
    }

    /// <summary>
    /// Parses a version string in the form "Major.Minor.Patch".
    /// </summary>
    /// <param name="version">e.g. "1.2.3"</param>
    /// <exception cref="FormatException">Thrown for malformed strings.</exception>
    public static ManifestVersion Parse(string version)
    {
        var parts = version?.Split('.') ?? [];
        if (parts.Length != 3
            || !int.TryParse(parts[0], out var major)
            || !int.TryParse(parts[1], out var minor)
            || !int.TryParse(parts[2], out var patch))
            throw new FormatException($"Invalid version string: '{version}'. Expected 'Major.Minor.Patch'.");

        return new ManifestVersion(major, minor, patch);
    }

    /// <summary>Returns a new version with Major incremented and Minor/Patch reset to 0.</summary>
    public ManifestVersion BumpMajor() => new(Major + 1, 0, 0);

    /// <summary>Returns a new version with Minor incremented and Patch reset to 0.</summary>
    public ManifestVersion BumpMinor() => new(Major, Minor + 1, 0);

    /// <summary>Returns a new version with only Patch incremented.</summary>
    public ManifestVersion BumpPatch() => new(Major, Minor, Patch + 1);

    /// <summary>Returns the canonical "Major.Minor.Patch" string (e.g. "1.2.3").</summary>
    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}
