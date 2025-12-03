---
title: Request Builders
description: Master the fluent builder pattern in CivitaiSharp for constructing type-safe, composable API queries with immutable records.
---

# Request Builders

CivitaiSharp.Core uses a fluent builder pattern for constructing API queries. This pattern provides an intuitive, composable, and type-safe way to build complex queries.

## Design Principles

### Immutability

All builders are implemented as immutable `record` types. Each fluent method returns a **new** builder instance with the updated configuration, leaving the original unchanged:

```csharp
var baseQuery = apiClient.Models.WhereType(ModelType.Lora);

// baseQuery is NOT modified - a new instance is created
var animeQuery = baseQuery.WhereTag("anime");
var realisticQuery = baseQuery.WhereTag("realistic");

// All three are independent queries
```

### Thread Safety

Because builders are immutable, they are inherently thread-safe. You can safely:

- Cache and reuse builder configurations
- Share builders across threads
- Build queries concurrently from the same base

### Lazy Execution

Building a query does not execute it. The request is only sent when you call an execution method like `ExecuteAsync()` or `FirstOrDefaultAsync()`.

## Available Builders

| Builder | Description | Endpoint |
|---------|-------------|----------|
| `ModelBuilder` | Query AI models | `/api/v1/models` |
| `ImageBuilder` | Query generated images | `/api/v1/images` |
| `TagBuilder` | Query available tags | `/api/v1/tags` |
| `CreatorBuilder` | Query content creators | `/api/v1/creators` |

## Common Operations

### Filtering

Use `Where*` methods to filter results:

[!code-csharp[Program.cs](RequestBuilders/Program.cs#filtering)]

### Sorting

Use `OrderBy` methods where available:

[!code-csharp[Program.cs](RequestBuilders/Program.cs#sorting)]

### Limiting Results

Pass `resultsLimit` to `ExecuteAsync` to control page size:

[!code-csharp[Program.cs](RequestBuilders/Program.cs#limiting)]

### Getting a Single Item

Use `GetByIdAsync` for direct lookups or `FirstOrDefaultAsync` for the first match:

[!code-csharp[Program.cs](RequestBuilders/Program.cs#single-item)]

## ModelBuilder Reference

The `ModelBuilder` provides the most comprehensive set of filters:

### Text Filters

| Method | Description |
|--------|-------------|
| `WhereName(string)` | Filter by model name (search query) |
| `WhereTag(string)` | Filter by a single tag |
| `WhereUsername(string)` | Filter by creator username |

### Type Filters

| Method | Description |
|--------|-------------|
| `WhereType(ModelType)` | Filter by model type (Checkpoint, Lora, TextualInversion, etc.) |

### Boolean Filters

| Method | Description |
|--------|-------------|
| `WhereFavorites()` | Only favorited models (requires authentication) |
| `WhereHidden()` | Only hidden models (requires authentication) |
| `WherePrimaryFileOnly()` | Only include primary file models |
| `WhereSupportsGeneration(bool)` | Filter models by generation capability support |

### Permission Filters

| Method | Description |
|--------|-------------|
| `WhereAllowNoCredit(bool)` | Filter by no-credit permission |
| `WhereAllowDerivatives(bool)` | Filter by derivatives permission |
| `WhereAllowDifferentLicenses(bool)` | Filter by different licenses permission |
| `WhereCommercialUse(params CommercialUsePermission[])` | Filter by commercial use levels (multiple values = OR logic) |

### Content Filters

| Method | Description |
|--------|-------------|
| `WhereNsfw(bool)` | Filter by NSFW status (true = only NSFW, false = exclude NSFW) |

### ID Filters

| Method | Description |
|--------|-------------|
| `WhereIds(params long[])` | Filter by specific model IDs |
| `WhereBaseModel(string)` | Filter by base model type (e.g., "SD 1.5", "SDXL 1.0") |
| `WhereBaseModels(params string[])` | Filter by multiple base model types |

### Pagination and Sorting

| Method | Description |
|--------|-------------|
| `WithPageIndex(int)` | Set page index for pagination |
| `OrderBy(ModelSort)` | Sort results (HighestRated, MostDownloaded, Newest) |
| `WherePeriod(TimePeriod)` | Filter by time period |
| `ExecuteAsync(resultsLimit, cursor)` | Execute query with optional limit (1-100) and cursor |

## ImageBuilder Reference

| Method | Description |
|--------|-------------|
| `WhereModelId(long)` | Filter by model ID |
| `WhereModelVersionId(long)` | Filter by model version ID |
| `WherePostId(long)` | Filter by post ID |
| `WhereUsername(string)` | Filter by creator username |
| `WhereNsfwLevel(ImageNsfwLevel)` | Filter by NSFW level |
| `OrderBy(ImageSort)` | Sort results (MostReactions, MostComments, MostCollected, Newest, Oldest, Random) |
| `WherePeriod(TimePeriod)` | Filter by time period |
| `ExecuteAsync(resultsLimit, cursor)` | Execute query with optional limit (1-200) and cursor |

## Validation

Builders validate parameters immediately when methods are called:

```csharp
// Throws ArgumentException - name cannot be null or whitespace
apiClient.Models.WhereName("");

// Throws ArgumentOutOfRangeException - ID must be positive
apiClient.Models.GetByIdAsync(0);

// Throws ArgumentOutOfRangeException - limit must be 1-100
apiClient.Models.ExecuteAsync(resultsLimit: 500);
```

## Next Steps

- [Working with Models](models.md) - Deep dive into model queries
- [Working with Images](images.md) - Query generated images
- [Pagination](pagination.md) - Navigate large result sets
