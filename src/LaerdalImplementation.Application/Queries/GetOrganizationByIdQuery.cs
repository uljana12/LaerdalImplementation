using System;
using LaerdalImplementation.Application.DTOs;
using MediatR;

namespace LaerdalImplementation.Application.Queries;

/// <summary>
/// Query that requests a single organization by its primary key ID,
/// including its parent reference and direct children.
/// Returns <c>null</c> when no organization with that ID exists.
/// </summary>
/// <param name="Id">The primary key of the organization to retrieve.</param>
public record GetOrganizationByIdQuery(Guid Id) : IRequest<OrganizationDto?>;
