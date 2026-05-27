using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Application.Mappers;
using LaerdalImplementation.Domain.Repositories;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Handles <see cref="UpdateOrganizationCommand"/>.
/// <para>
/// TODO: implement. Steps will be:
/// 1. Load org by ID (404 if not found).
/// 2. Call <c>org.Update(name, type, isActive)</c> — domain method validates and stamps UpdatedAt.
/// 3. Persist via <see cref="IOrganizationRepository.UpdateAsync"/>.
/// 4. Return updated DTO.
/// </para>
/// </summary>
public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
{
    private readonly IOrganizationRepository _organizationRepository;

    /// <summary>
    /// Initializes the handler with the organization repository injected by the DI container.
    /// </summary>
    public UpdateOrganizationCommandHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    /// <inheritdoc/>
    public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var org = await _organizationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (org == null)
            throw new InvalidOperationException($"Organization '{request.Id}' not found.");

        org.Update(request.Name, request.Type, request.IsActive);
        var updated = await _organizationRepository.UpdateAsync(org, cancellationToken);
        return OrganizationMapper.ToDto(updated);
    }
}
