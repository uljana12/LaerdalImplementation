using System;
using System.Collections.Generic;
using LaerdalImplementation.Application.DTOs;
using MediatR;

namespace LaerdalImplementation.Application.Queries;

/// <summary>
/// Query that returns all manifests (all statuses) for the given organization.
/// </summary>
/// <param name="OrganizationId">The owning organization's ID.</param>
public record GetManifestsByOrganizationQuery(Guid OrganizationId) : IRequest<IEnumerable<ManifestDto>>;
