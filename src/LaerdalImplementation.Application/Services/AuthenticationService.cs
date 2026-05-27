using System;
using System.Threading;
using System.Threading.Tasks;

namespace LaerdalImplementation.Application.Services;

/// <summary>
/// Handles OIDC token validation and extracts a typed principal from the JWT.
/// <para>
/// TODO: implement. Responsibilities:
/// <list type="bullet">
///   <item>Fetch and cache the Laerdal OIDC public key (JWKS endpoint, 5-min TTL).</item>
///   <item>Validate the JWT signature, expiry, and issuer on each request.</item>
///   <item>Extract <c>sub</c>, <c>org</c>, and <c>roles</c> claims into a typed principal.</item>
///   <item>Surface the principal via an interface so controllers and policies can read it.</item>
/// </list>
/// </para>
/// </summary>
public class AuthenticationService
{
    // TODO: inject IHttpClientFactory for JWKS fetching and IMemoryCache for key caching.

    /// <summary>
    /// Validates the bearer token from the HTTP Authorization header and returns
    /// the authenticated principal, or throws if the token is invalid/expired.
    /// </summary>
    /// <param name="bearerToken">Raw JWT string (without "Bearer " prefix).</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
    public Task<object> ValidateTokenAsync(string bearerToken, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("AuthenticationService is not yet implemented.");
}
