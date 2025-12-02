namespace CivitaiSharp.Core.Tests.Response;

using CivitaiSharp.Core.Response;
using Xunit;

public sealed class ResultTests
{
    [Fact]
    public void WhenCreatingSuccessResultThenIsSuccessReturnsTrue()
    {
        // Arrange & Act
        var result = new Result<string>.Success("test data");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void WhenCreatingFailureResultThenIsFailureReturnsTrue()
    {
        // Arrange
        var error = Error.Create(ErrorCode.Unknown, "Test error message");

        // Act
        var result = new Result<string>.Failure(error);

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void WhenAccessingValueOnSuccessResultThenReturnsData()
    {
        // Arrange
        var expectedData = "test data";
        var result = new Result<string>.Success(expectedData);

        // Act
        var actualData = result.Value;

        // Assert
        Assert.Equal(expectedData, actualData);
    }

    [Fact]
    public void WhenAccessingValueOnFailureResultThenThrowsInvalidOperationException()
    {
        // Arrange
        var error = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<string>.Failure(error);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => result.Value);
        Assert.Contains("Check IsSuccess", exception.Message);
    }

    [Fact]
    public void WhenAccessingErrorInfoOnFailureResultThenReturnsError()
    {
        // Arrange
        var expectedError = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<string>.Failure(expectedError);

        // Act
        var actualError = result.ErrorInfo;

        // Assert
        Assert.Equal(expectedError, actualError);
    }

    [Fact]
    public void WhenAccessingErrorInfoOnSuccessResultThenThrowsInvalidOperationException()
    {
        // Arrange
        var result = new Result<string>.Success("test data");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => result.ErrorInfo);
        Assert.Contains("Check IsFailure", exception.Message);
    }

    [Fact]
    public void WhenAccessingValueOrDefaultOnSuccessResultThenReturnsData()
    {
        // Arrange
        var expectedData = "test data";
        var result = new Result<string>.Success(expectedData);

        // Act
        var actualData = result.ValueOrDefault;

        // Assert
        Assert.Equal(expectedData, actualData);
    }

    [Fact]
    public void WhenAccessingValueOrDefaultOnFailureResultThenReturnsDefault()
    {
        // Arrange
        var error = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<string>.Failure(error);

        // Act
        var actualData = result.ValueOrDefault;

        // Assert
        Assert.Null(actualData);
    }

    [Fact]
    public void WhenAccessingErrorOrDefaultOnFailureResultThenReturnsError()
    {
        // Arrange
        var expectedError = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<string>.Failure(expectedError);

        // Act
        var actualError = result.ErrorOrDefault;

        // Assert
        Assert.Equal(expectedError, actualError);
    }

    [Fact]
    public void WhenAccessingErrorOrDefaultOnSuccessResultThenReturnsNull()
    {
        // Arrange
        var result = new Result<string>.Success("test data");

        // Act
        var actualError = result.ErrorOrDefault;

        // Assert
        Assert.Null(actualError);
    }

    [Fact]
    public void WhenCallingSelectOnSuccessResultThenTransformsValue()
    {
        // Arrange
        var result = new Result<int>.Success(5);

        // Act
        var transformed = result.Select(x => x * 2);

        // Assert
        Assert.True(transformed.IsSuccess);
        Assert.Equal(10, transformed.Value);
    }

    [Fact]
    public void WhenCallingSelectOnFailureResultThenPassesThroughError()
    {
        // Arrange
        var error = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<int>.Failure(error);

        // Act
        var transformed = result.Select(x => x * 2);

        // Assert
        Assert.True(transformed.IsFailure);
        Assert.Equal(error, transformed.ErrorInfo);
    }

