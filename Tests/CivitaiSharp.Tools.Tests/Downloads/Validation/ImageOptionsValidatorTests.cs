namespace CivitaiSharp.Tools.Tests.Downloads.Validation;

using CivitaiSharp.Tools.Downloads.Options;
using CivitaiSharp.Tools.Downloads.Validation;
using CivitaiSharp.Tools.Hashing;
using Xunit;

public sealed class ImageOptionsValidatorTests
{
    private readonly ImageOptionsValidator _validator = new();

    [Fact]
    public void WhenValidatingValidOptionsThenReturnsSuccess()
    {
        // Arrange
        var options = new ImageDownloadOptions
        {
            BaseDirectory = Path.GetTempPath(),
            PathPattern = "{Id}.{Extension}",
            OverwriteExisting = false
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void WhenValidatingOptionsWithNullBaseDirectoryThenReturnsFail()
    {
        // Arrange
        var options = new ImageDownloadOptions
        {
            BaseDirectory = null!,
            PathPattern = "{Id}.{Extension}"
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains("BaseDirectory", result.FailureMessage);
    }

    [Fact]
    public void WhenValidatingOptionsWithEmptyPathPatternThenReturnsFail()
    {
        // Arrange
        var options = new ImageDownloadOptions
        {
            BaseDirectory = Path.GetTempPath(),
            PathPattern = ""
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains("PathPattern", result.FailureMessage);
    }

    [Fact]
    public void WhenValidatingOptionsWithInvalidTokenThenReturnsFail()
    {
        // Arrange
        var options = new ImageDownloadOptions
        {
            BaseDirectory = Path.GetTempPath(),
            PathPattern = "{InvalidToken}.{Extension}"
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains("InvalidToken", result.FailureMessage);
    }

    [Fact]
    public void WhenValidatingOptionsWithComplexPatternThenReturnsSuccess()
    {
        // Arrange
        var options = new ImageDownloadOptions
        {
            BaseDirectory = Path.GetTempPath(),
            PathPattern = "{BaseModel}/{Username}/{Date}/{Id}_{Width}x{Height}.{Extension}"
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void WhenValidatingNullOptionsThenThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _validator.Validate(null, null!));
    }
}
