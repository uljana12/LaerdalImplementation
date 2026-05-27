using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Domain.Repositories;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Handles <see cref="PublishManifestCommand"/>.
/// <para>
/// TODO: implement. Steps will be:
/// 1. Load the draft manifest by ID (404 if not found, 400 if already published).
/// 2. Load the current Published manifest for the org (may be null for first publish).
/// 3. Calculate the new version using <c>ManifestVersion.Parse().Bump*()</c>.
/// 4. Call <c>currentPublished?.Archive()</c> on the old manifest.
/// 5. Call <c>draftManifest.Publish(newVersion.ToString())</c>.
/// 6. Persist both changes — ideally in a single transaction so they're atomic.
/// 7. Return the newly published manifest as a DTO.
/// </para>
/// </summary>
public class PublishManifestCommandHandler : IRequestHandler<PublishManifestCommand, ManifestDto>
{
    private readonly IManifestRepository _manifestRepository;

    /// <summary>
    /// Initializes the handler with the manifest repository injected by the DI container.
    /// </summary>
    public PublishManifestCommandHandler(IManifestRepository manifestRepository)
    {
        _manifestRepository = manifestRepository;
    }

    /// <inheritdoc/>
    public Task<ManifestDto> Handle(PublishManifestCommand request, CancellationToken cancellationToken)
        => throw new NotImplementedException("PublishManifestCommandHandler is not yet implemented.");
}
