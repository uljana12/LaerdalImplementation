using System;
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
/// Handles <see cref="GetOrganizationsQuery"/> by retrieving organizations from the
/// repository and mapping them to DTOs.
/// </summary>
public class GetOrganizationsQueryHandler : IRequestHandler<GetOrganizationsQuery, IEnumerable<OrganizationDto>>
{
    private readonly IOrganizationRepository _organizationRepository;

    /// <summary>
    /// Initializes the handler with the organization repository injected by the DI container.
    /// </summary>
    /// <param name="organizationRepository">Abstraction over the Organizations table.</param>
    public GetOrganizationsQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    /// <summary>
    /// Fetches organizations — all of them, or only direct children of the specified parent —
    /// and maps each to a <see cref="OrganizationDto"/> (including nested children).
    /// </summary>
    /// <param name="request">The query, optionally carrying a parent ID filter.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    /// <returns>A sequence of organization DTOs.</returns>
    public async Task<IEnumerable<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Organization> organizations;

        if (request.ParentId.HasValue)
        {
            organizations = await _organizationRepository.GetByParentIdAsync(request.ParentId.Value, cancellationToken);
        }
        else
        {
            organizations = await _organizationRepository.GetAllAsync(cancellationToken);
        }

        return organizations.Select(OrganizationMapper.ToDto);
    }
}
