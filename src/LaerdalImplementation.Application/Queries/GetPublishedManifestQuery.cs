using LaerdalImplementation.Application.DTOs;
using MediatR;

namespace LaerdalImplementation.Application.Queries;

/// <summary>
/// Query used by the downstream training application to fetch the currently
/// published manifest for an organization before starting a learner session.
/// Identified by organization code (not ID) so the training app doesn't need
/// to store internal GUIDs — the code is stable and human-readable.
/// </summary>
/// <param name="OrgCode">The organization's short identifier code (case-insensitive).</param>
public record GetPublishedManifestQuery(string OrgCode) : IRequest<ManifestDto?>;
