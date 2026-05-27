using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Api.Models;
using LaerdalImplementation.Application.Commands;
using LaerdalImplementation.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LaerdalImplementation.Api.Controllers;

/// <summary>
/// HTTP endpoints for listing, creating, and publishing manifests.
/// </summary>
[ApiController]
[Route("api")]
public class ManifestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ManifestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns all manifests for the specified organization, newest first.
    /// </summary>
    [HttpGet("organizations/{orgId}/manifests")]
    public async Task<IActionResult> List(Guid orgId, CancellationToken cancellationToken)
    {
        var manifests = await _mediator.Send(
            new GetManifestsByOrganizationQuery(orgId), cancellationToken);
        return Ok(manifests);
    }

    /// <summary>
    /// Creates a new manifest draft for the specified organization.
    /// Returns 201 Created with the new manifest.
    /// </summary>
    [HttpPost("organizations/{orgId}/manifests")]
    public async Task<IActionResult> CreateDraft(
        Guid orgId,
        [FromBody] CreateManifestRequest request,
        CancellationToken cancellationToken)
    {
        var manifest = await _mediator.Send(
            new CreateManifestCommand(orgId, request.Name, request.Description, request.Content),
            cancellationToken);

        return CreatedAtAction(nameof(List), new { orgId }, manifest);
    }

    /// <summary>
    /// Publishes a draft manifest. Archives the currently published manifest for
    /// the organization (if one exists) and assigns the new semantic version.
    /// </summary>
    [HttpPost("organizations/{orgId}/manifests/{manifestId}/publish")]
    public async Task<IActionResult> Publish(
        Guid orgId,
        Guid manifestId,
        [FromBody] PublishManifestRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var manifest = await _mediator.Send(
                new PublishManifestCommand(orgId, manifestId, request.VersionBump),
                cancellationToken);
            return Ok(manifest);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Returns the currently published manifest for the organization identified
    /// by its short code. Used by the downstream training application.
    /// </summary>
    [HttpGet("manifests/published/{orgCode}")]
    public async Task<IActionResult> GetPublished(
        string orgCode,
        CancellationToken cancellationToken)
    {
        var manifest = await _mediator.Send(
            new GetPublishedManifestQuery(orgCode), cancellationToken);

        if (manifest is null)
            return NotFound(new { error = $"No published manifest found for org '{orgCode}'." });

        return Ok(manifest);
    }
}
