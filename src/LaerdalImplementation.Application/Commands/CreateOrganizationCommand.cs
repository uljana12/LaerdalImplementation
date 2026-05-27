using LaerdalImplementation.Application.DTOs;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Command that requests the creation of a new organization.
/// Implements <see cref="IRequest{TResponse}"/> so MediatR can route it to
/// <see cref="CreateOrganizationCommandHandler"/> without the caller needing a
/// direct reference to the handler.
/// </summary>
/// <param name="Data">The validated input data for the new organization.</param>
public record CreateOrganizationCommand(CreateOrganizationDto Data) : IRequest<OrganizationDto>;
