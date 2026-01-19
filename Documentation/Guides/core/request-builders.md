---
title: Request Builder Pattern
description: Master the fluent builder pattern in CivitaiSharp for constructing type-safe, composable API queries.
---

# Request Builder Pattern

The CivitaiSharp library relies on a **fluent builder pattern** to construct API requests. This design ensures that all queries are type-safe, validated, and easy to compose.

This guide explains the mechanics of the builder pattern itself. For specific query parameters available for each resource, refer to the guides for [Models](models.md), [Images](images.md), [Tags](tags.md), and [Creators](creators.md).

## Core Design Principles

### Immutability & Thread Safety
All request builders are **immutable records**. When you call a method like `.WhereType(...)` or `.OrderBy(...)`, the original builder is not modified. Instead, a **new copy** of the builder is returned with the updated configuration.

This immutability guarantees thread safety, allowing you to:
- Define a "base query" (e.g., a filter for safe-for-work content) and reuse it across multiple threads or requests.
- Cache builder instances without risk of side effects.

### Lazy Execution
Constructing a builder does not trigger any network activity. The API request is only executing when you call a terminal method such as `ExecuteAsync()`, `FirstOrDefaultAsync()`, or `GetByIdAsync()`.

## Anatomy of a Request

Every request follows a standard lifecycle:

1.  **Entry Point**: Access the builder via the `ApiClient` (e.g., `client.Models`).
2.  **Configuration**: Chain methods to set filters, sorting, and options. Each step validates your input immediately.
3.  **Execution**: Call an async execution method to send the request and retrieve results.

### Example: The Builder Lifecycle

[!code-csharp[Program.cs](examples/RequestBuilders/Program.cs#Immutability)]

## Validation
Input validation occurs immediately when you call a configuration method. If you provide an invalid parameter (like a negative limit or null value), the builder will throw an exception precisely where the error was introduced, rather than waiting for the request to execute.

## Next Steps

Now that you understand how to construct requests, explore the specific capabilities of each resource:

- [Working with Models](models.md)
- [Working with Images](images.md)
- [Working with Tags](tags.md)
- [Working with Creators](creators.md)
