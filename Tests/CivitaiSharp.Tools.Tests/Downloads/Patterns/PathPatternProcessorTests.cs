namespace CivitaiSharp.Tools.Tests.Downloads.Patterns;

using System.Collections.Frozen;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Tools.Downloads.Patterns;
using Xunit;

public sealed class PathPatternProcessorTests
{
    [Fact]
    public void WhenValidatingPatternWithValidTokensThenReturnsSuccess()
    {
        // Arrange
        var pattern = "{Id}.{Extension}";
        var validTokens = new[] { "Id", "Extension" }.ToFrozenSet();

        // Act
        var result = PathPatternProcessor.ValidatePattern(pattern, validTokens);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void WhenValidatingPatternWithInvalidTokenThenReturnsFailure()
    {
        // Arrange
        var pattern = "{Id}.{InvalidToken}";
        var validTokens = new[] { "Id", "Extension" }.ToFrozenSet();

        // Act
        var result = PathPatternProcessor.ValidatePattern(pattern, validTokens);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.PatternValidationFailed, result.ErrorInfo.Code);
        Assert.Contains("InvalidToken", result.ErrorInfo.Message);
    }

    [Fact]
    public void WhenValidatingPatternWithMalformedTokenThenReturnsFailure()
    {
        // Arrange - this malformed pattern has nested braces which results in invalid token name
        var pattern = "{Id.{Extension}";
        var validTokens = new[] { "Id", "Extension" }.ToFrozenSet();

        // Act
        var result = PathPatternProcessor.ValidatePattern(pattern, validTokens);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.PatternValidationFailed, result.ErrorInfo.Code);
        Assert.Contains("Invalid tokens", result.ErrorInfo.Message);
    }

    [Fact]
    public void WhenValidatingPatternWithEmptyTokenThenReturnsFailure()
    {
        // Arrange
        var pattern = "{}.{Extension}";
        var validTokens = new[] { "Id", "Extension" }.ToFrozenSet();

        // Act
        var result = PathPatternProcessor.ValidatePattern(pattern, validTokens);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.PatternValidationFailed, result.ErrorInfo.Code);
        Assert.Contains("Empty", result.ErrorInfo.Message);
    }

    [Fact]
    public void WhenValidatingPatternWithUnclosedBraceThenReturnsFailure()
    {
        // Arrange - truly unclosed brace (no closing brace at all)
        var pattern = "{Id";
        var validTokens = new[] { "Id", "Extension" }.ToFrozenSet();

        // Act
        var result = PathPatternProcessor.ValidatePattern(pattern, validTokens);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.PatternValidationFailed, result.ErrorInfo.Code);
        Assert.Contains("Unclosed", result.ErrorInfo.Message);
    }

    [Fact]
    public void WhenValidatingNullPatternThenThrowsArgumentException()
    {
        // Arrange
        var validTokens = new[] { "Id" }.ToFrozenSet();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            PathPatternProcessor.ValidatePattern(null!, validTokens));
    }

    [Fact]
    public void WhenProcessingPatternThenTokensAreReplaced()
    {
        // Arrange
        var pattern = "{Id}.{Extension}";
        var tokenValues = new Dictionary<string, string>
        {
            ["Id"] = "12345",
            ["Extension"] = "png"
        };

        // Act
        var result = PathPatternProcessor.Process(pattern, tokenValues);

        // Assert
        Assert.Equal("12345.png", result);
    }

    [Fact]
    public void WhenProcessingPatternWithSubdirectoriesThenPathIsSanitized()
    {
        // Arrange
        var pattern = "{Username}/{Id}.{Extension}";
        var tokenValues = new Dictionary<string, string>
        {
            ["Username"] = "TestUser",
            ["Id"] = "12345",
            ["Extension"] = "png"
        };

        // Act
        var result = PathPatternProcessor.Process(pattern, tokenValues);

        // Assert
        Assert.Contains(Path.DirectorySeparatorChar.ToString(), result);
        Assert.DoesNotContain("//", result);
        Assert.DoesNotContain("\\\\", result);
    }

    [Fact]
    public void WhenProcessingPatternWithInvalidCharactersThenCharactersAreSanitized()
    {
        // Arrange
        var pattern = "{Username}/{Id}.{Extension}";
        var tokenValues = new Dictionary<string, string>
        {
            ["Username"] = "Test<>User|Name",
            ["Id"] = "12345",
            ["Extension"] = "png"
        };

        // Act
        var result = PathPatternProcessor.Process(pattern, tokenValues);

        // Assert
        Assert.DoesNotContain("<", result);
        Assert.DoesNotContain(">", result);
        Assert.DoesNotContain("|", result);
    }

    [Fact]
    public void WhenProcessingPatternWithMissingTokenThenTokenIsKeptAsLiteral()
    {
        // Arrange
        var pattern = "{Id}.{Extension}";
        var tokenValues = new Dictionary<string, string>
        {
            ["Id"] = "12345"
            // Extension is missing
        };

        // Act
        var result = PathPatternProcessor.Process(pattern, tokenValues);

        // Assert
        Assert.Contains("{Extension}", result);
    }

    [Fact]
    public void WhenProcessingPatternWithTextBetweenTokensThenTextIsPreserved()
    {
        // Arrange
        var pattern = "prefix_{Id}_middle_{Extension}_suffix";
        var tokenValues = new Dictionary<string, string>
        {
            ["Id"] = "12345",
            ["Extension"] = "png"
        };

        // Act
        var result = PathPatternProcessor.Process(pattern, tokenValues);

        // Assert
        Assert.Equal("prefix_12345_middle_png_suffix", result);
    }
}
