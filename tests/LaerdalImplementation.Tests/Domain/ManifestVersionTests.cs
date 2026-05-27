using System;
using LaerdalImplementation.Domain.ValueObjects;
using Xunit;

namespace LaerdalImplementation.Tests.Domain;

public class ManifestVersionTests
{
    [Fact]
    public void Parse_ValidString_ReturnsParsedVersion()
    {
        var version = ManifestVersion.Parse("1.2.3");

        Assert.Equal(1, version.Major);
        Assert.Equal(2, version.Minor);
        Assert.Equal(3, version.Patch);
    }

    [Theory]
    [InlineData("1.2")]
    [InlineData("1.2.3.4")]
    [InlineData("a.b.c")]
    [InlineData("")]
    public void Parse_InvalidFormat_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(() => ManifestVersion.Parse(input));
    }

    [Fact]
    public void BumpMajor_IncreasesMajorAndResetsMinorAndPatch()
    {
        var version = ManifestVersion.Parse("1.2.3");

        var bumped = version.BumpMajor();

        Assert.Equal(2, bumped.Major);
        Assert.Equal(0, bumped.Minor);
        Assert.Equal(0, bumped.Patch);
    }

    [Fact]
    public void BumpMinor_IncreasesMinorAndResetsPatch()
    {
        var version = ManifestVersion.Parse("1.2.3");

        var bumped = version.BumpMinor();

        Assert.Equal(1, bumped.Major);
        Assert.Equal(3, bumped.Minor);
        Assert.Equal(0, bumped.Patch);
    }

    [Fact]
    public void BumpPatch_IncreasesOnlyPatch()
    {
        var version = ManifestVersion.Parse("1.2.3");

        var bumped = version.BumpPatch();

        Assert.Equal(1, bumped.Major);
        Assert.Equal(2, bumped.Minor);
        Assert.Equal(4, bumped.Patch);
    }

    [Fact]
    public void Constructor_NegativeComponent_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ManifestVersion(-1, 0, 0));
        Assert.Throws<ArgumentException>(() => new ManifestVersion(0, -1, 0));
        Assert.Throws<ArgumentException>(() => new ManifestVersion(0, 0, -1));
    }

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        var version = new ManifestVersion(1, 2, 3);

        Assert.Equal("1.2.3", version.ToString());
    }
}
