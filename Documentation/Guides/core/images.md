---
title: Working with Images
description: Query AI-generated images from the Civitai gallery by model, username, or NSFW level using the ImageBuilder API.
---

# Working with Images

The `ImageBuilder` allows you to query generated images from the Civitai gallery. These are images created using AI models and shared by the community.

## Querying Images

### By Model

Find images generated with a specific model:

[!code-csharp[Program.cs](examples/Images/Program.cs#ByModel)]

### By Model Version

Find images generated with a specific model version:

[!code-csharp[Program.cs](examples/Images/Program.cs#ByVersion)]

### By Creator

Find images posted by a specific user:

[!code-csharp[Program.cs](examples/Images/Program.cs#ByCreator)]

### By Post

Find all images in a specific post:

[!code-csharp[Program.cs](examples/Images/Program.cs#ByPost)]

## The Image Record

The `Image` record contains image URL, dimensions, NSFW level, creation timestamp, creator username, and generation metadata. See the API reference for complete property details.

## Generation Metadata

Images often include generation metadata in the `Meta` property:

[!code-csharp[Program.cs](examples/Images/Program.cs#GenerationMetadata)]

The `ImageMeta` record includes prompt, steps, sampler, CFG scale, seed, and model information. See the API reference for complete details.

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

Available sort options: `MostReactions`, `MostComments`, `MostCollected`, `Newest`, `Oldest`, `Random`

## Image Statistics

Access reaction counts:

[!code-csharp[Program.cs](examples/Images/Program.cs#ImageStatistics)]

## Downloading Images

To download an image, use the `Url` property:

[!code-csharp[Program.cs](examples/Images/Program.cs#DownloadingImages)]

> [!TIP]
> For production use, consider using `CivitaiSharp.Tools` which provides robust download utilities with progress tracking and retry logic.

## Next Steps

- [Error Handling](error-handling.md) - Handle API errors gracefully
- [Pagination](pagination.md) - Navigate large result sets
