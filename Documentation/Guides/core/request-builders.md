---
title: Request Builders
description: Master the fluent builder pattern in CivitaiSharp for constructing type-safe, composable API queries with immutable records.
---

# Request Builders

CivitaiSharp.Core uses a fluent builder pattern for constructing API queries. This pattern provides an intuitive, composable, and type-safe way to build complex queries.

## Design Principles

### Immutability

All builders are implemented as immutable `record` types. Each fluent method returns a **new** builder instance with the updated configuration, leaving the original unchanged:

[!code-csharp[Program.cs](examples/RequestBuilders/Program.cs#Immutability)]

### Thread Safety

Because builders are immutable, they are inherently thread-safe. You can safely:

- Cache and reuse builder configurations
- Share builders across threads
- Build queries concurrently from the same base

### Lazy Execution

Building a query does not execute it. The request is only sent when you call an execution method like `ExecuteAsync()` or `FirstOrDefaultAsync()`.

## Available Builders

CivitaiSharp provides `ModelBuilder`, `ImageBuilder`, `TagBuilder`, and `CreatorBuilder` for querying their respective endpoints. All builders support filtering, sorting, pagination, and both batch (`ExecuteAsync`) and single-item queries (`FirstOrDefaultAsync`, `GetByIdAsync`).

See the API reference documentation for complete method listings and parameters.

## Common Operations

### Filtering

Use `Where*` methods to filter results:

[!code-csharp[Program.cs](examples/RequestBuilders/Program.cs#Filtering)]

### Sorting

Use `OrderBy` methods where available:

[!code-csharp[Program.cs](examples/RequestBuilders/Program.cs#Sorting)]

### Limiting Results

Pass `resultsLimit` to `ExecuteAsync` to control page size:

[!code-csharp[Program.cs](examples/RequestBuilders/Program.cs#Limiting)]

### Getting a Single Item

Use `GetByIdAsync` for direct lookups or `FirstOrDefaultAsync` for the first match:

[!code-csharp[Program.cs](examples/RequestBuilders/Program.cs#SingleItem)]

## Validation

Builders validate parameters immediately when methods are called:

[!code-csharp[Program.cs](examples/RequestBuilders/Program.cs#Validation)]

## Next Steps

- [Working with Models](models.md) - Deep dive into model queries
- [Working with Images](images.md) - Query generated images
- [Pagination](pagination.md) - Navigate large result sets
