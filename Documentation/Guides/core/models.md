---
title: Working with Models
description: Query AI models from Civitai including checkpoints, LoRAs, embeddings, VAEs, and ControlNets using the ModelBuilder API.
---

# Working with Models

The `ModelBuilder` allows you to query AI models from the Civitai database. Models include checkpoints, LoRAs, embeddings, VAEs, and other AI resources.

## Model Types

Civitai hosts many types of AI models:

| Type | Description |
|------|-------------|
| `Checkpoint` | Full Stable Diffusion models |
| `Lora` | Low-Rank Adaptation models for fine-tuning |
| `TextualInversion` | Embeddings for textual inversion |
| `Controlnet` | ControlNet models for image guidance |
| `Hypernetwork` | Hypernetwork models |
| `AestheticGradient` | Aesthetic gradient models |
| `Vae` | Variational Auto-Encoder models |
| `Poses` | Pose reference files |
| `Wildcards` | Wildcard text files |
| `MotionModule` | AnimateDiff motion modules |
| `Upscaler` | Image upscaling models |
| `Workflows` | ComfyUI/InvokeAI workflows |

## Querying Models

### By Type

[!code-csharp[Program.cs](examples/Models/Program.cs#ByType)]

### By Tag

[!code-csharp[Program.cs](examples/Models/Program.cs#ByTag)]

### By Creator

[!code-csharp[Program.cs](examples/Models/Program.cs#ByCreator)]

### By Name Search

[!code-csharp[Program.cs](examples/Models/Program.cs#ByName)]

### Getting a Specific Model

[!code-csharp[Program.cs](examples/Models/Program.cs#ById)]

### Getting a Model Version by ID

[!code-csharp[Program.cs](examples/Models/Program.cs#ByVersionId)]

### Getting a Model Version by Hash

[!code-csharp[Program.cs](examples/Models/Program.cs#ByVersionHash)]

## The Model Record

The `Model` record contains comprehensive information:

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `long` | Unique identifier |
| `Name` | `string` | Model name |
| `Description` | `string?` | HTML description |
| `Type` | `ModelType` | Model type enum |
| `IsNsfw` | `bool` | Whether model is NSFW |
| `NsfwLevel` | `int` | Numeric NSFW level |
| `Tags` | `IReadOnlyList<string>?` | Associated tags |
| `Creator` | `Creator?` | Creator information |
| `Stats` | `ModelStats?` | Download/rating statistics |
| `ModelVersions` | `IReadOnlyList<ModelVersion>?` | All versions |
| `SupportsGeneration` | `bool` | Whether it supports generation |
| `DownloadUrl` | `string?` | Direct download URL |

### Model Versions

Each model can have multiple versions. The `ModelVersion` record contains detailed information about a specific model version:

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `long` | Unique version identifier |
| `Name` | `string` | Version name |
| `BaseModel` | `string` | Base model (e.g., "SDXL 1.0", "SD 1.5") |
| `BaseModelType` | `string?` | Base model type classification |
| `Description` | `string?` | Version changelog/description |
| `CreatedAt` | `DateTime` | Creation timestamp |
| `PublishedAt` | `DateTime?` | Publication timestamp |
| `Status` | `string?` | Publication status |
| `Availability` | `Availability?` | Availability status |
| `NsfwLevel` | `int` | NSFW content level |
| `DownloadUrl` | `string?` | Direct download URL |
| `SupportsGeneration` | `bool` | Whether it supports generation |
| `TrainedWords` | `IReadOnlyList<string>?` | Trigger words for generation |
| `Files` | `IReadOnlyList<ModelFile>?` | Associated files |
| `Images` | `IReadOnlyList<ModelVersionImage>?` | Gallery images |
| `Stats` | `ModelVersionStats?` | Download/rating statistics |
| `AirIdentifier` | `string?` | AIR artifact identifier |

#### Working with Versions

[!code-csharp[Program.cs](examples/Models/Program.cs#WorkingWithVersions)]

#### Version-Specific Information

Access detailed version metadata:

[!code-csharp[Program.cs](examples/Models/Program.cs#VersionSpecificInformation)]

### Model Statistics

Access download counts and user feedback:

[!code-csharp[Program.cs](examples/Models/Program.cs#ModelStatistics)]

## Filtering by Permissions

Filter models by their usage permissions:

[!code-csharp[Program.cs](examples/Models/Program.cs#Permissions)]

## Authenticated Queries

Some queries require authentication with an API key. Configure your API key in the service registration:

[!code-csharp[Program.cs](../examples/Common/Program.cs#CoreSetupWithApiKey)]

### Favorites

[!code-csharp[Program.cs](examples/Models/Program.cs#Favorites)]

### Hidden Models

[!code-csharp[Program.cs](examples/Models/Program.cs#HiddenModels)]

## Sorting and Periods

### Sorting

[!code-csharp[Program.cs](examples/Models/Program.cs#Sorting)]

Available sort options:
- `HighestRated`
- `MostDownloaded`
- `Newest`

### Time Periods

| Period | Description |
|--------|-------------|
| `Day` | Last 24 hours |
| `Week` | Last 7 days |
| `Month` | Last 30 days |
| `Year` | Last 365 days |
| `AllTime` | All time |

## API Parameter Behavior Notes

### Authentication Required Parameters

Some filters require authentication via an API key:

- **`WhereFavorites()`** - Returns only the authenticated user's favorited models. Without authentication, returns 0 results.
- **`WhereHidden()`** - Returns only models the authenticated user has hidden. Without authentication, returns 0 results.
- **`WhereNsfw(true)`** - Filtering for NSFW-only models requires authentication. Without an API key, this filter will return no results.

Configure your API key in the service registration:

[!code-csharp[Program.cs](examples/Configuration/Program.cs#ActionDelegate)]

### Known Parameter Quirks

Based on API testing, be aware of these behaviors. For comprehensive details and workarounds, see the [API Behavior and Quirks](api-quirks.md) guide.

#### Commercial Use Permissions

The `WhereCommercialUse()` filter requires **at least two permission values** to return results. See [API Quirks - Commercial Use](api-quirks.md#commercial-use-permissions) for detailed explanation and examples.

#### License Filters

The following boolean filters have shown inconsistent behavior during testing:

- **`WhereAllowNoCredit()`** - May return 0 results regardless of value
- **`WhereAllowDerivatives()`** - May return 0 results regardless of value

These behaviors could be API-side data conditions or undocumented requirements. Use with caution and verify results.

#### Array Parameters

When using filters with multiple values:

- **Model IDs (`WhereIds`)**: Both repeated parameters (`ids=1&ids=2`) and comma-separated (`ids=1,2`) formats work.
- **Base Models (`WhereBaseModels`)**: Only repeated parameters work (`baseModels=SD%201.5&baseModels=SDXL%201.0`). Comma-separated values return a 400 Bad Request error.

CivitaiSharp automatically handles URL encoding and uses the safest format (repeated parameters) for all array filters.

### Endpoint Reliability

The **`/creators` endpoint** has shown reliability issues including:
- Request timeouts (10+ seconds with no response)
- Occasional 500 Internal Server Error responses

The library includes automatic retry and timeout handling via resilience policies, but persistent issues may still occur.

## Next Steps

- [Working with Images](images.md) - Query generated images
- [Error Handling](error-handling.md) - Handle API errors gracefully
- [Pagination](pagination.md) - Navigate large result sets
