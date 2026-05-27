using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Api.Models;
using LaerdalImplementation.Application.Commands;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaerdalImplementation.Api.Controllers;

/// <summary>
/// HTTP endpoints for managing organizations.
/// The controller is intentionally thin: it translates HTTP requests into MediatR
/// commands/queries, then maps the results back to HTTP responses. All business
/// logic lives in the Application and Domain layers.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes the controller with the MediatR dispatcher injected by the DI container.
    /// The controller only knows about <see cref="IMediator"/> — it has no direct
    /// dependency on repositories, DbContext, or any business logic.
    /// </summary>
    /// <param name="mediator">MediatR dispatcher that routes commands/queries to their handlers.</param>
    public OrganizationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new organization.
    /// </summary>
    /// <remarks>
    /// Returns <c>201 Created</c> with a <c>Location</c> header pointing to the new resource,
    /// or <c>400 Bad Request</c> if a business rule is violated (e.g., duplicate code,
    /// non-existent parent).
    /// </remarks>
    /// <param name="request">The organization details from the request body.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP connection.</param>
    [HttpPost]
    public async Task<ActionResult<OrganizationResponse>> Create(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateOrganizationCommand(
                new CreateOrganizationDto
                {
                    Name = request.Name,
                    Code = request.Code,
                    Type = request.Type,
                    ParentId = request.ParentId,
                    ExternalId = request.ExternalId
                }
            );

            var result = await _mediator.Send(command, cancellationToken);

            var response = MapDtoToResponse(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "An organization with that code already exists under the same parent." });
        }
    }

    /// <summary>
    /// Returns a list of organizations, optionally filtered to the direct children
    /// of a specific parent organization.
    /// </summary>
    /// <param name="parentId">
    /// Optional parent ID. When provided, only direct children are returned.
    /// When omitted, all organizations are returned.
    /// </param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP connection.</param>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationResponse>>> GetAll(
        [FromQuery] Guid? parentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOrganizationsQuery(parentId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result.Select(MapDtoToResponse));
    }

    /// <summary>
    /// Returns a single organization by ID, including its direct children.
    /// </summary>
    /// <param name="id">The organization's primary key.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP connection.</param>
    /// <returns><c>200 OK</c> with the organization, or <c>404 Not Found</c>.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrganizationResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrganizationByIdQuery(id);
        var org = await _mediator.Send(query, cancellationToken);

        if (org == null)
            return NotFound();

        return Ok(MapDtoToResponse(org));
    }

    /// <summary>
    /// Converts an <see cref="OrganizationDto"/> (Application layer type) to an
    /// <see cref="OrganizationResponse"/> (API layer type). Recursive so that the
    /// full child hierarchy is included in the response.
    /// </summary>
    /// <summary>
    /// Updates the mutable fields of an existing organization: name, type, and active status.
    /// The code and parent hierarchy are immutable after creation.
    /// </summary>
    /// <remarks>
    /// TODO: wire up. Will send an <c>UpdateOrganizationCommand</c> and return 200 / 404.
    /// </remarks>
    /// <param name="id">The organization's primary key.</param>
    /// <param name="request">Fields to update.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP connection.</param>
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateOrganizationCommand(id, request.Name, request.Type, request.IsActive);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(MapDtoToResponse(result));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Soft-deletes an organization by marking it inactive.
    /// </summary>
    /// <remarks>
    /// Returns <c>204 No Content</c> on success, <c>404 Not Found</c> if the organization
    /// does not exist, or <c>400 Bad Request</c> if it still has active child organizations.
    /// </remarks>
    /// <param name="id">The organization's primary key.</param>
    /// <param name="cancellationToken">Propagates cancellation from the HTTP connection.</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new DeleteOrganizationCommand(id), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static OrganizationResponse MapDtoToResponse(OrganizationDto dto)
    {
        return new OrganizationResponse
        {
            Id = dto.Id,
            ParentId = dto.ParentId,
            Name = dto.Name,
            Code = dto.Code,
            Type = dto.Type,
            ExternalId = dto.ExternalId,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Children = dto.Children.Select(MapDtoToResponse).ToList()
        };
    }
}
