using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Application.Mappers;
using LaerdalImplementation.Domain.Repositories;
using MediatR;

namespace LaerdalImplementation.Application.Queries;

/// <summary>
/// Handles <see cref="GetManifestsByOrganizationQuery"/>.
/// Returns all manifests for the organization ordered newest first.
/// </summary>
public class GetManifestsByOrganizationQueryHandler
    : IRequestHandler<GetManifestsByOrganizationQuery, IEnumerable<ManifestDto>>
{
    private readonly IManifestRepository _manifestRepository;

    public GetManifestsByOrganizationQueryHandler(IManifestRepository manifestRepository)
    {
        _manifestRepository = manifestRepository;
    }

    public async Task<IEnumerable<ManifestDto>> Handle(
        GetManifestsByOrganizationQuery request,
        CancellationToken cancellationToken)
    {
        var manifests = await _manifestRepository.GetByOrganizationIdAsync(
            request.OrganizationId, cancellationToken);

        return manifests
            .OrderByDescending(m => m.CreatedAt)
            .Select(ManifestMapper.ToDto);
    }
}
