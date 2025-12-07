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

```csharp
services.AddCivitaiApi(options =>
{
    options.ApiKey = "your-api-key"; // Optional - public endpoints work without a key
});
```

> [!NOTE]
> The Core library can query public endpoints (models, images, tags, creators) without an API key. An API key is only needed for authenticated features like favorites, hidden models, higher rate limits, and accessing NSFW content (e.g., `WhereNsfw(true)` or `ImageNsfwLevel.Mature`/`X`). Note: Images use cursor-based pagination while Models, Tags, and Creators use page-based pagination.

### Basic Usage

```csharp
public class MyService(IApiClient apiClient)
{
    public async Task QueryModelsAsync()
    {
        var result = await apiClient.Models
            .WhereType(ModelType.Lora)
            .WhereTag("anime")
            .ExecuteAsync(resultsLimit: 10);

        if (result is Result<PagedResult<Model>>.Success success)
        {
            foreach (var model in success.Data.Items)
            {
                Console.WriteLine($"{model.Id}: {model.Name}");
            }
        }
    }
}
```

## Architecture

The Core library is organized around four main concepts:

### 1. API Client

The `IApiClient` interface is the entry point. It exposes cached, immutable builder instances:

| Property | Type | Description |
|----------|------|-------------|
| `Models` | `ModelBuilder` | Query AI models (checkpoints, LoRAs, embeddings, etc.) |
| `Images` | `ImageBuilder` | Query generated images |
| `Tags` | `TagBuilder` | Query available tags |
| `Creators` | `CreatorBuilder` | Query content creators |

### 2. Request Builders

Each builder provides fluent methods for filtering, sorting, and pagination. Builders are immutable records where each method returns a new instance:

```csharp
var baseQuery = apiClient.Models.WhereType(ModelType.Lora);

// These create separate queries, baseQuery is unchanged
var animeQuery = baseQuery.WhereTag("anime");
var realisticQuery = baseQuery.WhereTag("realistic");
```

### 3. Response Models

All entities are modeled as immutable records with proper JSON serialization attributes. See the [Models guide](models.md) for details.

### 4. Result Pattern

Operations return `Result<T>` which can be either `Success` or `Failure`, enabling explicit error handling without exceptions. See the [Error Handling guide](error-handling.md).

## Guides

- [Request Builders](request-builders.md) - Master the fluent API pattern
- [Working with Models](models.md) - Query and filter models
- [Working with Images](images.md) - Query generated images
- [Error Handling](error-handling.md) - Handle errors gracefully
- [Pagination](pagination.md) - Navigate large result sets
