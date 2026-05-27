using System;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Enums;
using Xunit;

namespace LaerdalImplementation.Tests.Domain;

public class OrganizationTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsOrganization()
    {
        var org = Organization.Create("City Hospital", "CITY", OrganizationType.Hospital);

        Assert.Equal("City Hospital", org.Name);
        Assert.Equal("CITY", org.Code);
        Assert.Equal(OrganizationType.Hospital, org.Type);
        Assert.True(org.IsActive);
        Assert.NotEqual(Guid.Empty, org.Id);
    }

    [Fact]
    public void Create_WithParentId_SetsParentId()
    {
        var parentId = Guid.NewGuid();

        var org = Organization.Create("Cardiology", "CARD", OrganizationType.Department, parentId);

        Assert.Equal(parentId, org.ParentId);
    }

    [Fact]
    public void Create_Code_StoredAsUppercase()
    {
        var org = Organization.Create("City Hospital", "city", OrganizationType.Hospital);

        Assert.Equal("CITY", org.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankName_ThrowsArgumentException(string name)
    {
        Assert.Throws<ArgumentException>(() =>
            Organization.Create(name, "CODE", OrganizationType.Hospital));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankCode_ThrowsArgumentException(string code)
    {
        Assert.Throws<ArgumentException>(() =>
            Organization.Create("Name", code, OrganizationType.Hospital));
    }

    [Fact]
    public void Update_WithValidInputs_UpdatesFields()
    {
        var org = Organization.Create("Old Name", "CODE", OrganizationType.Hospital);
        var beforeUpdate = org.UpdatedAt;

        org.Update("New Name", OrganizationType.TrainingCenter, false);

        Assert.Equal("New Name", org.Name);
        Assert.Equal(OrganizationType.TrainingCenter, org.Type);
        Assert.False(org.IsActive);
        Assert.True(org.UpdatedAt >= beforeUpdate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithBlankName_ThrowsArgumentException(string name)
    {
        var org = Organization.Create("Name", "CODE", OrganizationType.Hospital);

        Assert.Throws<ArgumentException>(() => org.Update(name, OrganizationType.Hospital, true));
    }

    [Fact]
    public void CanBeDeleted_WithNoChildren_ReturnsTrue()
    {
        var org = Organization.Create("Hospital", "HOSP", OrganizationType.Hospital);

        Assert.True(org.CanBeDeleted());
    }

    [Fact]
    public void CanBeDeleted_WithActiveChildren_ReturnsFalse()
    {
        var org = Organization.Create("Hospital", "HOSP", OrganizationType.Hospital);
        org.Children.Add(Organization.Create("Cardiology", "CARD", OrganizationType.Department));

        Assert.False(org.CanBeDeleted());
    }

    [Fact]
    public void CanBeDeleted_WithOnlyInactiveChildren_ReturnsTrue()
    {
        var org = Organization.Create("Hospital", "HOSP", OrganizationType.Hospital);
        var child = Organization.Create("Cardiology", "CARD", OrganizationType.Department);
        child.Update(child.Name, child.Type, isActive: false);
        org.Children.Add(child);

        Assert.True(org.CanBeDeleted());
    }
}
