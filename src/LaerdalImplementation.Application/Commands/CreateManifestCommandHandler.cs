using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Application.Mappers;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Repositories;
using MediatR;

namespace LaerdalImplementation.Application.Commands;

/// <summary>
/// Handles <see cref="CreateManifestCommand"/>.
/// Creates a new draft manifest for the specified organization and persists it.
/// </summary>
public class CreateManifestCommandHandler : IRequestHandler<CreateManifestCommand, ManifestDto>
{
    private readonly IManifestRepository _manifestRepository;

    public CreateManifestCommandHandler(IManifestRepository manifestRepository)
    {
        _manifestRepository = manifestRepository;
    }

    public async Task<ManifestDto> Handle(CreateManifestCommand request, CancellationToken cancellationToken)
    {
        var manifest = Manifest.CreateDraft(
            request.OrganizationId,
            request.Name,
            request.Description,
            request.Content);

        var saved = await _manifestRepository.AddAsync(manifest, cancellationToken);
        return ManifestMapper.ToDto(saved);
    }
}
