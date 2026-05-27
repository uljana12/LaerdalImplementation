using System;
using LaerdalImplementation.Domain.Entities;
using LaerdalImplementation.Domain.Enums;
using Xunit;

namespace LaerdalImplementation.Tests.Domain;

public class ManifestTests
{
    private static readonly Guid OrgId = Guid.NewGuid();

    [Fact]
    public void CreateDraft_WithValidInputs_ReturnsDraftManifest()
    {
        var manifest = Manifest.CreateDraft(OrgId, "2026 Curriculum");

        Assert.Equal(OrgId, manifest.OrganizationId);
        Assert.Equal("2026 Curriculum", manifest.Name);
        Assert.Equal(ManifestStatus.Draft, manifest.Status);
        Assert.NotEqual(Guid.Empty, manifest.Id);
        Assert.Null(manifest.PublishedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateDraft_WithBlankName_ThrowsArgumentException(string name)
    {
        Assert.Throws<ArgumentException>(() => Manifest.CreateDraft(OrgId, name));
    }

    [Fact]
    public void CreateDraft_DefaultsToEmptyJsonContent()
    {
        var manifest = Manifest.CreateDraft(OrgId, "Curriculum");

        Assert.Equal("{}", manifest.Content);
    }

    [Fact]
    public void Publish_FromDraft_SetsPublishedStatus()
    {
        var manifest = Manifest.CreateDraft(OrgId, "Curriculum");

        manifest.Publish("1.0.0");

        Assert.Equal(ManifestStatus.Published, manifest.Status);
    }

    [Fact]
    public void Publish_AssignsVersion()
    {
        var manifest = Manifest.CreateDraft(OrgId, "Curriculum");

        manifest.Publish("2.1.0");

        Assert.Equal("2.1.0", manifest.Version);
    }

    [Fact]
    public void Publish_SetsPublishedAt()
    {
        var manifest = Manifest.CreateDraft(OrgId, "Curriculum");
        var before = DateTime.UtcNow;

        manifest.Publish("1.0.0");

        Assert.NotNull(manifest.PublishedAt);
        Assert.True(manifest.PublishedAt >= before);
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_ThrowsInvalidOperationException()
    {
        var manifest = Manifest.CreateDraft(OrgId, "Curriculum");
        manifest.Publish("1.0.0");

        Assert.Throws<InvalidOperationException>(() => manifest.Publish("2.0.0"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Publish_WithBlankVersion_ThrowsArgumentException(string version)
    {
        var manifest = Manifest.CreateDraft(OrgId, "Curriculum");

        Assert.Throws<ArgumentException>(() => manifest.Publish(version));
    }

    [Fact]
    public void Archive_SetsArchivedStatus()
    {
        var manifest = Manifest.CreateDraft(OrgId, "Curriculum");
        manifest.Publish("1.0.0");

        manifest.Archive();

        Assert.Equal(ManifestStatus.Archived, manifest.Status);
    }

    [Fact]
    public void Archive_WhenAlreadyArchived_ThrowsInvalidOperationException()
    {
        var manifest = Manifest.CreateDraft(OrgId, "Curriculum");
        manifest.Publish("1.0.0");
        manifest.Archive();

        Assert.Throws<InvalidOperationException>(() => manifest.Archive());
    }
}
