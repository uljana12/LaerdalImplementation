using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Domain.Repositories;
using MediatR;

namespace LaerdalImplementation.Application.Queries;

/// <summary>
/// Handles <see cref="GetPublishedManifestQuery"/>.
/// <para>
/// TODO: implement. Steps will be:
/// 1. Resolve org by code via <c>IOrganizationRepository.GetByCodeAsync</c> (null parentId for root,
///    or extend the interface to support a code-only lookup).
/// 2. If org not found, return null (caller returns 404).
/// 3. Call <c>IManifestRepository.GetPublishedByOrganizationAsync(org.Id)</c> —
///    this hits the IX_Manifest_OrgStatus index.
/// 4. Map to <see cref="ManifestDto"/> and return.
/// </para>
/// </summary>
public class GetPublishedManifestQueryHandler : IRequestHandler<GetPublishedManifestQuery, ManifestDto?>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IManifestRepository _manifestRepository;

    /// <summary>
    /// Initializes the handler with repositories injected by the DI container.
    /// </summary>
    public GetPublishedManifestQueryHandler(
        IOrganizationRepository organizationRepository,
        IManifestRepository manifestRepository)
    {
        _organizationRepository = organizationRepository;
        _manifestRepository = manifestRepository;
    }

    /// <inheritdoc/>
    public Task<ManifestDto?> Handle(GetPublishedManifestQuery request, CancellationToken cancellationToken)
        => throw new NotImplementedException("GetPublishedManifestQueryHandler is not yet implemented.");
}
