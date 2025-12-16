namespace CivitaiSharp.Sdk.Tests.Air;

using System;
using CivitaiSharp.Sdk.Air;
using Xunit;

public sealed class AirBuilderTests : IClassFixture<SdkTestFixture>
{
    #region Successful Build Tests

    [Fact]
    public void WhenAllRequiredPropertiesAreSetThenBuildSucceeds()
    {
        // Arrange
        var builder = new AirBuilder();

        // Act
        var result = builder
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(328553)
            .WithVersionId(368189)
            .Build();

        // Assert
        Assert.Equal(AirEcosystem.StableDiffusionXl, result.Ecosystem);
        Assert.Equal(AirAssetType.Lora, result.AssetType);
        Assert.Equal(AirSource.Civitai, result.Source);
        Assert.Equal(328553, result.ModelId);
        Assert.Equal(368189, result.VersionId);
    }

    [Fact]
    public void WhenBuildingWithDefaultSourceThenSourceIsCivitai()
    {
        // Arrange & Act
        var result = new AirBuilder()
            .WithEcosystem(AirEcosystem.Flux1)
            .WithAssetType(AirAssetType.Checkpoint)
            .WithModelId(12345)
            .WithVersionId(67890)
            .Build();

        // Assert
        Assert.Equal(AirSource.Civitai, result.Source);
    }

    [Fact]
    public void WhenBuildingWithCustomSourceThenSourceIsSet()
    {
        // Arrange & Act
        var result = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusion1)
            .WithAssetType(AirAssetType.Vae)
            .WithSource(AirSource.HuggingFace)
            .WithModelId(100)
            .WithVersionId(200)
            .Build();

