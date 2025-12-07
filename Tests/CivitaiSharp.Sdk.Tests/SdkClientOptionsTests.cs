namespace CivitaiSharp.Sdk.Tests;

using Xunit;

public sealed class SdkClientOptionsTests
{
    #region Default Values Tests

    [Fact]
    public void WhenCreatingOptionsWithDefaultsThenDefaultValuesAreUsed()
    {
        // Arrange & Act
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Assert
        Assert.Equal(SdkClientOptions.DefaultBaseUrl, options.BaseUrl);
        Assert.Equal(SdkClientOptions.DefaultApiVersion, options.ApiVersion);
        Assert.Equal(SdkClientOptions.DefaultTimeoutSeconds, options.TimeoutSeconds);
        Assert.Equal("test-token", options.ApiToken);
    }

    [Fact]
    public void WhenCheckingDefaultConstantsThenValuesAreCorrect()
    {
        // Assert
        Assert.Equal("https://orchestration.civitai.com", SdkClientOptions.DefaultBaseUrl);
        Assert.Equal("v1", SdkClientOptions.DefaultApiVersion);
        Assert.Equal(600, SdkClientOptions.DefaultTimeoutSeconds);
        Assert.Equal(1800, SdkClientOptions.MaxTimeoutSeconds);
    }

    #endregion

    #region ApiToken Tests

    [Fact]
    public void WhenSettingValidApiTokenThenTokenIsStored()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "initial-token" };

        // Act
        options.ApiToken = "new-valid-token";

        // Assert
        Assert.Equal("new-valid-token", options.ApiToken);
    }

    [Fact]
    public void WhenSettingNullApiTokenThenThrowsArgumentNullException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "initial-token" };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.ApiToken = null!);
    }

    [Fact]
    public void WhenSettingEmptyApiTokenThenThrowsArgumentException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "initial-token" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.ApiToken = "");
    }

    [Fact]
    public void WhenSettingWhitespaceApiTokenThenThrowsArgumentException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "initial-token" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.ApiToken = "   ");
    }

    #endregion

    #region BaseUrl Tests

    [Fact]
    public void WhenSettingValidBaseUrlThenUrlIsStored()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };
        const string expectedUrl = "https://api.example.com";

        // Act
        options.BaseUrl = expectedUrl;

        // Assert
        Assert.Equal(expectedUrl, options.BaseUrl);
    }

    [Fact]
    public void WhenSettingBaseUrlWithTrailingSlashThenSlashIsRemoved()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act
        options.BaseUrl = "https://api.example.com/";

        // Assert
        Assert.Equal("https://api.example.com", options.BaseUrl);
    }

    [Fact]
    public void WhenSettingNullBaseUrlThenThrowsArgumentNullException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.BaseUrl = null!);
    }

    [Fact]
    public void WhenSettingEmptyBaseUrlThenThrowsArgumentException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.BaseUrl = "");
    }

    [Fact]
    public void WhenSettingWhitespaceBaseUrlThenThrowsArgumentException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.BaseUrl = "   ");
    }

    [Fact]
    public void WhenSettingInvalidUrlThenThrowsArgumentException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.BaseUrl = "not-a-valid-url");
    }

    [Fact]
    public void WhenSettingNonHttpUrlThenThrowsArgumentException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.BaseUrl = "ftp://example.com");
    }

    [Fact]
    public void WhenSettingHttpUrlThenUrlIsAccepted()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act
        options.BaseUrl = "http://localhost:8080";

        // Assert
        Assert.Equal("http://localhost:8080", options.BaseUrl);
    }

    #endregion

    #region ApiVersion Tests

    [Fact]
    public void WhenSettingValidApiVersionThenVersionIsStored()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act
        options.ApiVersion = "v2";

        // Assert
        Assert.Equal("v2", options.ApiVersion);
    }

    [Fact]
    public void WhenSettingNullApiVersionThenThrowsArgumentNullException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.ApiVersion = null!);
    }

    [Fact]
    public void WhenSettingEmptyApiVersionThenThrowsArgumentException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.ApiVersion = "");
    }

    [Fact]
    public void WhenSettingWhitespaceApiVersionThenThrowsArgumentException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.ApiVersion = "   ");
    }

    #endregion

    #region TimeoutSeconds Tests

    [Fact]
    public void WhenSettingValidTimeoutThenTimeoutIsStored()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act
        options.TimeoutSeconds = 300;

        // Assert
        Assert.Equal(300, options.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingMinimumTimeoutThenTimeoutIsAccepted()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act
        options.TimeoutSeconds = 1;

        // Assert
        Assert.Equal(1, options.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingMaximumTimeoutThenTimeoutIsAccepted()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act
        options.TimeoutSeconds = SdkClientOptions.MaxTimeoutSeconds;

        // Assert
        Assert.Equal(SdkClientOptions.MaxTimeoutSeconds, options.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingZeroTimeoutThenThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.TimeoutSeconds = 0);
    }

    [Fact]
    public void WhenSettingNegativeTimeoutThenThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.TimeoutSeconds = -1);
    }

    [Fact]
    public void WhenSettingTimeoutAboveMaximumThenThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = new SdkClientOptions { ApiToken = "test-token" };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.TimeoutSeconds = SdkClientOptions.MaxTimeoutSeconds + 1);
    }

    #endregion
}
