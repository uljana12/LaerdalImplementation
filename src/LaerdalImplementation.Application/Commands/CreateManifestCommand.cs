using System;
using LaerdalImplementation.Application.DTOs;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Command that creates a new manifest in Draft status for the specified organization.
/// </summary>
/// <param name="OrganizationId">The owning organization's ID.</param>
/// <param name="Name">Display name for the manifest.</param>
/// <param name="Description">Optional human-readable description.</param>
/// <param name="Content">Optional initial JSON content blob.</param>
public record CreateManifestCommand(
    Guid OrganizationId,
    string Name,
    string? Description,
    string? Content) : IRequest<ManifestDto>;
