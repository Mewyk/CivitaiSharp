---
title: Error Handling
description: Handle API errors with CivitaiSharp's Result pattern using pattern matching for explicit success and failure handling.
---

# Error Handling

CivitaiSharp uses a Result pattern for error handling instead of throwing exceptions for API errors. This makes error handling explicit and encourages proper handling of failure cases.

## The Result Pattern

All API operations return a `Result<T>` type which is a discriminated union that can be either:

- `Result<T>.Success` - Contains the successful data
- `Result<T>.Failure` - Contains error information

## Pattern Matching

The recommended way to handle results is with pattern matching:

[!code-csharp[Program.cs](../examples/Common/Program.cs#ResultPatternMatching)]

## TryGet Methods

For a more traditional approach:

[!code-csharp[Program.cs](examples/ErrorHandling/Program.cs#TryGet)]

## The Match Method

Use `Match` for exhaustive handling:

[!code-csharp[Program.cs](examples/ErrorHandling/Program.cs#PatternMatching)]

## Chaining Operations

Transform successful values while propagating failures:

[!code-csharp[Program.cs](examples/ErrorHandling/Program.cs#ChainingOperations)]

## The Error Record

When a failure occurs, the `Error` record contains detailed information:

| Property | Type | Description |
|----------|------|-------------|
| `Code` | `ErrorCode` | Typed error code for programmatic handling |
| `Message` | `string` | Human-readable error description |
| `Details` | `IReadOnlyDictionary<string, string[]>?` | Field-level validation errors |
| `InnerException` | `Exception?` | Underlying exception if applicable |
| `HttpStatusCode` | `HttpStatusCode?` | HTTP status code if from HTTP response |
| `RetryAfter` | `TimeSpan?` | Retry delay for rate limiting |
| `TraceId` | `string?` | Trace ID for server-side correlation |

## Error Codes

The `ErrorCode` enum provides typed error codes organized by category. Key codes include:

- **HTTP**: `NotFound`, `Timeout`, `ServerError`, `BadGateway`, `ServiceUnavailable`
- **Authentication**: `Unauthorized`, `Forbidden`
- **Rate Limiting**: `RateLimited` (includes `RetryAfter` timespan)
- **Validation**: `InvalidParameter`, `ValidationFailed`
- **Serialization**: `DeserializationFailed`, `UnexpectedContentType`, `CloudflareError`

See the `ErrorCode` enum documentation for the complete list of error codes and their descriptions.

## Handling Specific Errors

[!code-csharp[Program.cs](examples/ErrorHandling/Program.cs#SpecificErrors)]

## Rate Limiting

When rate limited, the error includes retry information:

[!code-csharp[Program.cs](examples/ErrorHandling/Program.cs#RateLimiting)]

## OnSuccess and OnFailure

For side effects without transforming the result:

[!code-csharp[Program.cs](examples/ErrorHandling/Program.cs#OnSuccessOnFailure)]

## Next Steps

- [API Behavior and Quirks](api-quirks.md) - Understand API-specific behaviors
- [Pagination](pagination.md) - Navigate large result sets
