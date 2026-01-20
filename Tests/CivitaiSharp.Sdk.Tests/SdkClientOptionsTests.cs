namespace CivitaiSharp.Sdk.Tests;

using CivitaiSharp.Core;
using Xunit;

/// <summary>
/// Tests for SdkOptions configuration.
/// </summary>
public sealed class SdkOptionsTests
{
    #region Default Values Tests

    [Fact]
    public void WhenCreatingOptionsWithDefaultsThenDefaultValuesAreUsed()
    {
        // Arrange & Act
        var options = new CivitaiSharpOptions();
        options.Sdk.Key = "test-token";

        // Assert
        Assert.Equal(SdkOptions.DefaultVersion, options.Sdk.Version);
        Assert.Equal(SdkOptions.DefaultTimeoutSeconds, options.Sdk.TimeoutSeconds);
        Assert.Equal("test-token", options.Sdk.Key);
    }

    [Fact]
    public void WhenCheckingDefaultConstantsThenValuesAreCorrect()
    {
        // Assert
        Assert.Equal("https://orchestration.civitai.com", SdkOptions.DefaultBaseUrl);
        Assert.Equal("v1", SdkOptions.DefaultVersion);
        Assert.Equal(600, SdkOptions.DefaultTimeoutSeconds);
        Assert.Equal(1800, SdkOptions.MaxTimeoutSeconds);
    }

    #endregion

    #region Key Tests

    [Fact]
    public void WhenSettingValidApiTokenThenTokenIsStored()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Sdk.Key = "initial-token";

        // Act
        options.Sdk.Key = "new-valid-token";

        // Assert
        Assert.Equal("new-valid-token", options.Sdk.Key);
    }

    [Fact]
    public void WhenSettingNullApiTokenThenTokenIsNull()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Sdk.Key = "initial-token";

        // Act
        options.Sdk.Key = null;

        // Assert
        Assert.Null(options.Sdk.Key);
    }

    #endregion

    #region Version Tests

    [Fact]
    public void WhenSettingValidApiVersionThenVersionIsStored()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act
        options.Sdk.Version = "v2";

        // Assert
        Assert.Equal("v2", options.Sdk.Version);
    }

    [Fact]
    public void WhenSettingNullApiVersionThenValidateThrows()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Sdk.Key = "test-key";
        options.Sdk.Version = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.Sdk.Validate());
    }

    [Fact]
    public void WhenSettingEmptyApiVersionThenValidateThrows()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Sdk.Key = "test-key";
        options.Sdk.Version = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.Sdk.Validate());
    }

    [Fact]
    public void WhenSettingWhitespaceApiVersionThenValidateThrows()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Sdk.Key = "test-key";
        options.Sdk.Version = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.Sdk.Validate());
    }

    #endregion

    #region TimeoutSeconds Tests

    [Fact]
    public void WhenSettingValidTimeoutThenTimeoutIsStored()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act
        options.Sdk.TimeoutSeconds = 300;

        // Assert
        Assert.Equal(300, options.Sdk.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingMinimumTimeoutThenTimeoutIsAccepted()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act
        options.Sdk.TimeoutSeconds = 1;

        // Assert
        Assert.Equal(1, options.Sdk.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingMaximumTimeoutThenTimeoutIsAccepted()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act
        options.Sdk.TimeoutSeconds = SdkOptions.MaxTimeoutSeconds;

        // Assert
        Assert.Equal(SdkOptions.MaxTimeoutSeconds, options.Sdk.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingZeroTimeoutThenValidateThrows()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Sdk.Key = "test-key";
        options.Sdk.TimeoutSeconds = 0;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.Sdk.Validate());
    }

    [Fact]
    public void WhenSettingNegativeTimeoutThenValidateThrows()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Sdk.Key = "test-key";
        options.Sdk.TimeoutSeconds = -1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.Sdk.Validate());
    }

    [Fact]
    public void WhenSettingTimeoutAboveMaximumThenValidateThrows()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Sdk.Key = "test-key";
        options.Sdk.TimeoutSeconds = SdkOptions.MaxTimeoutSeconds + 1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.Sdk.Validate());
    }

    #endregion
}
