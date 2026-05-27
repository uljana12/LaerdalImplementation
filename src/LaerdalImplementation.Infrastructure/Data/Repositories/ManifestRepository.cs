using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Enums;
using LaerdalImplementation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LaerdalImplementation.Infrastructure.Data.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IManifestRepository"/>.
/// All database access for manifests flows through this class.
/// Like <c>OrganizationRepository</c>, the rest of the application only sees the
/// interface, keeping the persistence technology swappable.
/// </summary>
public class ManifestRepository : IManifestRepository
{
    private readonly LaerdalDbContext _context;

    /// <summary>
    /// Initializes the repository with the scoped <see cref="LaerdalDbContext"/>
    /// injected by the DI container.
    /// </summary>
    /// <param name="context">The EF Core context for this request.</param>
    public ManifestRepository(LaerdalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Inserts a new manifest row into the database. Called when creating a draft
    /// or when publishing creates a new version record.
    /// </summary>
    public async Task<Manifest> AddAsync(Manifest manifest, CancellationToken cancellationToken = default)
    {
        _context.Manifests.Add(manifest);
        await _context.SaveChangesAsync(cancellationToken);
        return manifest;
    }

    /// <summary>
    /// Loads a single manifest by primary key with its owning organization,
    /// or returns <c>null</c> if not found.
    /// </summary>
    public async Task<Manifest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Manifests
            .Include(m => m.Organization)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    /// <summary>
    /// Returns the single <see cref="ManifestStatus.Published"/> manifest for an
    /// organization, or <c>null</c> if none exists. This is the primary read path
    /// for the downstream training application: it calls this before starting a
    /// learner session to obtain the active content snapshot. The query hits the
    /// <c>IX_Manifest_OrgStatus</c> index for efficiency.
    /// </summary>
    public async Task<Manifest?> GetPublishedByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Manifests
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.Status == ManifestStatus.Published, cancellationToken);
    }

    /// <summary>
    /// Returns all manifests for the given organization regardless of status.
    /// Useful for admin views showing the full version history.
    /// </summary>
    public async Task<IEnumerable<Manifest>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.Manifests
            .Where(m => m.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Returns manifests for the given organization filtered to a specific status.
    /// For example, fetching only drafts when the admin opens the "edit manifests" panel.
    /// </summary>
    public async Task<IEnumerable<Manifest>> GetByOrganizationIdAndStatusAsync(Guid organizationId, ManifestStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Manifests
            .Where(m => m.OrganizationId == organizationId && m.Status == status)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Persists changes to an existing manifest (e.g., archiving a previously published
    /// version, or stamping PublishedAt when promoting a draft).
    /// </summary>
    public async Task<Manifest> UpdateAsync(Manifest manifest, CancellationToken cancellationToken = default)
    {
        _context.Manifests.Update(manifest);
        await _context.SaveChangesAsync(cancellationToken);
        return manifest;
    }
}
