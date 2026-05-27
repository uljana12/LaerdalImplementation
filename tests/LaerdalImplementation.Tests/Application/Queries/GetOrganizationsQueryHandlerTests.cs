using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.Queries;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Enums;
using LaerdalImplementation.Domain.Repositories;
using Moq;
using Xunit;

namespace LaerdalImplementation.Tests.Application.Queries;

public class GetOrganizationsQueryHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repo = new();
    private readonly GetOrganizationsQueryHandler _handler;

    public GetOrganizationsQueryHandlerTests()
    {
        _handler = new GetOrganizationsQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_WithoutParentId_CallsGetAll()
    {
        var orgs = new List<Organization>
        {
            Organization.Create("Hospital A", "HOSPA", OrganizationType.Hospital),
            Organization.Create("Hospital B", "HOSPB", OrganizationType.Hospital)
        };
        var query = new GetOrganizationsQuery(null);

        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(orgs);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count());
        _repo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.GetByParentIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithParentId_CallsGetByParentId()
    {
        var parentId = Guid.NewGuid();
        var children = new List<Organization>
        {
            Organization.Create("Cardiology", "CARD", OrganizationType.Department, parentId)
        };
        var query = new GetOrganizationsQuery(parentId);

        _repo.Setup(r => r.GetByParentIdAsync(parentId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(children);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        _repo.Verify(r => r.GetByParentIdAsync(parentId, It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
