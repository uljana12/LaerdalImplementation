using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Enums;

namespace LaerdalImplementation.Domain.Repositories;

/// <summary>
/// Defines persistence operations for <see cref="Manifest"/> entities.
/// Lives in the Domain layer so the Application layer stays decoupled from any
/// specific storage technology. The EF Core implementation is in Infrastructure.
/// </summary>
public interface IManifestRepository
{
    /// <summary>Persists a new manifest and returns the saved entity.</summary>
    /// <param name="manifest">The manifest to insert.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<Manifest> AddAsync(Manifest manifest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single manifest by its ID, including its owning organization,
    /// or <c>null</c> if no match is found.
    /// </summary>
    /// <param name="id">The manifest's primary key.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<Manifest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the single <see cref="ManifestStatus.Published"/> manifest for the given
    /// organization, or <c>null</c> if none exists. Used by the training app to fetch
    /// the active content snapshot before starting a learner session.
    /// </summary>
    /// <param name="organizationId">The owning organization's ID.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<Manifest?> GetPublishedByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>Returns all manifests (all statuses) for the given organization.</summary>
    /// <param name="organizationId">The owning organization's ID.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<IEnumerable<Manifest>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns manifests for the given organization filtered to a specific status.
    /// Useful for listing only drafts or only archived versions.
    /// </summary>
    /// <param name="organizationId">The owning organization's ID.</param>
    /// <param name="status">The lifecycle stage to filter by.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<IEnumerable<Manifest>> GetByOrganizationIdAndStatusAsync(Guid organizationId, ManifestStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists changes to an existing manifest and returns the updated entity.
    /// Used when archiving a previously published manifest or promoting a draft.
    /// </summary>
    /// <param name="manifest">The manifest with updated field values.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<Manifest> UpdateAsync(Manifest manifest, CancellationToken cancellationToken = default);
}
