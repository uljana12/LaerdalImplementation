using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Domain.Repositories;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Handles <see cref="DeleteOrganizationCommand"/>.
/// Soft-deletes the organization by setting IsActive to false.
/// Rejects the request if the org has active children to avoid orphaning them.
/// </summary>
public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand>
{
    private readonly IOrganizationRepository _organizationRepository;

    /// <summary>
    /// Initializes the handler with the organization repository injected by the DI container.
    /// </summary>
    /// <param name="organizationRepository">Repository for loading and persisting organizations.</param>
    public DeleteOrganizationCommandHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    /// <inheritdoc/>
    public async Task Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var org = await _organizationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (org == null)
            throw new InvalidOperationException($"Organization '{request.Id}' not found.");

        if (!org.CanBeDeleted())
            throw new InvalidOperationException("Cannot delete an organization that has active child organizations.");

        org.Update(org.Name, org.Type, isActive: false);
        await _organizationRepository.UpdateAsync(org, cancellationToken);
    }
}
