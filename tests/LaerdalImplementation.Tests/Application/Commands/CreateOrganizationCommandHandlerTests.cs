using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.Commands;
using LaerdalImplementation.Application.DTOs;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Enums;
using LaerdalImplementation.Domain.Repositories;
using Moq;
using Xunit;

namespace LaerdalImplementation.Tests.Application.Commands;

public class CreateOrganizationCommandHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repo = new();
    private readonly CreateOrganizationCommandHandler _handler;

    public CreateOrganizationCommandHandlerTests()
    {
        _handler = new CreateOrganizationCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ValidRootOrg_ReturnsOrganizationDto()
    {
        var dto = new CreateOrganizationDto { Name = "City Hospital", Code = "CITY", Type = OrganizationType.Hospital };
        var command = new CreateOrganizationCommand(dto);

        _repo.Setup(r => r.GetByCodeAsync("CITY", null, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization?)null);
        _repo.Setup(r => r.AddAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization org, CancellationToken _) => org);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("City Hospital", result.Name);
        Assert.Equal("CITY", result.Code);
    }

    [Fact]
    public async Task Handle_DuplicateCodeUnderSameParent_ThrowsInvalidOperationException()
    {
        var dto = new CreateOrganizationDto { Name = "Hospital", Code = "DUPE", Type = OrganizationType.Hospital };
        var command = new CreateOrganizationCommand(dto);
        var existing = Organization.Create("Other", "DUPE", OrganizationType.Hospital);

        _repo.Setup(r => r.GetByCodeAsync("DUPE", null, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CodeOfSoftDeletedOrg_Succeeds()
    {
        var dto = new CreateOrganizationDto { Name = "Hospital", Code = "REUSE", Type = OrganizationType.Hospital };
        var command = new CreateOrganizationCommand(dto);

        // Repo returns null — simulates GetByCodeAsync filtering out inactive orgs
        _repo.Setup(r => r.GetByCodeAsync("REUSE", null, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization?)null);
        _repo.Setup(r => r.AddAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization org, CancellationToken _) => org);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("REUSE", result.Code);
    }

    [Fact]
    public async Task Handle_NonExistentParent_ThrowsInvalidOperationException()
    {
        var parentId = Guid.NewGuid();
        var dto = new CreateOrganizationDto { Name = "Dept", Code = "DEPT", Type = OrganizationType.Department, ParentId = parentId };
        var command = new CreateOrganizationCommand(dto);

        _repo.Setup(r => r.GetByCodeAsync("DEPT", parentId, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization?)null);
        _repo.Setup(r => r.ExistsAsync(parentId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidChildOrg_DoesNotCheckParentExistence_WhenParentIdIsNull()
    {
        var dto = new CreateOrganizationDto { Name = "Root Org", Code = "ROOT", Type = OrganizationType.Hospital };
        var command = new CreateOrganizationCommand(dto);

        _repo.Setup(r => r.GetByCodeAsync("ROOT", null, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization?)null);
        _repo.Setup(r => r.AddAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization org, CancellationToken _) => org);

        await _handler.Handle(command, CancellationToken.None);

        _repo.Verify(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
