namespace CivitaiSharp.Core.Response;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using CivitaiSharp.Core.Models.Common;

/// <summary>
/// Represents an error encountered during an operation. Contains a typed <see cref="ErrorCode"/>
/// for programmatic handling, plus a human-readable message and optional context for diagnostics.
/// Use pattern matching on <see cref="Code"/> to handle specific error conditions, and check
/// <see cref="Details"/> for field-level validation errors.
/// </summary>
/// <param name="Code">The typed error code for programmatic error handling.</param>
/// <param name="Message">The human-readable error message describing what went wrong.</param>
/// <param name="Details">Optional field-level error details from RFC 7807 validation errors.</param>
/// <param name="InnerException">Optional inner exception providing additional context.</param>
/// <param name="HttpStatusCode">The HTTP status code if this error originated from an HTTP response.</param>
/// <param name="RetryAfter">The retry delay from the Retry-After header (typically for rate limiting).</param>
/// <param name="TraceId">The trace identifier for correlating with server logs.</param>
public sealed record Error(
    ErrorCode Code,
    string Message,
    IReadOnlyDictionary<string, string[]>? Details = null,
    Exception? InnerException = null,
    HttpStatusCode? HttpStatusCode = null,
    TimeSpan? RetryAfter = null,
    string? TraceId = null)
{
    /// <summary>
    /// Creates an error with the specified code and message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error"/> instance.</returns>
    public static Error Create(ErrorCode code, string message) => new(code, message);

    /// <summary>
    /// Creates an error with the specified code, message, and inner exception.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <returns>A new <see cref="Error"/> instance.</returns>
    public static Error Create(ErrorCode code, string message, Exception innerException) =>
        new(code, message, InnerException: innerException);
}

/// <summary>
/// Represents a page of items along with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
/// <param name="Items">The items in this page.</param>
/// <param name="Metadata">Optional pagination metadata for navigating to other pages.</param>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    PaginationMetadata? Metadata = null);

