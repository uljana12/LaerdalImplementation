using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Application.Mappers;
using LaerdalImplementation.Domain.Repositories;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Handles <see cref="CreateOrganizationCommand"/> by enforcing business rules,
/// constructing the domain entity via its factory, and persisting it.
/// <para>
/// Business rules enforced here (rather than in the controller) so they apply
/// regardless of how the command is triggered:
/// <list type="bullet">
///   <item>Organization code must be unique within the same parent scope.</item>
///   <item>The specified parent organization must already exist.</item>
/// </list>
/// </para>
/// </summary>
public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
{
    private readonly IOrganizationRepository _organizationRepository;

    /// <summary>
    /// Initializes the handler with the organization repository injected by the DI container.
    /// </summary>
    /// <param name="organizationRepository">Abstraction over the Organizations table.</param>
    public CreateOrganizationCommandHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    /// <summary>
    /// Executes the create-organization use case end-to-end:
    /// validate → construct domain entity → persist → return DTO.
    /// </summary>
    /// <param name="request">The command carrying the new organization's data.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    /// <returns>A DTO representing the newly created organization.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the code already exists under the same parent, or if the specified
    /// parent organization does not exist.
    /// </exception>
    public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        // Validate code uniqueness within parent scope
        var existingOrg = await _organizationRepository.GetByCodeAsync(data.Code, data.ParentId, cancellationToken);
        if (existingOrg != null)
            throw new InvalidOperationException($"Organization with code '{data.Code}' already exists under the same parent.");

        // Validate parent exists if specified
        if (data.ParentId.HasValue)
        {
            var parentExists = await _organizationRepository.ExistsAsync(data.ParentId.Value, cancellationToken);
            if (!parentExists)
                throw new InvalidOperationException($"Parent organization with ID '{data.ParentId}' does not exist.");
        }

        // Create and persist organization
        var organization = OrganizationMapper.ToEntity(data, data.ParentId);
        var created = await _organizationRepository.AddAsync(organization, cancellationToken);

        return OrganizationMapper.ToDto(created);
    }
}
