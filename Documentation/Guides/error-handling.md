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

[!code-csharp[Program.cs](ErrorHandling/Program.cs#pattern-matching)]

## Using Properties

For simpler cases, you can use the helper properties:

[!code-csharp[Program.cs](ErrorHandling/Program.cs#properties)]

## TryGet Methods

For a more traditional approach:

[!code-csharp[Program.cs](ErrorHandling/Program.cs#tryget)]

## The Match Method

Use `Match` for exhaustive handling:

[!code-csharp[Program.cs](ErrorHandling/Program.cs#match)]

## Chaining Operations

### Select

Transform successful values while propagating failures:

```csharp
var modelNames = result.Select(paged => paged.Items.Select(m => m.Name).ToList());
```

### SelectMany

Chain multiple operations that return results:

```csharp
var modelDetails = await apiClient.Models
    .FirstOrDefaultAsync()
    .SelectManyAsync(async model => 
        model is not null 
            ? await apiClient.Images.WhereModelId(model.Id).FirstOrDefaultAsync()
            : new Result<Image?>.Success(null));
```

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

Error codes are organized by category:

### General Errors (0-99)

| Code | Description |
|------|-------------|
| `Unknown` (0) | An unknown or unclassified error occurred |

### HTTP and Network Errors (100-199)

| Code | Description |
|------|-------------|
| `HttpError` (100) | General HTTP communication error |
| `Timeout` (101) | Request timed out |
| `BadRequest` (102) | Server returned 400 |
| `NotFound` (103) | Resource not found (404) |
| `Conflict` (104) | Conflict with current state (409) |
| `ServerError` (105) | Server error (500) |
| `BadGateway` (106) | Bad gateway (502) |
| `ServiceUnavailable` (107) | Service unavailable (503) |
| `GatewayTimeout` (108) | Gateway timeout (504) |
| `MethodNotAllowed` (109) | HTTP method not allowed (405) |
| `NotAcceptable` (110) | Cannot produce response matching Accept headers (406) |
| `NoContent` (111) | Response contains no content (204) |
| `MovedPermanently` (112) | Resource permanently moved (301) |
| `TemporaryRedirect` (113) | Resource temporarily redirected (307) |
| `PermanentRedirect` (114) | Resource permanently redirected (308) |
| `UnavailableForLegalReasons` (115) | Unavailable for legal reasons (451) |
| `RequestHeaderFieldsTooLarge` (116) | Request header fields too large (431) |
| `NotImplemented` (117) | Functionality not implemented (501) |
| `HttpVersionNotSupported` (118) | HTTP version not supported (505) |
| `LoopDetected` (119) | Loop detected during processing (508) |

### Authentication Errors (200-299)

| Code | Description |
|------|-------------|
| `Unauthorized` (200) | Not authenticated (401) |
| `Forbidden` (201) | Not authorized to access resource (403) |
| `InsufficientCredits` (202) | Insufficient credits (402) |

### Rate Limiting Errors (300-399)

| Code | Description |
|------|-------------|
| `RateLimited` (300) | Rate limit exceeded (429) |

### Validation Errors (400-499)

| Code | Description |
|------|-------------|
| `InvalidUrl` (400) | URL was invalid or malformed |
| `InvalidParameter` (401) | Required parameter missing or invalid |
| `ValidationFailed` (402) | Request validation failed |

### Serialization Errors (500-599)

| Code | Description |
|------|-------------|
| `DeserializationFailed` (500) | Failed to deserialize JSON response |
| `EmptyResponse` (501) | Response was empty or null |
| `UnexpectedContentType` (502) | Response was not JSON (HTML or other content type) |
| `CloudflareError` (503) | Cloudflare error page received (origin server unreachable, DDoS protection, or CDN rate limiting) |

### File and I/O Errors (600-699)

| Code | Description |
|------|-------------|
| `FileNotFound` (600) | Specified file was not found |
| `IoError` (601) | File I/O operation failed |
| `StreamNotReadable` (602) | Stream was not readable |
| `HashComputationFailed` (603) | Hash computation failed |
| `FileWriteFailed` (604) | Failed to write to file |

### Resource State Errors (700-799)

| Code | Description |
|------|-------------|
| `ResourceUnavailable` (700) | Resource is not available or not ready |

## Handling Specific Errors

[!code-csharp[Program.cs](ErrorHandling/Program.cs#specific-errors)]

## Rate Limiting

When rate limited, the error includes retry information:

```csharp
if (result is Result<PagedResult<Model>>.Failure { Error: { Code: ErrorCode.RateLimited } error })
{
    if (error.RetryAfter.HasValue)
    {
        Console.WriteLine($"Rate limited. Retry after: {error.RetryAfter.Value.TotalSeconds} seconds");
        await Task.Delay(error.RetryAfter.Value);
        // Retry the request
    }
}
```

## OnSuccess and OnFailure

For side effects without transforming the result:

```csharp
await apiClient.Models
    .WhereName("example")
    .FirstOrDefaultAsync()
    .OnSuccess(model => Console.WriteLine($"Found: {model?.Name}"))
    .OnFailure(error => _logger.LogError("Query failed: {Message}", error.Message));
```

## Best Practices

1. **Always handle failures** - Don't ignore the result; handle both cases
2. **Use pattern matching** - It's the most expressive and safe approach
3. **Check error codes** - Use specific handling for known error types
4. **Log errors** - Include trace IDs for debugging
5. **Handle rate limits** - Implement exponential backoff for rate-limited requests

## Next Steps

- [Pagination](pagination.md) - Navigate large result sets
- [Request Builders](request-builders.md) - Master the fluent API pattern
