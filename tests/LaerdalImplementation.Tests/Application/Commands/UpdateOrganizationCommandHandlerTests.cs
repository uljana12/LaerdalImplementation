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

public class UpdateOrganizationCommandHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repo = new();
    private readonly UpdateOrganizationCommandHandler _handler;

    public UpdateOrganizationCommandHandlerTests()
    {
        _handler = new UpdateOrganizationCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ExistingOrganization_ReturnsUpdatedDto()
    {
        var org = Organization.Create("Old Name", "HOSP", OrganizationType.Hospital);
        var command = new UpdateOrganizationCommand(org.Id, "New Name", OrganizationType.TrainingCenter, false);

        _repo.Setup(r => r.GetByIdAsync(org.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(org);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization o, CancellationToken _) => o);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("New Name", result.Name);
        Assert.Equal(OrganizationType.TrainingCenter, result.Type);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task Handle_NonExistentOrganization_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var command = new UpdateOrganizationCommand(id, "Name", OrganizationType.Hospital, true);

        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
