using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Api.Models;
using LaerdalImplementation.Application.Commands;
using LaerdalImplementation.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LaerdalImplementation.Api.Controllers;

/// <summary>
/// HTTP endpoints for creating, publishing, and fetching manifests.
/// All three routes are stubs — they return 501 Not Implemented until the
/// corresponding command/query handlers are built.
/// The routing structure is intentional:
/// <list type="bullet">
///   <item>Manifest create and publish are nested under /organizations/{orgId}/... to make
///         the ownership hierarchy explicit in the URL.</item>
///   <item>The training-app fetch lives at /manifests/published/{orgCode} (not nested) because
///         the caller identifies the org by its human-readable code, not a GUID.</item>
/// </list>
/// </summary>
[ApiController]
[Route("api")]
public class ManifestsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes the controller with the MediatR dispatcher injected by the DI container.
    /// </summary>
    public ManifestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new manifest draft for the specified organization.
    /// </summary>
    /// <remarks>
    /// TODO: wire up. Will send a <c>CreateManifestCommand</c> and return 201 Created.
    /// </remarks>
    /// <param name="orgId">The owning organization's ID.</param>
    /// <param name="request">Manifest name, description, and optional initial content.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP connection.</param>
    [HttpPost("organizations/{orgId}/manifests")]
    public Task<IActionResult> CreateDraft(
        Guid orgId,
        [FromBody] CreateManifestRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: send CreateManifestCommand, return CreatedAtAction with ManifestResponse
        return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status501NotImplemented));
    }

    /// <summary>
    /// Publishes a draft manifest, assigns it a new semantic version, and archives
    /// the previously published manifest for the organization (if one exists).
    /// </summary>
    /// <remarks>
    /// TODO: wire up. Will send a <c>PublishManifestCommand</c> and return 200 with the published manifest.
    /// </remarks>
    /// <param name="orgId">The owning organization's ID.</param>
    /// <param name="manifestId">The draft manifest to publish.</param>
    /// <param name="request">The version bump type: major, minor, or patch.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP connection.</param>
    [HttpPost("organizations/{orgId}/manifests/{manifestId}/publish")]
    public Task<IActionResult> Publish(
        Guid orgId,
        Guid manifestId,
        [FromBody] PublishManifestRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: send PublishManifestCommand(orgId, manifestId, request.VersionBump), return 200 ManifestResponse
        return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status501NotImplemented));
    }

    /// <summary>
    /// Returns the currently published manifest for the organization identified by
    /// <paramref name="orgCode"/>. This is the primary endpoint consumed by the
    /// downstream training application before starting a learner session.
    /// </summary>
    /// <remarks>
    /// Auth: requires a service-account token with scope <c>manifest:read:{orgCode}</c>
    /// (OAuth 2.0 client credentials). Not yet enforced — TODO: add [Authorize] policy.
    /// <br/>
    /// TODO: wire up. Will send a <c>GetPublishedManifestQuery</c> and return 200 / 404.
    /// </remarks>
    /// <param name="orgCode">The organization's short identifier code.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP connection.</param>
    [HttpGet("manifests/published/{orgCode}")]
    public Task<IActionResult> GetPublished(
        string orgCode,
        CancellationToken cancellationToken)
    {
        // TODO: send GetPublishedManifestQuery(orgCode), return 200 ManifestResponse or 404
        return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status501NotImplemented));
    }
}
