namespace CivitaiSharp.Tools.Tests.Downloads.Patterns;

using CivitaiSharp.Core.Models;
using CivitaiSharp.Tools.Downloads.Patterns;
using Xunit;

public sealed class ImagePatternTokensTests
{
    [Fact]
    public void WhenExtractingTokenValuesFromImageThenAllTokensArePresent()
    {
        // Arrange
        var image = new Image(
            Id: 12345678,
            Url: "https://example.com/image.png",
            Hash: "abcd1234",
            Width: 1920,
            Height: 1080,
            NsfwLevel: ImageNsfwLevel.None,
            Type: MediaType.Image,
            IsNsfw: false,
            BrowsingLevel: 1,
            CreatedAt: new DateTime(2024, 1, 15),
            PostId: 987654,
            Stats: null,
            Meta: null,
            Username: "TestUser",
            BaseModel: "SDXL 1.0",
            ModelVersionIds: null);

        // Act
        var tokens = ImagePatternTokens.ExtractTokenValues(image, "png");

        // Assert
        Assert.Equal("12345678", tokens["Id"]);
        Assert.Equal("987654", tokens["PostId"]);
        Assert.Equal("TestUser", tokens["Username"]);
        Assert.Equal("1920", tokens["Width"]);
        Assert.Equal("1080", tokens["Height"]);
        Assert.Equal("SDXL 1.0", tokens["BaseModel"]);
        Assert.Equal("None", tokens["NsfwLevel"]);
        Assert.Equal("2024-01-15", tokens["Date"]);
        Assert.Equal("png", tokens["Extension"]);
    }

    [Fact]
    public void WhenExtractingTokenValuesWithNullFieldsThenFallbacksAreUsed()
    {
        // Arrange
        var image = new Image(
            Id: 12345678,
            Url: "https://example.com/image.png",
            Hash: null,
            Width: 800,
            Height: 600,
            NsfwLevel: null,
            Type: null,
            IsNsfw: null,
            BrowsingLevel: null,
            CreatedAt: null,
            PostId: null,
            Stats: null,
            Meta: null,
            Username: null,
            BaseModel: null,
            ModelVersionIds: null);

        // Act
        var tokens = ImagePatternTokens.ExtractTokenValues(image, "jpg");

        // Assert
        Assert.Equal("12345678", tokens["Id"]);
        Assert.Equal("unknown", tokens["PostId"]);
        Assert.Equal("unknown", tokens["Username"]);
        Assert.Equal("unknown", tokens["BaseModel"]);
        Assert.Equal("None", tokens["NsfwLevel"]);
        Assert.Equal("jpg", tokens["Extension"]);
    }

    [Fact]
    public void WhenInferringExtensionFromUrlThenCorrectExtensionIsReturned()
    {
        // Arrange
        var image = new Image(
            Id: 12345678,
            Url: "https://example.com/images/12345678.webp?width=100",
            Hash: null,
            Width: 800,
            Height: 600,
            NsfwLevel: null,
            Type: null,
            IsNsfw: null,
            BrowsingLevel: null,
            CreatedAt: null,
            PostId: null,
            Stats: null,
            Meta: null,
            Username: null,
            BaseModel: null,
            ModelVersionIds: null);

        // Act
        var extension = ImagePatternTokens.InferExtension(image);

        // Assert
        Assert.Equal("webp", extension);
    }

    [Fact]
    public void WhenInferringExtensionFromVideoTypeThenMp4IsReturned()
    {
        // Arrange
        var image = new Image(
            Id: 12345678,
            Url: "https://example.com/video/12345678",
            Hash: null,
            Width: 800,
            Height: 600,
            NsfwLevel: null,
            Type: MediaType.Video,
            IsNsfw: null,
            BrowsingLevel: null,
            CreatedAt: null,
            PostId: null,
            Stats: null,
            Meta: null,
            Username: null,
            BaseModel: null,
            ModelVersionIds: null);

        // Act
        var extension = ImagePatternTokens.InferExtension(image);

        // Assert
        Assert.Equal("mp4", extension);
    }

    [Fact]
    public void WhenValidTokensAreRequestedThenAllExpectedTokensArePresent()
    {
        // Act
        var validTokens = (IEnumerable<string>)ImagePatternTokens.ValidTokens;

        // Assert
        Assert.Contains("Id", validTokens);
        Assert.Contains("PostId", validTokens);
        Assert.Contains("Username", validTokens);
        Assert.Contains("Width", validTokens);
        Assert.Contains("Height", validTokens);
        Assert.Contains("BaseModel", validTokens);
        Assert.Contains("NsfwLevel", validTokens);
        Assert.Contains("Date", validTokens);
        Assert.Contains("Extension", validTokens);
    }
}
