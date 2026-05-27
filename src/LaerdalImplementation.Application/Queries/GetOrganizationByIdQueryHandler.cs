using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Application.Mappers;
using LaerdalImplementation.Domain.Repositories;
using MediatR;

namespace LaerdalImplementation.Application.Queries;

/// <summary>
/// Handles <see cref="GetOrganizationByIdQuery"/> by fetching a single organization
/// directly from the repository by primary key, then mapping it to a DTO.
/// </summary>
public class GetOrganizationByIdQueryHandler : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto?>
{
    private readonly IOrganizationRepository _organizationRepository;

    /// <summary>
    /// Initializes the handler with the organization repository injected by the DI container.
    /// </summary>
    /// <param name="organizationRepository">Abstraction over the Organizations table.</param>
    public GetOrganizationByIdQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    /// <summary>
    /// Fetches the organization with the specified ID and maps it to a DTO,
    /// or returns <c>null</c> if no such organization exists.
    /// </summary>
    /// <param name="request">The query carrying the target organization ID.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    /// <returns>An <see cref="OrganizationDto"/> or <c>null</c>.</returns>
    public async Task<OrganizationDto?> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(request.Id, cancellationToken);
        return organization is null ? null : OrganizationMapper.ToDto(organization);
    }
}
