namespace CivitaiSharp.Tools.Tests.Downloads.Validation;

using CivitaiSharp.Tools.Downloads.Options;
using CivitaiSharp.Tools.Downloads.Validation;
using CivitaiSharp.Tools.Hashing;
using Xunit;

public sealed class ModelOptionsValidatorTests
{
    private readonly ModelOptionsValidator _validator = new();

    [Fact]
    public void WhenValidatingValidOptionsThenReturnsSuccess()
    {
        // Arrange
        var options = new ModelDownloadOptions
        {
            BaseDirectory = Path.GetTempPath(),
            PathPattern = "{FileName}",
            OverwriteExisting = true,
            VerifyHash = true,
            HashAlgorithm = HashAlgorithm.Sha256
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
        var options = new ModelDownloadOptions
        {
            BaseDirectory = null!,
            PathPattern = "{FileName}"
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
        var options = new ModelDownloadOptions
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
        var options = new ModelDownloadOptions
        {
            BaseDirectory = Path.GetTempPath(),
            PathPattern = "{InvalidToken}/{FileName}"
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains("InvalidToken", result.FailureMessage);
    }

    [Fact]
    public void WhenValidatingOptionsWithFileOnlyTokensThenReturnsSuccess()
    {
        // Arrange
        var options = new ModelDownloadOptions
        {
            BaseDirectory = Path.GetTempPath(),
            PathPattern = "{Format}/{FileType}/{FileName}"
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void WhenValidatingOptionsWithAllTokensThenReturnsSuccess()
    {
        // Arrange
        var options = new ModelDownloadOptions
        {
            BaseDirectory = Path.GetTempPath(),
            PathPattern = "{ModelType}/{BaseModel}/{ModelName}/{VersionName}/{FileName}"
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(HashAlgorithm.Sha256)]
    [InlineData(HashAlgorithm.Sha512)]
    [InlineData(HashAlgorithm.Blake3)]
    [InlineData(HashAlgorithm.Crc32)]
    public void WhenValidatingOptionsWithValidHashAlgorithmThenReturnsSuccess(HashAlgorithm algorithm)
    {
        // Arrange
        var options = new ModelDownloadOptions
        {
            BaseDirectory = Path.GetTempPath(),
            PathPattern = "{FileName}",
            HashAlgorithm = algorithm
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void WhenValidatingOptionsWithInvalidHashAlgorithmThenReturnsFail()
    {
        // Arrange
        var options = new ModelDownloadOptions
        {
            BaseDirectory = Path.GetTempPath(),
            PathPattern = "{FileName}",
            HashAlgorithm = (HashAlgorithm)999
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains("HashAlgorithm", result.FailureMessage);
    }

    [Fact]
    public void WhenValidatingNullOptionsThenThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _validator.Validate(null, null!));
    }
}
