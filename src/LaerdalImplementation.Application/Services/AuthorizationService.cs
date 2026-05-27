using System;
using System.Threading;
using System.Threading.Tasks;

namespace LaerdalImplementation.Application.Services;

/// <summary>
/// Enforces role-based and scope-based access control for organizations and manifests.
/// <para>
/// TODO: implement. Authorization levels:
/// <list type="bullet">
///   <item><b>LaerdalAdmin</b> (role = <c>laerdal_admin</c>) — full CRUD on any organization,
///         full manifest management across all orgs.</item>
///   <item><b>OrgAdmin</b> (role = <c>org_admin</c> + <c>orgId</c> claim) — CRUD scoped to their
///         own org and descendants; cannot modify the parent org.</item>
///   <item><b>OrgEditor</b> (role = <c>org_editor</c>) — can draft and publish manifests;
///         read-only on organization settings.</item>
///   <item><b>ServiceAccount</b> — OAuth 2.0 client credentials; read-only access scoped to a
///         specific org code via the <c>manifest:read:{orgCode}</c> scope.</item>
/// </list>
/// </para>
/// </summary>
public class AuthorizationService
{
    // TODO: inject IOrganizationRepository to resolve the org hierarchy for
    //       descendant-scoped permission checks.

    /// <summary>
    /// Returns <c>true</c> if the current principal is allowed to perform the requested
    /// operation on the specified organization.
    /// </summary>
    /// <param name="principal">The authenticated principal (from AuthenticationService).</param>
    /// <param name="organizationId">The target organization.</param>
    /// <param name="requiredRole">The minimum role required for the operation.</param>
    public Task<bool> CanAccessOrganizationAsync(
        object principal,
        Guid organizationId,
        string requiredRole,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException("AuthorizationService is not yet implemented.");
}
