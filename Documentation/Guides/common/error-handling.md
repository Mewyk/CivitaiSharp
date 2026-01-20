---
title: Error Handling
description: Handle API errors with the Result pattern.
---

# Error Handling

All API operations return `Result<T>` (success or failure) instead of throwing exceptions.

## Pattern Matching

[!code-csharp[Program.cs](../core/examples/ErrorHandling/Program.cs#PatternMatching)]

## TryGet

[!code-csharp[Program.cs](../core/examples/ErrorHandling/Program.cs#TryGet)]

## Match Method

[!code-csharp[Program.cs](../core/examples/ErrorHandling/Program.cs#PatternMatching)]

## Chaining

[!code-csharp[Program.cs](../core/examples/ErrorHandling/Program.cs#ChainingOperations)]

## Error Properties

- `Code` - Typed error code
- `Message` - Human-readable description
- `Details` - Field-level validation errors
- `HttpStatusCode` - HTTP status
- `RetryAfter` - Retry delay for rate limits
- `TraceId` - Server correlation ID

## Common Error Codes

**HTTP:** `NotFound`, `Timeout`, `ServerError`

**Auth:** `Unauthorized`, `Forbidden`

**Rate Limit:** `RateLimited` (check `RetryAfter`)

**Validation:** `InvalidParameter`, `ValidationFailed`

## Specific Error Handling

[!code-csharp[Program.cs](../core/examples/ErrorHandling/Program.cs#SpecificErrors)]

## Rate Limiting

[!code-csharp[Program.cs](../core/examples/ErrorHandling/Program.cs#RateLimiting)]

## Side Effects

[!code-csharp[Program.cs](../core/examples/ErrorHandling/Program.cs#OnSuccessOnFailure)]

## Next Steps

- [API Quirks](api-quirks.md)
- [Pagination](pagination.md)
