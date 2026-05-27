using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LaerdalImplementation.Api.Middleware;

/// <summary>
/// ASP.NET Core middleware that validates the JWT bearer token on every request
/// and populates <see cref="HttpContext.User"/> with the authenticated principal.
/// <para>
/// Currently a pass-through stub — all requests are allowed through.
/// </para>
/// TODO: implement.
/// <list type="bullet">
///   <item>Extract the <c>Authorization: Bearer {token}</c> header.</item>
///   <item>Delegate validation to <c>AuthenticationService.ValidateTokenAsync</c>.</item>
///   <item>On success: set <c>HttpContext.User</c> to the typed claims principal.</item>
///   <item>On failure: short-circuit with <c>401 Unauthorized</c> before the request reaches a controller.</item>
///   <item>Skip validation for public routes (e.g., <c>/api/auth/login</c>, <c>/health</c>).</item>
/// </list>
/// </summary>
public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes the middleware with the next delegate in the ASP.NET Core pipeline.
    /// </summary>
    /// <param name="next">The next middleware component.</param>
    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware for the current HTTP request.
    /// Currently passes through to the next component unconditionally.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public Task InvokeAsync(HttpContext context)
    {
        // TODO: validate JWT and populate context.User before calling _next
        return _next(context);
    }
}