/// <summary>
/// Discriminated union representing the result of an operation as either success with data or failure
/// with error information. Use pattern matching for exhaustive handling of success and error cases.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public abstract record Result<T>
{
    /// <summary>
    /// Represents a successful operation result.
    /// </summary>
    /// <param name="Data">The data returned from the successful operation.</param>
    public sealed record Success(T Data) : Result<T>;

    /// <summary>
    /// Represents a failed operation result.
    /// </summary>
    /// <param name="Error">The error information describing what failed.</param>
    public sealed record Failure(Error Error) : Result<T>;

    private Result() { }

    /// <summary>
    /// Gets a value indicating whether this result represents a successful outcome.
    /// </summary>
    [MemberNotNullWhen(true, nameof(ValueOrDefault))]
    public bool IsSuccess => this is Success;

    /// <summary>
    /// Gets a value indicating whether this result represents a failed outcome.
    /// </summary>
    [MemberNotNullWhen(true, nameof(ErrorOrDefault))]
    public bool IsFailure => this is Failure;

    /// <summary>
    /// Gets the data value if this is a successful result, otherwise null.
    /// Use this for safe access without exceptions. Prefer pattern matching for most cases.
    /// </summary>
    public T? ValueOrDefault => this is Success success ? success.Data : default;

    /// <summary>
    /// Gets the error information if this is a failed result, otherwise null.
    /// Use this for safe access without exceptions. Prefer pattern matching for most cases.
    /// </summary>
    public Error? ErrorOrDefault => this is Failure failure ? failure.Error : null;

    /// <summary>
    /// Gets the data value from a successful result.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if this is a failure result.</exception>
    public T Value => this is Success success
        ? success.Data
        : throw new InvalidOperationException("Cannot access Value on a failed result. Check IsSuccess before accessing.");

    /// <summary>
    /// Gets the error information from a failed result.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if this is a successful result.</exception>
    public Error ErrorInfo => this is Failure failure
        ? failure.Error
        : throw new InvalidOperationException("Cannot access ErrorInfo on a successful result. Check IsFailure before accessing.");

    /// <summary>
    /// Attempts to get the success value.
    /// </summary>
    /// <param name="value">When this method returns, contains the success value if the result is successful; otherwise, the default value for type <typeparamref name="T"/>.</param>
    /// <returns>True if this result is successful; otherwise, false.</returns>
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        if (this is Success success)
        {
            value = success.Data;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to get the error information.
    /// </summary>
    /// <param name="error">When this method returns, contains the error if the result is a failure; otherwise, null.</param>
    /// <returns>True if this result is a failure; otherwise, false.</returns>
    public bool TryGetError([NotNullWhen(true)] out Error? error)
    {
        if (this is Failure failure)
        {
            error = failure.Error;
            return true;
        }

        error = null;
        return false;
    }

    /// <summary>
    /// Transforms the success value using the provided function, or passes through the failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the transformed value.</typeparam>
    /// <param name="selector">The function to transform a successful value.</param>
    /// <returns>A new result with the transformed value or the original failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when selector is null.</exception>
    public Result<TResult> Select<TResult>(Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return this switch
        {
            Success success => new Result<TResult>.Success(selector(success.Data)),
            Failure failure => new Result<TResult>.Failure(failure.Error),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    /// <summary>
    /// Transforms the success value using a function that returns a new result, enabling chaining of operations.
    /// </summary>
    /// <typeparam name="TResult">The type of the value in the resulting Result.</typeparam>
    /// <param name="selector">The function to transform a successful value into a new result.</param>
    /// <returns>The result of the selector function, or the original failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when selector is null.</exception>
    public Result<TResult> SelectMany<TResult>(Func<T, Result<TResult>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return this switch
        {
            Success success => selector(success.Data),
            Failure failure => new Result<TResult>.Failure(failure.Error),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    /// <summary>
    /// Asynchronously transforms the success value using a function that returns a new result.
    /// </summary>
    /// <typeparam name="TResult">The type of the value in the resulting Result.</typeparam>
    /// <param name="selector">The async function to transform a successful value into a new result.</param>
    /// <returns>A task containing the result of the selector function, or the original failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when selector is null.</exception>
    public async Task<Result<TResult>> SelectManyAsync<TResult>(Func<T, Task<Result<TResult>>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return this switch
        {
            Success success => await selector(success.Data).ConfigureAwait(false),
            Failure failure => new Result<TResult>.Failure(failure.Error),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }

    /// <summary>
    /// Returns the success value if present, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The value to return if this is a failure.</param>
    /// <returns>The success value or the default value.</returns>
    public T GetValueOrDefault(T defaultValue)
    {
        return this is Success success ? success.Data : defaultValue;
    }

    /// <summary>
    /// Returns the success value if present, otherwise invokes the factory function to get a default value.
    /// </summary>
    /// <param name="defaultFactory">A function that produces the default value.</param>
    /// <returns>The success value or the result of the factory function.</returns>
    /// <exception cref="ArgumentNullException">Thrown when defaultFactory is null.</exception>
    public T GetValueOrDefault(Func<T> defaultFactory)
    {
        ArgumentNullException.ThrowIfNull(defaultFactory);
        return this is Success success ? success.Data : defaultFactory();
    }

    /// <summary>
    /// Executes an action if the result is successful.
    /// </summary>
    /// <param name="action">The action to execute with the success value.</param>
    /// <returns>This result for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    public Result<T> OnSuccess(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (this is Success success)
        {
            action(success.Data);
        }
        return this;
    }

    /// <summary>
    /// Executes an action if the result is a failure.
    /// </summary>
    /// <param name="action">The action to execute with the error.</param>
    /// <returns>This result for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
    public Result<T> OnFailure(Action<Error> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (this is Failure failure)
        {
            action(failure.Error);
        }
        return this;
    }

    /// <summary>
    /// Matches the result to one of two functions based on success or failure.
    /// </summary>
    /// <typeparam name="TResult">The return type of the match functions.</typeparam>
    /// <param name="onSuccess">Function to invoke if this is a success.</param>
    /// <param name="onFailure">Function to invoke if this is a failure.</param>
    /// <returns>The result of the invoked function.</returns>
    /// <exception cref="ArgumentNullException">Thrown when onSuccess or onFailure is null.</exception>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return this switch
        {
            Success success => onSuccess(success.Data),
            Failure failure => onFailure(failure.Error),
            _ => throw new InvalidOperationException("Unexpected result type.")
        };
    }
}
