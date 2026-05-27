using System;
using LaerdalImplementation.Application.DTOs;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Command that transitions a draft manifest to Published status and archives
/// the previously published manifest for the same organization (if one exists).
/// </summary>
/// <param name="OrganizationId">The owning organization's ID.</param>
/// <param name="ManifestId">The draft manifest to publish.</param>
/// <param name="VersionBump">
/// Which version component to increment: "major", "minor", or "patch".
/// The new version is calculated from the current highest version for this org.
/// </param>
public record PublishManifestCommand(
    Guid OrganizationId,
    Guid ManifestId,
    string VersionBump) : IRequest<ManifestDto>;
