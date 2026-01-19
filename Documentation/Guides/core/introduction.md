---
title: CivitaiSharp.Core Introduction
description: Learn about CivitaiSharp.Core, a low-level typed .NET client for the Civitai API with fluent builders, immutable records, and result-based error handling.
---

# CivitaiSharp.Core

CivitaiSharp.Core provides a low-level, typed client for the [Civitai public API](https://github.com/civitai/civitai/wiki/REST-API-Reference). It gives you direct access to query models, images, tags, and creators with a fluent builder pattern.

## Key Features

- **Fluent Request Builders** - Compose complex queries with an intuitive, chainable API
- **Immutable and Thread-Safe** - Builders are immutable records, safe to share across threads
- **Typed Models** - Strongly-typed response models for all API entities
- **Result Pattern** - Explicit error handling without exceptions
- **Pagination Support** - Built-in cursor-based pagination with metadata

## Getting Started

### Installation

```bash
dotnet add package CivitaiSharp.Core --prerelease
```

### Registration

Register the API client using dependency injection:

[!code-csharp[Program.cs](../examples/Common/Program.cs#CoreBasicSetup)]

> [!NOTE]
> The Core library can query public endpoints (models, images, tags, creators) without an API key. An API key is only needed for authenticated features like favorites, hidden models, higher rate limits, and accessing NSFW content (e.g., `WhereNsfw(true)` or `ImageNsfwLevel.Mature`/`X`). See [Pagination](pagination.md) for endpoint-specific pagination methods.

### Basic Usage

[!code-csharp[Program.cs](../examples/Common/Program.cs#ResultPatternMatching)]

## Architecture

The Core library is organized around four main concepts:

### 1. API Client

The `IApiClient` interface exposes cached builder instances for `Models`, `Images`, `Tags`, and `Creators`.

### 2. Request Builders

Each builder provides fluent methods for filtering, sorting, and pagination. Builders are immutable records where each method returns a new instance:

[!code-csharp[Program.cs](examples/RequestBuilders/Program.cs#Immutability)]

### 3. Response Models

All entities are modeled as immutable records with proper JSON serialization attributes. See the [Models guide](models.md) for details.

### 4. Result Pattern

Operations return `Result<T>` which can be either `Success` or `Failure`, enabling explicit error handling without exceptions. See the [Error Handling guide](error-handling.md).

## Next Steps

- [Quick Start](quick-start.md) - Get up and running in minutes
- [Request Builders](request-builders.md) - Master the fluent builder pattern
- [Models Guide](models.md) - Query and filter AI models
- [Error Handling](error-handling.md) - Work with the Result pattern
