using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.Commands;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Enums;
using LaerdalImplementation.Domain.Repositories;
using Moq;
using Xunit;

namespace LaerdalImplementation.Tests.Application.Commands;

public class DeleteOrganizationCommandHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repo = new();
    private readonly DeleteOrganizationCommandHandler _handler;

    public DeleteOrganizationCommandHandlerTests()
    {
        _handler = new DeleteOrganizationCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ExistingOrgWithNoChildren_SetsInactive()
    {
        var org = Organization.Create("Hospital", "HOSP", OrganizationType.Hospital);
        var command = new DeleteOrganizationCommand(org.Id);

        _repo.Setup(r => r.GetByIdAsync(org.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(org);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization o, CancellationToken _) => o);

        await _handler.Handle(command, CancellationToken.None);

        Assert.False(org.IsActive);
        _repo.Verify(r => r.UpdateAsync(org, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentOrganization_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var command = new DeleteOrganizationCommand(id);

        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_OrgWithActiveChildren_ThrowsInvalidOperationException()
    {
        var org = Organization.Create("Hospital", "HOSP", OrganizationType.Hospital);
        org.Children.Add(Organization.Create("Cardiology", "CARD", OrganizationType.Department));
        var command = new DeleteOrganizationCommand(org.Id);

        _repo.Setup(r => r.GetByIdAsync(org.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(org);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
