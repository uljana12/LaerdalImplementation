using System;
using System.Collections.Generic;
using LaerdalImplementation.Application.DTOs;
using MediatR;

namespace LaerdalImplementation.Application.Queries;

/// <summary>
/// Query that requests a list of organizations, with an optional filter by parent.
/// <list type="bullet">
///   <item>When <see cref="ParentId"/> is <c>null</c>, all organizations are returned.</item>
///   <item>When <see cref="ParentId"/> is set, only direct children of that org are returned.</item>
/// </list>
/// </summary>
/// <param name="ParentId">Optional parent organization ID to filter by.</param>
public record GetOrganizationsQuery(Guid? ParentId = null) : IRequest<IEnumerable<OrganizationDto>>;
