---
title: Working with Images
description: Query AI-generated images from the Civitai gallery by model, username, or NSFW level using the ImageBuilder API.
---

# Working with Images

The `ImageBuilder` allows you to query generated images from the Civitai gallery. These are images created using AI models and shared by the community.

## Querying Images

### By Model

Find images generated with a specific model:

[!code-csharp[Program.cs](examples/Images/Program.cs#by-model)]

### By Model Version

Find images generated with a specific model version:

[!code-csharp[Program.cs](examples/Images/Program.cs#by-version)]

### By Creator

Find images posted by a specific user:

[!code-csharp[Program.cs](examples/Images/Program.cs#by-creator)]

### By Post

Find all images in a specific post:

[!code-csharp[Program.cs](examples/Images/Program.cs#ByPost)]

## The Image Record

The `Image` record contains:

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `long` | Unique identifier |
| `Url` | `string` | Image URL at source resolution |
| `Hash` | `string?` | Blurhash for placeholder generation |
| `Width` | `int` | Image width in pixels |
| `Height` | `int` | Image height in pixels |
| `NsfwLevel` | `ImageNsfwLevel?` | NSFW classification |
| `Type` | `MediaType?` | Media type (image/video) |
| `CreatedAt` | `DateTime?` | When the image was posted |
| `PostId` | `long?` | Parent post ID |
| `Stats` | `ImageStats?` | Reaction statistics |
| `Meta` | `ImageMeta?` | Generation metadata |
| `Username` | `string?` | Creator's username |
| `BaseModel` | `string?` | Base model used |
| `ModelVersionIds` | `IReadOnlyList<long>?` | Model versions used |

## Generation Metadata

Images often include generation metadata in the `Meta` property:

[!code-csharp[Program.cs](examples/Images/Program.cs#GenerationMetadata)]

The `ImageMeta` record includes:

| Property | Type | Description |
|----------|------|-------------|
| `Prompt` | `string?` | The positive prompt |
| `NegativePrompt` | `string?` | The negative prompt |
| `Steps` | `int?` | Number of sampling steps |
| `Sampler` | `string?` | Sampler name |
| `CfgScale` | `decimal?` | Classifier-Free Guidance scale |
| `Seed` | `long?` | Generation seed |
| `Model` | `string?` | Model name used |
| `Size` | `string?` | Image dimensions as string |
| `ClipSkip` | `int?` | CLIP skip value |

## NSFW Filtering

Filter images by NSFW level:

[!code-csharp[Program.cs](examples/Images/Program.cs#NsfwFiltering)]

Available NSFW levels:
- `None` - Safe for work
- `Soft` - Mildly suggestive
- `Mature` - Mature content
- `Explicit` - Explicit content (maps to API value "X")

> **Note**: Accessing `ImageNsfwLevel.Mature` and `ImageNsfwLevel.X` requires authentication with an API key. Without authentication, these levels will return no results.

## Sorting Images

[!code-csharp[Program.cs](examples/Images/Program.cs#SortingImages)]

Available sort options:
- `MostReactions` - Sort by reaction count (descending)
- `MostComments` - Sort by comment count (descending)
- `MostCollected` - Sort by collection count (descending)
- `Newest` - Sort by creation date, newest first
- `Oldest` - Sort by creation date, oldest first
- `Random` - Random order

## Image Statistics

Access reaction counts:

[!code-csharp[Program.cs](examples/Images/Program.cs#ImageStatistics)]

## Pagination

Images use cursor-based pagination:

[!code-csharp[Program.cs](examples/Images/Program.cs#pagination)]

## Downloading Images

To download an image, use the `Url` property:

[!code-csharp[Program.cs](examples/Images/Program.cs#DownloadingImages)]

> [!TIP]
> For production use, consider using `CivitaiSharp.Tools` which provides robust download utilities with progress tracking and retry logic.

## Next Steps

- [Error Handling](error-handling.md) - Handle API errors gracefully
- [Pagination](pagination.md) - Navigate large result sets