    [Fact]
    public void WhenCallingSelectWithNullSelectorThenThrowsArgumentNullException()
    {
        // Arrange
        var result = new Result<int>.Success(5);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => result.Select<string>(null!));
    }

    [Fact]
    public void WhenCallingSelectManyOnSuccessResultThenChainsResults()
    {
        // Arrange
        var result = new Result<int>.Success(5);
        static Result<string> Selector(int x) => new Result<string>.Success($"Value: {x}");

        // Act
        var chained = result.SelectMany(Selector);

        // Assert
        Assert.True(chained.IsSuccess);
        Assert.Equal("Value: 5", chained.Value);
    }

    [Fact]
    public void WhenCallingSelectManyOnFailureResultThenPassesThroughError()
    {
        // Arrange
        var error = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<int>.Failure(error);
        static Result<string> Selector(int x) => new Result<string>.Success($"Value: {x}");

        // Act
        var chained = result.SelectMany(Selector);

        // Assert
        Assert.True(chained.IsFailure);
        Assert.Equal(error, chained.ErrorInfo);
    }

    [Fact]
    public void WhenCallingSelectManyAndSelectorReturnsFailureThenReturnsFailure()
    {
        // Arrange
        var result = new Result<int>.Success(5);
        var expectedError = Error.Create(ErrorCode.Unknown, "Selector failed");
        Result<string> Selector(int _) => new Result<string>.Failure(expectedError);

        // Act
        var chained = result.SelectMany(Selector);

        // Assert
        Assert.True(chained.IsFailure);
        Assert.Equal(expectedError, chained.ErrorInfo);
    }

    [Fact]
    public async Task WhenCallingSelectManyAsyncOnSuccessResultThenChainsResults()
    {
        // Arrange
        var result = new Result<int>.Success(5);
        static Task<Result<string>> Selector(int x) => Task.FromResult<Result<string>>(new Result<string>.Success($"Value: {x}"));

        // Act
        var chained = await result.SelectManyAsync(Selector);

        // Assert
        Assert.True(chained.IsSuccess);
        Assert.Equal("Value: 5", chained.Value);
    }

    [Fact]
    public async Task WhenCallingSelectManyAsyncOnFailureResultThenPassesThroughError()
    {
        // Arrange
        var error = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<int>.Failure(error);
        static Task<Result<string>> Selector(int x) => Task.FromResult<Result<string>>(new Result<string>.Success($"Value: {x}"));

        // Act
        var chained = await result.SelectManyAsync(Selector);

        // Assert
        Assert.True(chained.IsFailure);
        Assert.Equal(error, chained.ErrorInfo);
    }

    [Fact]
    public void WhenCallingGetValueOrDefaultWithValueOnSuccessResultThenReturnsValue()
    {
        // Arrange
        var result = new Result<string>.Success("actual");

        // Act
        var value = result.GetValueOrDefault("default");

        // Assert
        Assert.Equal("actual", value);
    }

    [Fact]
    public void WhenCallingGetValueOrDefaultWithValueOnFailureResultThenReturnsDefault()
    {
        // Arrange
        var error = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<string>.Failure(error);

        // Act
        var value = result.GetValueOrDefault("default");

        // Assert
        Assert.Equal("default", value);
    }

    [Fact]
    public void WhenCallingGetValueOrDefaultWithFactoryOnSuccessResultThenReturnsValue()
    {
        // Arrange
        var result = new Result<string>.Success("actual");
        var factoryCalled = false;

        // Act
        var value = result.GetValueOrDefault(() =>
        {
            factoryCalled = true;
            return "default";
        });

        // Assert
        Assert.Equal("actual", value);
        Assert.False(factoryCalled);
    }

    [Fact]
    public void WhenCallingGetValueOrDefaultWithFactoryOnFailureResultThenCallsFactory()
    {
        // Arrange
        var error = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<string>.Failure(error);
        var factoryCalled = false;

        // Act
        var value = result.GetValueOrDefault(() =>
        {
            factoryCalled = true;
            return "default";
        });

        // Assert
        Assert.Equal("default", value);
        Assert.True(factoryCalled);
    }

    [Fact]
    public void WhenCallingOnSuccessWithSuccessResultThenExecutesAction()
    {
        // Arrange
        var result = new Result<string>.Success("test");
        string? capturedValue = null;

        // Act
        var returned = result.OnSuccess(x => capturedValue = x);

        // Assert
        Assert.Equal("test", capturedValue);
        Assert.Same(result, returned);
    }

    [Fact]
    public void WhenCallingOnSuccessWithFailureResultThenDoesNotExecuteAction()
    {
        // Arrange
        var error = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<string>.Failure(error);
        var actionCalled = false;

        // Act
        var returned = result.OnSuccess(_ => actionCalled = true);

        // Assert
        Assert.False(actionCalled);
        Assert.Same(result, returned);
    }

    [Fact]
    public void WhenCallingOnFailureWithFailureResultThenExecutesAction()
    {
        // Arrange
        var expectedError = Error.Create(ErrorCode.Unknown, "Test error message");
        var result = new Result<string>.Failure(expectedError);
        Error? capturedError = null;

        // Act
        var returned = result.OnFailure(e => capturedError = e);

        // Assert
        Assert.Equal(expectedError, capturedError);
        Assert.Same(result, returned);
    }

    [Fact]
    public void WhenCallingOnFailureWithSuccessResultThenDoesNotExecuteAction()
    {
        // Arrange
        var result = new Result<string>.Success("test");
        var actionCalled = false;

        // Act
        var returned = result.OnFailure(_ => actionCalled = true);

        // Assert
        Assert.False(actionCalled);
        Assert.Same(result, returned);
    }

    [Fact]
    public void WhenCallingMatchOnSuccessResultThenCallsSuccessFunction()
    {
        // Arrange
        var result = new Result<int>.Success(5);

        // Act
        var matched = result.Match(
            onSuccess: x => $"Success: {x}",
            onFailure: e => $"Failure: {e.Code}");

        // Assert
        Assert.Equal("Success: 5", matched);
    }

    [Fact]
    public void WhenCallingMatchOnFailureResultThenCallsFailureFunction()
    {
        // Arrange
        var error = Error.Create(ErrorCode.NotFound, "Test error message");
        var result = new Result<int>.Failure(error);

        // Act
        var matched = result.Match(
            onSuccess: x => $"Success: {x}",
            onFailure: e => $"Failure: {e.Code}");

        // Assert
        Assert.Equal("Failure: NotFound", matched);
    }

    [Fact]
    public void WhenCallingMatchWithNullOnSuccessThenThrowsArgumentNullException()
    {
        // Arrange
        var result = new Result<int>.Success(5);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => result.Match(
            onSuccess: null!,
            onFailure: e => $"Failure: {e.Code}"));
    }

    [Fact]
    public void WhenCallingMatchWithNullOnFailureThenThrowsArgumentNullException()
    {
        // Arrange
        var result = new Result<int>.Success(5);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => result.Match(
            onSuccess: x => $"Success: {x}",
            onFailure: null!));
    }
}
