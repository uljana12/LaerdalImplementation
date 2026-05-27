using System;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Command that soft-deletes an organization by marking it inactive.
/// The organization record is retained in the database to preserve audit trails
/// and foreign key relationships with manifests.
/// </summary>
/// <param name="Id">Primary key of the organization to delete.</param>
public record DeleteOrganizationCommand(Guid Id) : IRequest;
