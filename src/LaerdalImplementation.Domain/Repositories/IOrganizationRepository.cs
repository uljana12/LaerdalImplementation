using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Domain.Entities;

namespace LaerdalImplementation.Domain.Repositories;

/// <summary>
/// Defines persistence operations for <see cref="Organization"/> entities.
/// The interface lives in the Domain layer so that the Application layer can depend
/// on the abstraction rather than on any specific database technology.
/// The concrete implementation (EF Core / SQL Server) lives in the Infrastructure layer.
/// </summary>
public interface IOrganizationRepository
{
    /// <summary>Persists a new organization and returns the saved entity.</summary>
    /// <param name="organization">The organization to insert.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single organization by its ID, including its parent and direct
    /// children, or <c>null</c> if no match is found.
    /// </summary>
    /// <param name="id">The organization's primary key.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds an organization by code within a specific parent scope, or <c>null</c>
    /// if no match exists. Used to enforce the uniqueness business rule before inserting.
    /// </summary>
    /// <param name="code">The short identifier code (case-sensitive after normalization).</param>
    /// <param name="parentId">
    /// The parent organization's ID, or <c>null</c> to search among root organizations.
    /// </param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<Organization?> GetByCodeAsync(string code, Guid? parentId = null, CancellationToken cancellationToken = default);

    /// <summary>Returns all organizations, each including their parent and direct children.</summary>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<IEnumerable<Organization>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns all direct children of the specified parent organization.</summary>
    /// <param name="parentId">The parent organization's ID.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<IEnumerable<Organization>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default);

    /// <summary>Persists changes to an existing organization and returns the updated entity.</summary>
    /// <param name="organization">The organization with updated field values.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<Organization> UpdateAsync(Organization organization, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <c>true</c> if an organization with the given ID exists; otherwise <c>false</c>.
    /// Cheaper than <see cref="GetByIdAsync"/> when only existence matters (e.g., parent validation).
    /// </summary>
    /// <param name="id">The organization's primary key.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
