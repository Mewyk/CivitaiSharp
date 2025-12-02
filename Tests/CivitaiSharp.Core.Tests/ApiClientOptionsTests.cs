namespace CivitaiSharp.Core.Tests;

using Xunit;

public sealed class ApiClientOptionsTests
{
    [Fact]
    public void WhenCreatingOptionsWithDefaultsThenDefaultValuesAreUsed()
    {
        // Arrange & Act
        var options = new ApiClientOptions();

        // Assert
        Assert.Equal(ApiClientOptions.DefaultBaseUrl, options.BaseUrl);
        Assert.Equal(ApiClientOptions.DefaultApiVersion, options.ApiVersion);
        Assert.Equal(ApiClientOptions.DefaultTimeoutSeconds, options.TimeoutSeconds);
        Assert.Null(options.ApiKey);
    }

    [Fact]
    public void WhenSettingValidBaseUrlThenUrlIsStored()
    {
        // Arrange
        var options = new ApiClientOptions();
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
        var options = new ApiClientOptions();

        // Act
        options.BaseUrl = "https://api.example.com/";

        // Assert
        Assert.Equal("https://api.example.com", options.BaseUrl);
    }

    [Fact]
    public void WhenSettingNullBaseUrlThenThrowsArgumentNullException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.BaseUrl = null!);
    }

    [Fact]
    public void WhenSettingEmptyBaseUrlThenThrowsArgumentException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.BaseUrl = "");
    }

    [Fact]
    public void WhenSettingWhitespaceBaseUrlThenThrowsArgumentException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.BaseUrl = "   ");
    }

    [Fact]
    public void WhenSettingInvalidUrlThenThrowsArgumentException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.BaseUrl = "not-a-valid-url");
    }

    [Fact]
    public void WhenSettingNonHttpUrlThenThrowsArgumentException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.BaseUrl = "ftp://example.com");
    }

    [Fact]
    public void WhenSettingValidApiVersionThenVersionIsStored()
    {
        // Arrange
        var options = new ApiClientOptions();
        const string expectedVersion = "v2";

        // Act
        options.ApiVersion = expectedVersion;

        // Assert
        Assert.Equal(expectedVersion, options.ApiVersion);
    }

    [Fact]
    public void WhenSettingNullApiVersionThenThrowsArgumentNullException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.ApiVersion = null!);
    }

    [Fact]
    public void WhenSettingEmptyApiVersionThenThrowsArgumentException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.ApiVersion = "");
    }

    [Fact]
    public void WhenSettingValidTimeoutThenTimeoutIsStored()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act
        options.TimeoutSeconds = 60;

        // Assert
        Assert.Equal(60, options.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingTimeoutBelowMinimumThenThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.TimeoutSeconds = 0);
    }

    [Fact]
    public void WhenSettingTimeoutAboveMaximumThenThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.TimeoutSeconds = 301);
    }

    [Fact]
    public void WhenSettingTimeoutToMaximumValueThenSucceeds()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act
        options.TimeoutSeconds = ApiClientOptions.MaxTimeoutSeconds;

        // Assert
        Assert.Equal(300, options.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingTimeoutToMinimumValueThenSucceeds()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act
        options.TimeoutSeconds = 1;

        // Assert
        Assert.Equal(1, options.TimeoutSeconds);
    }

    [Fact]
    public void WhenSettingApiKeyThenKeyIsStored()
    {
        // Arrange
        var options = new ApiClientOptions();
        const string apiKey = "test-api-key";

        // Act
        options.ApiKey = apiKey;

        // Assert
        Assert.Equal(apiKey, options.ApiKey);
    }

    [Fact]
    public void WhenCallingGetApiPathThenReturnsCorrectPath()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act
        var path = options.GetApiPath("models");

        // Assert
        Assert.Equal("/api/v1/models", path);
    }

    [Fact]
    public void WhenCallingGetApiPathWithLeadingSlashThenSlashIsRemoved()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act
        var path = options.GetApiPath("/images");

        // Assert
        Assert.Equal("/api/v1/images", path);
    }

    [Fact]
    public void WhenCallingGetApiPathWithCustomVersionThenUsesCustomVersion()
    {
        // Arrange
        var options = new ApiClientOptions { ApiVersion = "v2" };

        // Act
        var path = options.GetApiPath("tags");

        // Assert
        Assert.Equal("/api/v2/tags", path);
    }

    [Fact]
    public void WhenCallingGetApiPathWithNullPathThenThrowsArgumentNullException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.GetApiPath(null!));
    }

    [Fact]
    public void WhenCallingGetApiPathWithEmptyPathThenThrowsArgumentException()
    {
        // Arrange
        var options = new ApiClientOptions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => options.GetApiPath(""));
    }
}
