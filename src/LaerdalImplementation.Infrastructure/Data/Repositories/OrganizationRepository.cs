using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LaerdalImplementation.Infrastructure.Data.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IOrganizationRepository"/>.
/// All database access for organizations flows through this class.
/// The rest of the application only ever sees <see cref="IOrganizationRepository"/>,
/// so swapping to a different persistence layer (e.g., PostgreSQL, in-memory for tests)
/// requires changing only this class and its registration in <c>Program.cs</c>.
/// </summary>
public class OrganizationRepository : IOrganizationRepository
{
    private readonly LaerdalDbContext _context;

    /// <summary>
    /// Initializes the repository with the scoped <see cref="LaerdalDbContext"/>
    /// injected by the DI container.
    /// </summary>
    /// <param name="context">The EF Core context for this request.</param>
    public OrganizationRepository(LaerdalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds the organization to EF's change tracker and flushes it to the database
    /// with a single <c>INSERT</c> statement.
    /// </summary>
    public async Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);
        return organization;
    }

    /// <summary>
    /// Loads a single organization by primary key, eagerly including its parent
    /// and direct children so the caller has the full local hierarchy context.
    /// Returns <c>null</c> if not found.
    /// </summary>
    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .Include(o => o.Parent)
            .Include(o => o.Children)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    /// <summary>
    /// Finds an organization matching both <paramref name="code"/> and
    /// <paramref name="parentId"/> (which may be <c>null</c> for root orgs).
    /// EF Core correctly translates a null parentId to <c>WHERE ParentId IS NULL</c>.
    /// Used by the command handler to enforce code uniqueness before inserting.
    /// </summary>
    public async Task<Organization?> GetByCodeAsync(string code, Guid? parentId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .FirstOrDefaultAsync(o => o.Code == code && o.ParentId == parentId && o.IsActive, cancellationToken);
    }

    /// <summary>
    /// Returns all organizations, each with their parent and direct children loaded.
    /// Used by the list endpoint and the <c>?parentId</c>-less variant of the query.
    /// </summary>
    public async Task<IEnumerable<Organization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .Where(o => o.IsActive)
            .Include(o => o.Parent)
            .Include(o => o.Children)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Returns all direct children of the specified parent without loading their
    /// own children — suitable for shallow list views.
    /// </summary>
    public async Task<IEnumerable<Organization>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .Where(o => o.ParentId == parentId && o.IsActive)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Marks the entity as modified in EF's change tracker and persists the changes
    /// with an <c>UPDATE</c> statement. The entity must already be tracked or
    /// re-attached before calling this.
    /// </summary>
    public async Task<Organization> UpdateAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        _context.Organizations.Update(organization);
        await _context.SaveChangesAsync(cancellationToken);
        return organization;
    }

    /// <summary>
    /// Returns <c>true</c> if any organization row has the given ID.
    /// More efficient than <see cref="GetByIdAsync"/> when the caller only needs
    /// to verify existence (e.g., parent validation in the command handler).
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations.AnyAsync(o => o.Id == id, cancellationToken);
    }
}
