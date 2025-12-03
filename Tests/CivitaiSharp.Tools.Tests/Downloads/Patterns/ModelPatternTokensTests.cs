namespace CivitaiSharp.Tools.Tests.Downloads.Patterns;

using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Models.Common;
using CivitaiSharp.Tools.Downloads.Patterns;
using Xunit;

public sealed class ModelPatternTokensTests
{
    [Fact]
    public void WhenExtractingTokenValuesFromModelFileThenAllTokensArePresent()
    {
        // Arrange
        var file = new ModelFile(
            Id: 123456,
            SizeKilobytes: 4096000,
            Name: "amazing_model_v1.safetensors",
            Type: "Model",
            PickleScanResult: null,
            PickleScanMessage: null,
            VirusScanResult: null,
            VirusScanMessage: null,
            ScannedAt: null,
            Metadata: new FileMetadata("SafeTensor", "full", "fp16"),
            Hashes: null,
            DownloadUrl: "https://example.com/download/123456",
            Primary: true);

        // Act
        var tokens = ModelPatternTokens.ExtractTokenValues(file);

        // Assert
        Assert.Equal("123456", tokens["FileId"]);
        Assert.Equal("amazing_model_v1.safetensors", tokens["FileName"]);
        Assert.Equal("Model", tokens["FileType"]);
        Assert.Equal("SafeTensor", tokens["Format"]);
        Assert.Equal("full", tokens["Size"]);
        Assert.Equal("fp16", tokens["Precision"]);
    }

    [Fact]
    public void WhenExtractingTokenValuesWithNullMetadataThenFallbacksAreUsed()
    {
        // Arrange
        var file = new ModelFile(
            Id: 123456,
            SizeKilobytes: 4096000,
            Name: "model.bin",
            Type: "Model",
            PickleScanResult: null,
            PickleScanMessage: null,
            VirusScanResult: null,
            VirusScanMessage: null,
            ScannedAt: null,
            Metadata: null,
            Hashes: null,
            DownloadUrl: null,
            Primary: null);

        // Act
        var tokens = ModelPatternTokens.ExtractTokenValues(file);

        // Assert
        Assert.Equal("123456", tokens["FileId"]);
        Assert.Equal("model.bin", tokens["FileName"]);
        Assert.Equal("Model", tokens["FileType"]);
        Assert.Equal("unknown", tokens["Format"]);
        Assert.Equal("unknown", tokens["Size"]);
        Assert.Equal("unknown", tokens["Precision"]);
    }

    [Fact]
    public void WhenExtractingTokenValuesWithVersionThenVersionTokensAreIncluded()
    {
        // Arrange
        var file = new ModelFile(
            Id: 123456,
            SizeKilobytes: 4096000,
            Name: "model.safetensors",
            Type: "Model",
            PickleScanResult: null,
            PickleScanMessage: null,
            VirusScanResult: null,
            VirusScanMessage: null,
            ScannedAt: null,
            Metadata: new FileMetadata("SafeTensor", "full", "fp16"),
            Hashes: null,
            DownloadUrl: "https://example.com/download",
            Primary: true);

        var version = new ModelVersion(
            Id: 789012,
            Index: 1,
            ModelId: 555555,
            Name: "v1.0",
            BaseModel: "SDXL 1.0",
            BaseModelType: null,
            Description: null,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: null,
            PublishedAt: null,
            Status: "Published",
            Availability: null,
            NsfwLevel: 1,
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
            Model: new ModelVersionModel("Amazing Model", ModelType.Checkpoint, false, false),
            Files: null,
            Images: null,
            Stats: null);

        // Act
        var tokens = ModelPatternTokens.ExtractTokenValues(file, version);

        // Assert
        // File tokens
        Assert.Equal("123456", tokens["FileId"]);
        Assert.Equal("model.safetensors", tokens["FileName"]);

        // Version tokens
        Assert.Equal("789012", tokens["VersionId"]);
        Assert.Equal("v1.0", tokens["VersionName"]);
        Assert.Equal("SDXL 1.0", tokens["BaseModel"]);

        // Model tokens
        Assert.Equal("555555", tokens["ModelId"]);
        Assert.Equal("Amazing Model", tokens["ModelName"]);
        Assert.Equal("Checkpoint", tokens["ModelType"]);
    }

    [Fact]
    public void WhenExtractingTokenValuesWithVersionButNoModelThenFallbacksAreUsed()
    {
        // Arrange
        var file = new ModelFile(
            Id: 123456,
            SizeKilobytes: 4096000,
            Name: "model.safetensors",
            Type: "Model",
            PickleScanResult: null,
            PickleScanMessage: null,
            VirusScanResult: null,
            VirusScanMessage: null,
            ScannedAt: null,
            Metadata: null,
            Hashes: null,
            DownloadUrl: null,
            Primary: null);

        var version = new ModelVersion(
            Id: 789012,
            Index: null,
            ModelId: 555555,
            Name: "v1.0",
            BaseModel: "SDXL 1.0",
            BaseModelType: null,
            Description: null,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: null,
            PublishedAt: null,
            Status: null,
            Availability: null,
            NsfwLevel: 1,
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

        // Act
        var tokens = ModelPatternTokens.ExtractTokenValues(file, version);

        // Assert
        Assert.Equal("555555", tokens["ModelId"]);
        Assert.Equal("unknown", tokens["ModelName"]);
        Assert.Equal("unknown", tokens["ModelType"]);
    }

    [Fact]
    public void WhenFileOnlyTokensAreRequestedThenCorrectSubsetIsReturned()
    {
        // Act
        var fileOnlyTokens = (IEnumerable<string>)ModelPatternTokens.FileOnlyTokens;

        // Assert
        Assert.Contains("FileId", fileOnlyTokens);
        Assert.Contains("FileName", fileOnlyTokens);
        Assert.Contains("FileType", fileOnlyTokens);
        Assert.Contains("Format", fileOnlyTokens);
        Assert.Contains("Size", fileOnlyTokens);
        Assert.Contains("Precision", fileOnlyTokens);
        Assert.DoesNotContain("VersionId", fileOnlyTokens);
        Assert.DoesNotContain("ModelId", fileOnlyTokens);
    }

    [Fact]
    public void WhenAllTokensAreRequestedThenAllTokensArePresent()
    {
        // Act
        var allTokens = (IEnumerable<string>)ModelPatternTokens.AllTokens;

        // Assert
        // File tokens
        Assert.Contains("FileId", allTokens);
        Assert.Contains("FileName", allTokens);

        // Version tokens
        Assert.Contains("VersionId", allTokens);
        Assert.Contains("VersionName", allTokens);
        Assert.Contains("BaseModel", allTokens);

        // Model tokens
        Assert.Contains("ModelId", allTokens);
        Assert.Contains("ModelName", allTokens);
        Assert.Contains("ModelType", allTokens);
    }

    [Fact]
    public void WhenGettingValidTokensWithVersionThenAllTokensAreReturned()
    {
        // Act
        var tokens = ModelPatternTokens.GetValidTokens(hasVersion: true);

        // Assert
        Assert.Equal(ModelPatternTokens.AllTokens, tokens);
    }

    [Fact]
    public void WhenGettingValidTokensWithoutVersionThenFileOnlyTokensAreReturned()
    {
        // Act
        var tokens = ModelPatternTokens.GetValidTokens(hasVersion: false);

        // Assert
        Assert.Equal(ModelPatternTokens.FileOnlyTokens, tokens);
    }
}
