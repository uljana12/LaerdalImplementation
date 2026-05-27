using System;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Domain.Enums;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Command that requests a partial update of an existing organization's mutable fields.
/// Only name, type, and active status can change — the code and hierarchy are immutable
/// after creation to preserve referential integrity.
/// </summary>
/// <param name="Id">Primary key of the organization to update.</param>
/// <param name="Name">New display name.</param>
/// <param name="Type">New organization type.</param>
/// <param name="IsActive">Whether the organization should remain active.</param>
public record UpdateOrganizationCommand(
    Guid Id,
    string Name,
    OrganizationType Type,
    bool IsActive) : IRequest<OrganizationDto>;
