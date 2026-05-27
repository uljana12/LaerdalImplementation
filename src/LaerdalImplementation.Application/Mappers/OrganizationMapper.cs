using System;
using System.Collections.Generic;
using System.Linq;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Domain.Entities;

namespace LaerdalImplementation.Application.Mappers;

/// <summary>
/// Converts between <see cref="Organization"/> domain entities and the DTOs used
/// by the Application layer. Keeping mapping logic in one place means the controller,
/// command handler, and query handler all produce consistent output shapes.
/// </summary>
public static class OrganizationMapper
{
    /// <summary>
    /// Maps a domain entity to a read DTO. Recursively maps each child organization
    /// so the caller receives the full subtree in one object.
    /// </summary>
    /// <param name="organization">The entity loaded from the repository.</param>
    /// <returns>An <see cref="OrganizationDto"/> with nested children.</returns>
    public static OrganizationDto ToDto(Organization organization)
    {
        return new OrganizationDto
        {
            Id = organization.Id,
            ParentId = organization.ParentId,
            Name = organization.Name,
            Code = organization.Code,
            Type = organization.Type,
            ExternalId = organization.ExternalId,
            IsActive = organization.IsActive,
            CreatedAt = organization.CreatedAt,
            UpdatedAt = organization.UpdatedAt,
            Children = organization.Children.Select(ToDto).ToList()
        };
    }

    /// <summary>
    /// Creates a new <see cref="Organization"/> entity from a create DTO by delegating
    /// to the domain factory method, which enforces validation invariants.
    /// </summary>
    /// <param name="dto">The input data, typically sourced from the HTTP request.</param>
    /// <param name="parentId">
    /// Explicit parent ID to use; falls back to <see cref="CreateOrganizationDto.ParentId"/>
    /// if not provided.
    /// </param>
    /// <returns>A valid, unsaved <see cref="Organization"/> entity.</returns>
    public static Organization ToEntity(CreateOrganizationDto dto, Guid? parentId = null)
    {
        return Organization.Create(
            dto.Name,
            dto.Code,
            dto.Type,
            parentId ?? dto.ParentId,
            dto.ExternalId
        );
    }
}
