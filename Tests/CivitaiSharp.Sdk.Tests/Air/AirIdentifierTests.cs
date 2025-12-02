namespace CivitaiSharp.Sdk.Tests.Air;

using CivitaiSharp.Sdk.Air;
using Xunit;

public sealed class AirIdentifierTests : IClassFixture<SdkTestFixture>
{
    #region Constructor Tests

    [Fact]
    public void WhenCreatingWithValidParametersThenPropertiesAreSet()
    {
        // Arrange & Act
        var air = new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            "civitai",
            4201,
            130072);

        // Assert
        Assert.Equal(AirEcosystem.StableDiffusionXl, air.Ecosystem);
        Assert.Equal(AirAssetType.Checkpoint, air.AssetType);
        Assert.Equal("civitai", air.Source);
        Assert.Equal(4201, air.ModelId);
        Assert.Equal(130072, air.VersionId);
    }

    [Fact]
    public void WhenCreatingWithNullSourceThenThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            null!,
            4201,
            130072));
    }

    [Fact]
    public void WhenCreatingWithEmptySourceThenThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            "",
            4201,
            130072));
    }

    [Fact]
    public void WhenCreatingWithWhitespaceSourceThenThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            "   ",
            4201,
            130072));
    }

    [Fact]
    public void WhenCreatingWithZeroModelIdThenThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            "civitai",
            0,
            130072));
    }

    [Fact]
    public void WhenCreatingWithNegativeModelIdThenThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            "civitai",
            -1,
            130072));
    }

    [Fact]
    public void WhenCreatingWithZeroVersionIdThenThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            "civitai",
            4201,
            0));
    }

    [Fact]
    public void WhenCreatingWithNegativeVersionIdThenThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new AirIdentifier(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            "civitai",
            4201,
            -1));
    }

    #endregion

    #region Create Factory Method Tests

    [Fact]
    public void WhenCreatingViaFactoryMethodThenSourceIsCivitai()
    {
        // Arrange & Act
        var air = AirIdentifier.Create(
            AirEcosystem.Flux1,
            AirAssetType.Lora,
            12345,
            67890);

        // Assert
        Assert.Equal("civitai", air.Source);
        Assert.Equal(AirEcosystem.Flux1, air.Ecosystem);
        Assert.Equal(AirAssetType.Lora, air.AssetType);
        Assert.Equal(12345, air.ModelId);
        Assert.Equal(67890, air.VersionId);
    }

    #endregion

    #region TryParse Tests

    [Fact]
    public void WhenParsingValidAirStringThenReturnsTrue()
    {
        // Arrange
        const string airString = "urn:air:sdxl:checkpoint:civitai:4201@130072";

        // Act
        var success = AirIdentifier.TryParse(airString, out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(AirEcosystem.StableDiffusionXl, result.Ecosystem);
        Assert.Equal(AirAssetType.Checkpoint, result.AssetType);
        Assert.Equal("civitai", result.Source);
        Assert.Equal(4201, result.ModelId);
        Assert.Equal(130072, result.VersionId);
    }

    [Theory]
    [InlineData("urn:air:sd1:lora:civitai:100@200", AirEcosystem.StableDiffusion1, AirAssetType.Lora)]
    [InlineData("urn:air:sd2:vae:civitai:300@400", AirEcosystem.StableDiffusion2, AirAssetType.Vae)]
    [InlineData("urn:air:flux1:checkpoint:civitai:500@600", AirEcosystem.Flux1, AirAssetType.Checkpoint)]
    [InlineData("urn:air:pony:embedding:civitai:700@800", AirEcosystem.Pony, AirAssetType.Embedding)]
    [InlineData("urn:air:sdxl:lycoris:civitai:900@1000", AirEcosystem.StableDiffusionXl, AirAssetType.Lycoris)]
    [InlineData("urn:air:sdxl:hypernet:civitai:1100@1200", AirEcosystem.StableDiffusionXl, AirAssetType.Hypernetwork)]
    public void WhenParsingVariousValidAirStringsThenParsesCorrectly(
        string airString,
        AirEcosystem expectedEcosystem,
        AirAssetType expectedAssetType)
    {
        // Act
        var success = AirIdentifier.TryParse(airString, out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedEcosystem, result.Ecosystem);
        Assert.Equal(expectedAssetType, result.AssetType);
    }

    [Fact]
    public void WhenParsingCaseInsensitiveAirStringThenReturnsTrue()
    {
        // Arrange - mixed case
        const string airString = "URN:AIR:SDXL:CHECKPOINT:CIVITAI:4201@130072";

        // Act
        var success = AirIdentifier.TryParse(airString, out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(AirEcosystem.StableDiffusionXl, result.Ecosystem);
    }

    [Fact]
    public void WhenParsingNullStringThenReturnsFalse()
    {
        // Act
        var success = AirIdentifier.TryParse(null, out var result);

        // Assert
        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void WhenParsingEmptyStringThenReturnsFalse()
    {
        // Act
        var success = AirIdentifier.TryParse("", out var result);

        // Assert
        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void WhenParsingWhitespaceStringThenReturnsFalse()
    {
        // Act
        var success = AirIdentifier.TryParse("   ", out var result);

        // Assert
        Assert.False(success);
    }

    [Theory]
    [InlineData("not-an-air-string")]
    [InlineData("urn:air:invalid:checkpoint:civitai:4201@130072")]
    [InlineData("urn:air:sdxl:invalid:civitai:4201@130072")]
    [InlineData("urn:air:sdxl:checkpoint:civitai:abc@130072")]
    [InlineData("urn:air:sdxl:checkpoint:civitai:4201@abc")]
    [InlineData("urn:air:sdxl:checkpoint:civitai:0@130072")]
    [InlineData("urn:air:sdxl:checkpoint:civitai:4201@0")]
    [InlineData("urn:air:sdxl:checkpoint:civitai:-1@130072")]
    [InlineData("urn:air:sdxl:checkpoint:civitai:4201@-1")]
    [InlineData("urn:air:sdxl:checkpoint:civitai:4201")]
    [InlineData("air:sdxl:checkpoint:civitai:4201@130072")]
    public void WhenParsingInvalidAirStringThenReturnsFalse(string invalidAirString)
    {
        // Act
        var success = AirIdentifier.TryParse(invalidAirString, out _);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region Parse Tests

    [Fact]
    public void WhenParsingValidStringWithParseThenReturnsAirIdentifier()
    {
        // Arrange
        const string airString = "urn:air:sdxl:checkpoint:civitai:4201@130072";

        // Act
        var result = AirIdentifier.Parse(airString);

        // Assert
        Assert.Equal(AirEcosystem.StableDiffusionXl, result.Ecosystem);
        Assert.Equal(4201, result.ModelId);
    }

    [Fact]
    public void WhenParsingNullStringWithParseThenThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AirIdentifier.Parse(null!));
    }

    [Fact]
    public void WhenParsingEmptyStringWithParseThenThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => AirIdentifier.Parse(""));
    }

    [Fact]
    public void WhenParsingInvalidStringWithParseThenThrowsFormatException()
    {
        // Act & Assert
        var exception = Assert.Throws<FormatException>(() => AirIdentifier.Parse("not-valid"));
        Assert.Contains("is not a valid AIR identifier", exception.Message);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void WhenConvertingToStringThenReturnsCorrectFormat()
    {
        // Arrange
        var air = AirIdentifier.Create(
            AirEcosystem.StableDiffusionXl,
            AirAssetType.Checkpoint,
            4201,
            130072);

        // Act
        var result = air.ToString();

        // Assert
        Assert.Equal("urn:air:sdxl:checkpoint:civitai:4201@130072", result);
    }

    [Fact]
    public void WhenRoundTrippingThroughStringThenValuesArePreserved()
    {
        // Arrange
        var original = AirIdentifier.Create(
            AirEcosystem.Flux1,
            AirAssetType.Lora,
            12345,
            67890);

        // Act
        var stringValue = original.ToString();
        var parsed = AirIdentifier.Parse(stringValue);

        // Assert
        Assert.Equal(original, parsed);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void WhenComparingEqualIdentifiersThenReturnsTrue()
    {
        // Arrange
        var air1 = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 4201, 130072);
        var air2 = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 4201, 130072);

        // Act & Assert
        Assert.True(air1.Equals(air2));
        Assert.True(air1 == air2);
        Assert.False(air1 != air2);
        Assert.Equal(air1.GetHashCode(), air2.GetHashCode());
    }

    [Fact]
    public void WhenComparingDifferentEcosystemsThenReturnsFalse()
    {
        // Arrange
        var air1 = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 4201, 130072);
        var air2 = AirIdentifier.Create(AirEcosystem.Flux1, AirAssetType.Checkpoint, 4201, 130072);

        // Act & Assert
        Assert.False(air1.Equals(air2));
        Assert.False(air1 == air2);
        Assert.True(air1 != air2);
    }

    [Fact]
    public void WhenComparingDifferentAssetTypesThenReturnsFalse()
    {
        // Arrange
        var air1 = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 4201, 130072);
        var air2 = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Lora, 4201, 130072);

        // Act & Assert
        Assert.False(air1.Equals(air2));
    }

    [Fact]
    public void WhenComparingDifferentModelIdsThenReturnsFalse()
    {
        // Arrange
        var air1 = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 4201, 130072);
        var air2 = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 9999, 130072);

        // Act & Assert
        Assert.False(air1.Equals(air2));
    }

    [Fact]
    public void WhenComparingDifferentVersionIdsThenReturnsFalse()
    {
        // Arrange
        var air1 = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 4201, 130072);
        var air2 = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 4201, 999999);

        // Act & Assert
        Assert.False(air1.Equals(air2));
    }

    [Fact]
    public void WhenComparingWithSourceCaseDifferenceThenReturnsTrue()
    {
        // Arrange - Source comparison should be case-insensitive
        var air1 = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, "civitai", 4201, 130072);
        var air2 = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, "CIVITAI", 4201, 130072);

        // Act & Assert
        Assert.True(air1.Equals(air2));
        Assert.Equal(air1.GetHashCode(), air2.GetHashCode());
    }

    [Fact]
    public void WhenComparingWithObjectThenEqualsWorksCorrectly()
    {
        // Arrange
        var air = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 4201, 130072);
        object boxed = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 4201, 130072);
        object differentType = "not an AirIdentifier";

        // Act & Assert
        Assert.True(air.Equals(boxed));
        Assert.False(air.Equals(differentType));
        Assert.False(air.Equals(null));
    }

    #endregion

    #region Implicit Conversion Tests

    [Fact]
    public void WhenImplicitlyConvertingToStringThenReturnsCorrectFormat()
    {
        // Arrange
        var air = AirIdentifier.Create(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, 4201, 130072);

        // Act
        string result = air;

        // Assert
        Assert.Equal("urn:air:sdxl:checkpoint:civitai:4201@130072", result);
    }

    #endregion
}
