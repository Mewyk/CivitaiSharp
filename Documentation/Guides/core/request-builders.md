---
title: Request Builder Pattern
description: Fluent builder pattern for type-safe API queries.
---

# Request Builder Pattern

CivitaiSharp uses a fluent builder pattern for constructing type-safe, validated queries.

## Core Principles

**Immutability:** Builders are immutable records. Each method returns a new copy with updated configuration.

**Lazy Execution:** No network activity until you call `ExecuteAsync()`, `FirstOrDefaultAsync()`, or `GetByIdAsync()`.

**Immediate Validation:** Invalid parameters throw immediately, not during execution.

## Request Lifecycle

1. **Entry Point:** Access builder via `ApiClient` (`client.Models`, `client.Images`, etc.)
2. **Configuration:** Chain methods to set filters, sorting, options
3. **Execution:** Call async method to send request and retrieve results

## Example

[!code-csharp[Program.cs](examples/RequestBuilders/Program.cs#Immutability)]

## Next Steps

- [Models](models.md)
- [Images](images.md)
- [Tags](tags.md)
- [Creators](creators.md)
