namespace CivitaiSharp.Core.Tests;

using Xunit;

/// <summary>
/// Tests for ApiOptions configuration.
/// </summary>
public sealed class ApiOptionsTests
{
    #region Default Values Tests

    [Fact]
    public void WhenCreatingOptionsWithDefaultsThenDefaultValuesAreUsed()
    {
        // Arrange & Act
        var options = new CivitaiSharpOptions();

        // Assert
        Assert.Equal(ApiOptions.DefaultVersion, options.Api.Version);
        Assert.Equal(ApiOptions.DefaultTimeoutSeconds, options.Api.TimeoutSeconds);
        Assert.Null(options.Api.Key);
    }

    [Fact]
    public void WhenCheckingDefaultConstantsThenValuesAreCorrect()
    {
        // Assert
        Assert.Equal("https://civitai.com", ApiOptions.DefaultBaseUrl);
        Assert.Equal("v1", ApiOptions.DefaultVersion);
        Assert.Equal(30, ApiOptions.DefaultTimeoutSeconds);
        Assert.Equal(300, ApiOptions.MaxTimeoutSeconds);
    }

    #endregion

    #region Key Tests

    [Fact]
    public void WhenSettingValidApiKeyThenKeyIsStored()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        const string expectedKey = "test-api-key";

        // Act
        options.Api.Key = expectedKey;

        // Assert
        Assert.Equal(expectedKey, options.Api.Key);
    }

    [Fact]
    public void WhenSettingNullApiKeyThenKeyIsNull()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Api.Key = "initial-key";

        // Act
        options.Api.Key = null;

        // Assert
        Assert.Null(options.Api.Key);
    }

    #endregion

    #region Version Tests

    [Fact]
    public void WhenSettingValidApiVersionThenVersionIsStored()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        const string expectedVersion = "v2";

        // Act
        options.Api.Version = expectedVersion;

        // Assert
        Assert.Equal(expectedVersion, options.Api.Version);
    }

    [Fact]
    public void WhenSettingNullApiVersionThenValidateThrows()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Api.Version = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.Api.Validate());
    }

    [Fact]
    public void WhenSettingEmptyApiVersionThenValidateThrows()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Api.Version = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.Api.Validate());
    }

    #endregion

    #region TimeoutSeconds Tests

    [Fact]
    public void WhenSettingValidTimeoutThenTimeoutIsStored()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act
        options.Api.TimeoutSeconds = 60;

        // Assert
        Assert.Equal(60, options.Api.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingTimeoutBelowMinimumThenValidateThrows()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Api.TimeoutSeconds = 0;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.Api.Validate());
    }

    [Fact]
    public void WhenSettingTimeoutAboveMaximumThenValidateThrows()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Api.TimeoutSeconds = 301;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.Api.Validate());
    }

    [Fact]
    public void WhenSettingTimeoutToMaximumValueThenSucceeds()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act
        options.Api.TimeoutSeconds = ApiOptions.MaxTimeoutSeconds;

        // Assert
        Assert.Equal(300, options.Api.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingTimeoutToMinimumValueThenSucceeds()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act
        options.Api.TimeoutSeconds = 1;

        // Assert
        Assert.Equal(1, options.Api.TimeoutSeconds);
    }

    #endregion

    #region GetApiPath Tests

    [Fact]
    public void WhenCallingGetApiPathThenReturnsCorrectPath()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act
        var path = options.Api.GetApiPath("models");

        // Assert
        Assert.Equal("/api/v1/models", path);
    }

    [Fact]
    public void WhenCallingGetApiPathWithLeadingSlashThenSlashIsRemoved()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act
        var path = options.Api.GetApiPath("/images");

        // Assert
        Assert.Equal("/api/v1/images", path);
    }

    [Fact]
    public void WhenCallingGetApiPathWithCustomVersionThenUsesCustomVersion()
    {
        // Arrange
        var options = new CivitaiSharpOptions();
        options.Api.Version = "v2";

        // Act
        var path = options.Api.GetApiPath("tags");

        // Assert
        Assert.Equal("/api/v2/tags", path);
    }

    [Fact]
    public void WhenCallingGetApiPathWithNullPathThenThrowsArgumentNullException()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.Api.GetApiPath(null!));
    }

    [Fact]
    public void WhenCallingGetApiPathWithEmptyPathThenThrowsArgumentException()
    {
        // Arrange
        var options = new CivitaiSharpOptions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.Api.GetApiPath(""));
    }

    #endregion
}