        // Assert
        Assert.Equal(AirSource.HuggingFace, result.Source);
    }

    [Fact]
    public void WhenBuildingWithAllEcosystemsThenAllSucceed()
    {
        // Arrange
        var ecosystems = new[]
        {
            AirEcosystem.StableDiffusion1,
            AirEcosystem.StableDiffusion2,
            AirEcosystem.StableDiffusionXl,
            AirEcosystem.Flux1,
            AirEcosystem.Pony
        };

        // Act & Assert
        foreach (var ecosystem in ecosystems)
        {
            var result = new AirBuilder()
                .WithEcosystem(ecosystem)
                .WithAssetType(AirAssetType.Checkpoint)
                .WithModelId(1)
                .WithVersionId(1)
                .Build();

            Assert.Equal(ecosystem, result.Ecosystem);
        }
    }

    [Fact]
    public void WhenBuildingWithAllAssetTypesThenAllSucceed()
    {
        // Arrange
        var assetTypes = new[]
        {
            AirAssetType.Checkpoint,
            AirAssetType.Lora,
            AirAssetType.Lycoris,
            AirAssetType.Vae,
            AirAssetType.Embedding,
            AirAssetType.Hypernetwork
        };

        // Act & Assert
        foreach (var assetType in assetTypes)
        {
            var result = new AirBuilder()
                .WithEcosystem(AirEcosystem.StableDiffusionXl)
                .WithAssetType(assetType)
                .WithModelId(1)
                .WithVersionId(1)
                .Build();

            Assert.Equal(assetType, result.AssetType);
        }
    }

    #endregion

    #region Fluent API Chaining Tests

    [Fact]
    public void WhenUsingFluentApiThenMethodsChainingWorks()
    {
        // Arrange & Act
        var result = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(328553)
            .WithVersionId(368189)
            .WithSource(AirSource.Civitai)
            .Build();

        // Assert
        Assert.Equal(AirEcosystem.StableDiffusionXl, result.Ecosystem);
        Assert.Equal(AirAssetType.Lora, result.AssetType);
    }

    [Fact]
    public void WhenSettingPropertiesInDifferentOrderThenBuildSucceeds()
    {
        // Arrange & Act
        var result = new AirBuilder()
            .WithVersionId(368189)
            .WithModelId(328553)
            .WithAssetType(AirAssetType.Lora)
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .Build();

        // Assert
        Assert.Equal(AirEcosystem.StableDiffusionXl, result.Ecosystem);
        Assert.Equal(AirAssetType.Lora, result.AssetType);
        Assert.Equal(328553, result.ModelId);
        Assert.Equal(368189, result.VersionId);
    }

    #endregion

    #region Immutability Tests

    [Fact]
    public void WhenWithMethodIsCalledThenOriginalBuilderIsUnchanged()
    {
        // Arrange
        var original = new AirBuilder();

        // Act
        var modified = original
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(328553)
            .WithVersionId(368189);

        // Assert - original should still fail to build (unchanged)
        Assert.Throws<InvalidOperationException>(() => original.Build());
        
        // modified should build successfully
        var result = modified.Build();
        Assert.Equal(AirEcosystem.StableDiffusionXl, result.Ecosystem);
    }

    [Fact]
    public void WhenBuildingFromSameBaseBuilderThenResultsAreIndependent()
    {
        // Arrange
        var baseBuilder = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora);

        // Act - create two different configurations from the same base
        var air1 = baseBuilder
            .WithModelId(100)
            .WithVersionId(200)
            .Build();

        var air2 = baseBuilder
            .WithModelId(300)
            .WithVersionId(400)
            .Build();

        // Assert - both should have SDXL/Lora but different IDs
        Assert.Equal(AirEcosystem.StableDiffusionXl, air1.Ecosystem);
        Assert.Equal(AirEcosystem.StableDiffusionXl, air2.Ecosystem);
        Assert.Equal(100, air1.ModelId);
        Assert.Equal(300, air2.ModelId);
    }

    [Fact]
    public void WhenNewBuilderIsCreatedThenSourceDefaultsToCivitai()
    {
        // Arrange & Act
        var result = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(1)
            .WithVersionId(1)
            .Build();

        // Assert
        Assert.Equal(AirSource.Civitai, result.Source);
    }

    #endregion

    #region TryBuild Tests

    [Fact]
    public void WhenAllRequiredPropertiesAreSetThenTryBuildSucceeds()
    {
        // Arrange
        var builder = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(328553)
            .WithVersionId(368189);

        // Act
        var success = builder.TryBuild(out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(AirEcosystem.StableDiffusionXl, result.Ecosystem);
        Assert.Equal(AirAssetType.Lora, result.AssetType);
        Assert.Equal(AirSource.Civitai, result.Source);
        Assert.Equal(328553, result.ModelId);
        Assert.Equal(368189, result.VersionId);
    }

    [Fact]
    public void WhenEcosystemIsMissingThenTryBuildReturnsFalse()
    {
        // Arrange
        var builder = new AirBuilder()
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(328553)
            .WithVersionId(368189);

        // Act
        var success = builder.TryBuild(out var result);

        // Assert
        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void WhenAssetTypeIsMissingThenTryBuildReturnsFalse()
    {
        // Arrange
        var builder = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithModelId(328553)
            .WithVersionId(368189);

        // Act
        var success = builder.TryBuild(out _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void WhenModelIdIsMissingThenTryBuildReturnsFalse()
    {
        // Arrange
        var builder = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithVersionId(368189);

        // Act
        var success = builder.TryBuild(out _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void WhenVersionIdIsMissingThenTryBuildReturnsFalse()
    {
        // Arrange
        var builder = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(328553);

        // Act
        var success = builder.TryBuild(out _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void WhenNoPropertiesAreSetThenTryBuildReturnsFalse()
    {
        // Arrange
        var builder = new AirBuilder();

        // Act
        var success = builder.TryBuild(out _);

        // Assert
        Assert.False(success);
    }

    [Fact]
    public void WhenTryBuildSucceedsThenResultMatchesBuild()
    {
        // Arrange
        var builder = new AirBuilder()
            .WithEcosystem(AirEcosystem.Flux1)
            .WithAssetType(AirAssetType.Checkpoint)
            .WithSource(AirSource.HuggingFace)
            .WithModelId(4201)
            .WithVersionId(130072);

        // Act
        var buildResult = builder.Build();
        var tryBuildSuccess = builder.TryBuild(out var tryBuildResult);

        // Assert
        Assert.True(tryBuildSuccess);
        Assert.Equal(buildResult, tryBuildResult);
    }

    #endregion

    #region Validation Tests - Missing Required Properties

    [Fact]
    public void WhenBuildingWithoutEcosystemThenThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new AirBuilder()
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(328553)
            .WithVersionId(368189);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Ecosystem", exception.Message);
        Assert.Contains("WithEcosystem", exception.Message);
    }

    [Fact]
    public void WhenBuildingWithoutAssetTypeThenThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithModelId(328553)
            .WithVersionId(368189);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("AssetType", exception.Message);
        Assert.Contains("WithAssetType", exception.Message);
    }

    [Fact]
    public void WhenBuildingWithoutModelIdThenThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithVersionId(368189);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("ModelId", exception.Message);
        Assert.Contains("WithModelId", exception.Message);
    }

    [Fact]
    public void WhenBuildingWithoutVersionIdThenThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(328553);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("VersionId", exception.Message);
        Assert.Contains("WithVersionId", exception.Message);
    }

    #endregion

    #region Validation Tests - Invalid Values

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void WhenSettingInvalidModelIdThenThrowsArgumentOutOfRangeException(long invalidModelId)
    {
        // Arrange
        var builder = new AirBuilder();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithModelId(invalidModelId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void WhenSettingInvalidVersionIdThenThrowsArgumentOutOfRangeException(long invalidVersionId)
    {
        // Arrange
        var builder = new AirBuilder();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithVersionId(invalidVersionId));
    }

    [Fact]
    public void WhenSettingAllSourceTypesThenBuildSucceeds()
    {
        // Arrange
        var sources = new[]
        {
            AirSource.Civitai,
            AirSource.HuggingFace,
            AirSource.OpenAi,
            AirSource.Leonardo
        };

        // Act & Assert
        foreach (var source in sources)
        {
            var result = new AirBuilder()
                .WithEcosystem(AirEcosystem.StableDiffusionXl)
                .WithAssetType(AirAssetType.Checkpoint)
                .WithSource(source)
                .WithModelId(1)
                .WithVersionId(1)
                .Build();

            Assert.Equal(source, result.Source);
        }
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void WhenSettingLargeModelIdThenBuildSucceeds()
    {
        // Arrange & Act
        var result = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(long.MaxValue)
            .WithVersionId(1)
            .Build();

        // Assert
        Assert.Equal(long.MaxValue, result.ModelId);
    }

    [Fact]
    public void WhenSettingLargeVersionIdThenBuildSucceeds()
    {
        // Arrange & Act
        var result = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(1)
            .WithVersionId(long.MaxValue)
            .Build();

        // Assert
        Assert.Equal(long.MaxValue, result.VersionId);
    }

    [Fact]
    public void WhenOverwritingPropertyThenLastValueIsUsed()
    {
        // Arrange & Act
        var result = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusion1)
            .WithEcosystem(AirEcosystem.Flux1) // Overwrite
            .WithAssetType(AirAssetType.Checkpoint)
            .WithModelId(100)
            .WithModelId(200) // Overwrite
            .WithVersionId(1)
            .Build();

        // Assert
        Assert.Equal(AirEcosystem.Flux1, result.Ecosystem);
        Assert.Equal(200, result.ModelId);
    }

    #endregion

    #region Integration Tests with AirIdentifier

    [Fact]
    public void WhenBuiltAirIdentifierIsConvertedToStringThenFormatIsCorrect()
    {
        // Arrange & Act
        var air = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(328553)
            .WithVersionId(368189)
            .Build();

        var airString = air.ToString();

        // Assert
        Assert.Equal("urn:air:sdxl:lora:civitai:328553@368189", airString);
    }

    [Fact]
    public void WhenBuiltAirIdentifierCanBeParsedBackThenValuesMatch()
    {
        // Arrange
        var original = new AirBuilder()
            .WithEcosystem(AirEcosystem.Flux1)
            .WithAssetType(AirAssetType.Checkpoint)
            .WithModelId(4201)
            .WithVersionId(130072)
            .Build();

        // Act
        var airString = original.ToString();
        var parsed = AirIdentifier.Parse(airString);

        // Assert
        Assert.Equal(original.Ecosystem, parsed.Ecosystem);
        Assert.Equal(original.AssetType, parsed.AssetType);
        Assert.Equal(original.Source, parsed.Source);
        Assert.Equal(original.ModelId, parsed.ModelId);
        Assert.Equal(original.VersionId, parsed.VersionId);
    }

    [Fact]
    public void WhenBuildingMultipleAirIdentifiersFromSameBuilderThenAllAreIndependent()
    {
        // Arrange - immutable builder allows safe reuse
        var baseBuilder = new AirBuilder();

        // Act
        var air1 = baseBuilder
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(100)
            .WithVersionId(200)
            .Build();

        var air2 = baseBuilder
            .WithEcosystem(AirEcosystem.Flux1)
            .WithAssetType(AirAssetType.Checkpoint)
            .WithModelId(300)
            .WithVersionId(400)
            .Build();

        // Assert
        Assert.NotEqual(air1.Ecosystem, air2.Ecosystem);
        Assert.NotEqual(air1.AssetType, air2.AssetType);
        Assert.NotEqual(air1.ModelId, air2.ModelId);
        Assert.NotEqual(air1.VersionId, air2.VersionId);
    }

    #endregion
}
