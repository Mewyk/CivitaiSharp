---
title: Working with Images
description: Query AI-generated images from the Civitai gallery by model, username, or NSFW level using the ImageBuilder API.
---

# Working with Images

The `ImageBuilder` allows you to query generated images from the Civitai gallery. These are images created using AI models and shared by the community.

## Querying Images

### By Model

Find images generated with a specific model:

[!code-csharp[Program.cs](Images/Program.cs#by-model)]

### By Model Version

Find images generated with a specific model version:

[!code-csharp[Program.cs](Images/Program.cs#by-version)]

### By Creator

Find images posted by a specific user:

[!code-csharp[Program.cs](Images/Program.cs#by-creator)]

### By Post

Find all images in a specific post:

```csharp
var result = await apiClient.Images
    .WherePostId(12345)
    .ExecuteAsync();
```

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

```csharp
if (image.Meta is { } meta)
{
    Console.WriteLine($"Prompt: {meta.Prompt}");
    Console.WriteLine($"Negative Prompt: {meta.NegativePrompt}");
    Console.WriteLine($"Steps: {meta.Steps}");
    Console.WriteLine($"Sampler: {meta.Sampler}");
    Console.WriteLine($"CFG Scale: {meta.CfgScale}");
    Console.WriteLine($"Seed: {meta.Seed}");
    Console.WriteLine($"Model: {meta.Model}");
}
```

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

```csharp
// Only safe-for-work images
var safeImages = await apiClient.Images
    .WhereNsfwLevel(ImageNsfwLevel.None)
    .ExecuteAsync();

// Allow soft NSFW
var softImages = await apiClient.Images
    .WhereNsfwLevel(ImageNsfwLevel.Soft)
    .ExecuteAsync();
```

Available NSFW levels:
- `None` - Safe for work
- `Soft` - Mildly suggestive

> **Note**: Accessing `ImageNsfwLevel.Mature` and `ImageNsfwLevel.X` requires authentication with an API key. Without authentication, these levels will return no results.
- `Mature` - Mature content
- `Explicit` - Explicit content (maps to API value "X")

## Sorting Images

```csharp
var mostReacted = await apiClient.Images
    .OrderBy(ImageSort.MostReactions)
    .WherePeriod(TimePeriod.Week)
    .ExecuteAsync(resultsLimit: 20);
```

Available sort options:
- `MostReactions` - Sort by reaction count (descending)
- `MostComments` - Sort by comment count (descending)
- `MostCollected` - Sort by collection count (descending)
- `Newest` - Sort by creation date, newest first
- `Oldest` - Sort by creation date, oldest first
- `Random` - Random order

## Image Statistics

Access reaction counts:

```csharp
if (image.Stats is { } stats)
{
    Console.WriteLine($"Likes: {stats.LikeCount}");
    Console.WriteLine($"Hearts: {stats.HeartCount}");
    Console.WriteLine($"Laughs: {stats.LaughCount}");
    Console.WriteLine($"Cries: {stats.CryCount}");
    Console.WriteLine($"Comments: {stats.CommentCount}");
}
```

## Pagination

Images use cursor-based pagination:

[!code-csharp[Program.cs](Images/Program.cs#L62-L85)]

## Downloading Images

To download an image, use the `Url` property:

```csharp
using var httpClient = new HttpClient();
var imageBytes = await httpClient.GetByteArrayAsync(image.Url);
await File.WriteAllBytesAsync($"{image.Id}.png", imageBytes);
```

> [!TIP]
> For production use, consider using `CivitaiSharp.Tools` which provides robust download utilities with progress tracking and retry logic.

## Next Steps

- [Error Handling](error-handling.md) - Handle API errors gracefully
- [Pagination](pagination.md) - Navigate large result sets
