namespace CivitaiSharp.Core.Tests.Response;

using CivitaiSharp.Core.Response;
using Xunit;

public sealed class ErrorTests
{
    [Fact]
    public void WhenCreatingErrorThenPropertiesAreSet()
    {
        // Arrange
        const ErrorCode code = ErrorCode.Unknown;
        const string message = "Test message";

        // Act
        var error = Error.Create(code, message);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(message, error.Message);
        Assert.Null(error.Details);
        Assert.Null(error.InnerException);
    }

    [Fact]
    public void WhenCreatingErrorWithDetailsThenDetailsAreSet()
    {
        // Arrange
        const ErrorCode code = ErrorCode.BadRequest;
        const string message = "Test message";
        var details = new Dictionary<string, string[]>
        {
            ["field1"] = ["error1", "error2"],
            ["field2"] = ["error3"]
        };

        // Act
        var error = new Error(code, message, Details: details);

        // Assert
        Assert.Equal(details, error.Details);
        Assert.Equal(2, error.Details!["field1"].Length);
    }

    [Fact]
    public void WhenCreatingErrorWithExceptionThenExceptionIsSet()
    {
        // Arrange
        const ErrorCode code = ErrorCode.HttpError;
        const string message = "Test message";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var error = Error.Create(code, message, innerException);

        // Assert
        Assert.Equal(innerException, error.InnerException);
    }

    [Fact]
    public void WhenComparingEqualErrorsThenReturnsTrue()
    {
        // Arrange
        var error1 = Error.Create(ErrorCode.NotFound, "Message");
        var error2 = Error.Create(ErrorCode.NotFound, "Message");

        // Act & Assert
        Assert.Equal(error1, error2);
    }

    [Fact]
    public void WhenComparingDifferentErrorsThenReturnsFalse()
    {
        // Arrange
        var error1 = Error.Create(ErrorCode.NotFound, "Message");
        var error2 = Error.Create(ErrorCode.BadRequest, "Message");

        // Act & Assert
        Assert.NotEqual(error1, error2);
    }
}
