using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Application.Mappers;
using LaerdalImplementation.Domain.Repositories;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Handles <see cref="PublishManifestCommand"/>.
/// 1. Loads the draft manifest (returns null → 404 if not found).
/// 2. Archives the currently published manifest for the org (if one exists).
/// 3. Bumps the version and publishes the draft.
/// </summary>
public class PublishManifestCommandHandler : IRequestHandler<PublishManifestCommand, ManifestDto>
{
    private readonly IManifestRepository _manifestRepository;

    public PublishManifestCommandHandler(IManifestRepository manifestRepository)
    {
        _manifestRepository = manifestRepository;
    }

    public async Task<ManifestDto> Handle(PublishManifestCommand request, CancellationToken cancellationToken)
    {
        var draft = await _manifestRepository.GetByIdAsync(request.ManifestId, cancellationToken)
            ?? throw new InvalidOperationException($"Manifest {request.ManifestId} not found.");

        // Archive the currently published manifest for this org, if there is one.
        var currentlyPublished = await _manifestRepository.GetPublishedByOrganizationAsync(
            request.OrganizationId, cancellationToken);

        if (currentlyPublished != null)
        {
            currentlyPublished.Archive();
            await _manifestRepository.UpdateAsync(currentlyPublished, cancellationToken);
        }

        // Calculate the new version by bumping the draft's current version.
        var newVersion = BumpVersion(draft.Version, request.VersionBump);
        draft.Publish(newVersion);

        var saved = await _manifestRepository.UpdateAsync(draft, cancellationToken);
        return ManifestMapper.ToDto(saved);
    }

    private static string BumpVersion(string version, string bump)
    {
        var parts = version.Split('.');
        var major = int.Parse(parts[0]);
        var minor = int.Parse(parts[1]);
        var patch = int.Parse(parts[2]);

        return bump.ToLowerInvariant() switch
        {
            "major" => $"{major + 1}.0.0",
            "minor" => $"{major}.{minor + 1}.0",
            _       => $"{major}.{minor}.{patch + 1}",  // "patch" or anything else
        };
    }
}
