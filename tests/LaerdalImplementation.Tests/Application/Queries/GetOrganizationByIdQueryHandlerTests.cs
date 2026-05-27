using System;
using System.Threading;
using System.Threading.Tasks;
using LaerdalImplementation.Application.Queries;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Enums;
using LaerdalImplementation.Domain.Repositories;
using Moq;
using Xunit;

namespace LaerdalImplementation.Tests.Application.Queries;

public class GetOrganizationByIdQueryHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repo = new();
    private readonly GetOrganizationByIdQueryHandler _handler;

    public GetOrganizationByIdQueryHandlerTests()
    {
        _handler = new GetOrganizationByIdQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ExistingOrganization_ReturnsDto()
    {
        var org = Organization.Create("City Hospital", "CITY", OrganizationType.Hospital);
        var query = new GetOrganizationByIdQuery(org.Id);

        _repo.Setup(r => r.GetByIdAsync(org.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(org);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(org.Id, result.Id);
        Assert.Equal("City Hospital", result.Name);
    }

    [Fact]
    public async Task Handle_NonExistentOrganization_ReturnsNull()
    {
        var id = Guid.NewGuid();
        var query = new GetOrganizationByIdQuery(id);

        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
             .ReturnsAsync((Organization?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}
