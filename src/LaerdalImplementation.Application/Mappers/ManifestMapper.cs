using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Domain.Entities;

namespace LaerdalImplementation.Application.Mappers;

/// <summary>
/// Converts between <see cref="Manifest"/> domain entities and <see cref="ManifestDto"/>
/// read models used by the Application layer.
/// </summary>
public static class ManifestMapper
{
    /// <summary>
    /// Maps a domain entity to a read DTO, including the full JSON content blob
    /// so consumers (e.g., the training app) receive everything in one response.
    /// </summary>
    /// <param name="manifest">The entity loaded from the repository.</param>
    /// <returns>A <see cref="ManifestDto"/> ready to return to the caller.</returns>
    public static ManifestDto ToDto(Manifest manifest)
    {
        return new ManifestDto
        {
            Id = manifest.Id,
            OrganizationId = manifest.OrganizationId,
            Name = manifest.Name,
            Description = manifest.Description,
            Version = manifest.Version,
            Status = manifest.Status,
            Content = manifest.Content,
            PublishedAt = manifest.PublishedAt,
            CreatedAt = manifest.CreatedAt,
            UpdatedAt = manifest.UpdatedAt
        };
    }
}
