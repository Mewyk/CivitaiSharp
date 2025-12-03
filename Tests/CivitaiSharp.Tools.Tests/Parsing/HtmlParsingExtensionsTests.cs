namespace CivitaiSharp.Tools.Tests.Parsing;

using CivitaiSharp.Core.Models;
using CivitaiSharp.Tools.Parsing;
using Xunit;

public sealed class HtmlParsingExtensionsTests
{
    [Fact]
    public void WhenCallingGetDescriptionAsMarkdownOnModelThenConvertsHtml()
    {
        // Arrange
        var model = CreateTestModel("<p>Test <strong>description</strong></p>");

        // Act
        var result = model.GetDescriptionAsMarkdown();

        // Assert
        Assert.Contains("**description**", result);
    }

    [Fact]
    public void WhenCallingGetDescriptionAsMarkdownOnModelWithNullDescriptionThenReturnsEmpty()
    {
        // Arrange
        var model = CreateTestModel(null);

        // Act
        var result = model.GetDescriptionAsMarkdown();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void WhenCallingGetDescriptionAsMarkdownWithNullModelThenThrowsArgumentNullException()
    {
        // Arrange
        Model? model = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => model!.GetDescriptionAsMarkdown());
    }

    [Fact]
    public void WhenCallingGetDescriptionAsPlainTextOnModelThenStripsHtml()
    {
        // Arrange
        var model = CreateTestModel("<p>Test <strong>description</strong></p>");

        // Act
        var result = model.GetDescriptionAsPlainText();

        // Assert
        Assert.Contains("Test description", result);
        Assert.DoesNotContain("<p>", result);
        Assert.DoesNotContain("<strong>", result);
    }

    [Fact]
    public void WhenCallingGetDescriptionAsPlainTextOnModelWithNullDescriptionThenReturnsEmpty()
    {
        // Arrange
        var model = CreateTestModel(null);

        // Act
        var result = model.GetDescriptionAsPlainText();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void WhenCallingGetDescriptionAsPlainTextWithNullModelThenThrowsArgumentNullException()
    {
        // Arrange
        Model? model = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => model!.GetDescriptionAsPlainText());
    }

    [Fact]
    public void WhenCallingGetDescriptionAsMarkdownOnModelVersionThenConvertsHtml()
    {
        // Arrange
        var version = CreateTestModelVersion("<h2>Version Notes</h2><p>Changes in this version.</p>");

        // Act
        var result = version.GetDescriptionAsMarkdown();

        // Assert
        Assert.Contains("## Version Notes", result);
        Assert.Contains("Changes in this version.", result);
    }

    [Fact]
    public void WhenCallingGetDescriptionAsMarkdownOnModelVersionWithNullDescriptionThenReturnsEmpty()
    {
        // Arrange
        var version = CreateTestModelVersion(null);

        // Act
        var result = version.GetDescriptionAsMarkdown();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void WhenCallingGetDescriptionAsMarkdownWithNullModelVersionThenThrowsArgumentNullException()
    {
        // Arrange
        ModelVersion? version = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => version!.GetDescriptionAsMarkdown());
    }

    [Fact]
    public void WhenCallingGetDescriptionAsPlainTextOnModelVersionThenStripsHtml()
    {
        // Arrange
        var version = CreateTestModelVersion("<p>Version <em>notes</em> here.</p>");

        // Act
        var result = version.GetDescriptionAsPlainText();

        // Assert
        Assert.Contains("Version notes here.", result);
        Assert.DoesNotContain("<p>", result);
        Assert.DoesNotContain("<em>", result);
    }

    [Fact]
    public void WhenCallingGetDescriptionAsPlainTextOnModelVersionWithNullDescriptionThenReturnsEmpty()
    {
        // Arrange
        var version = CreateTestModelVersion(null);

        // Act
        var result = version.GetDescriptionAsPlainText();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void WhenCallingGetDescriptionAsPlainTextWithNullModelVersionThenThrowsArgumentNullException()
    {
        // Arrange
        ModelVersion? version = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => version!.GetDescriptionAsPlainText());
    }

    private static Model CreateTestModel(string? description)
    {
        return new Model(
            Id: 1,
            Name: "Test Model",
            Description: description,
            Type: ModelType.Checkpoint,
            IsNsfw: false,
            NsfwLevel: 0,
            Tags: null,
            Creator: null,
            Stats: null,
            ModelVersions: null,
            AllowNoCredit: true,
            AllowDerivatives: true,
            AllowDifferentLicense: false,
            AllowCommercialUse: null,
            IsPersonOfInterest: false,
            Minor: false,
            IsSafeForWorkOnly: true,
            Availability: null,
            Cosmetic: null,
            SupportsGeneration: false,
            UserId: null,
            DownloadUrl: null,
            Mode: null);
    }

    private static ModelVersion CreateTestModelVersion(string? description)
    {
        return new ModelVersion(
            Id: 1,
            Index: null,
            ModelId: null,
            Name: "v1.0",
            BaseModel: "SDXL 1.0",
            BaseModelType: null,
            Description: description,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: null,
            PublishedAt: null,
            Status: null,
            Availability: null,
            NsfwLevel: 0,
            DownloadUrl: null,
            SupportsGeneration: false,
            TrainedWords: null,
            TrainingStatus: null,
            TrainingDetails: null,
            EarlyAccessEndsAt: null,
            EarlyAccessConfig: null,
            UploadType: null,
            UsageControl: null,
            AirIdentifier: null,
            Model: null,
            Files: null,
            Images: null,
            Stats: null);
    }
}
