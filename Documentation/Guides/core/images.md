---
title: Working with Images
description: Query AI-generated images from Civitai.
---

# Working with Images

Query generated images from the Civitai gallery.

## Query Examples

**By Model:**
[!code-csharp[Program.cs](examples/Images/Program.cs#ByModel)]

**By Model Version:**
[!code-csharp[Program.cs](examples/Images/Program.cs#ByVersion)]

**By Creator:**
[!code-csharp[Program.cs](examples/Images/Program.cs#ByCreator)]

**By Post:**
[!code-csharp[Program.cs](examples/Images/Program.cs#ByPost)]

## Generation Metadata

[!code-csharp[Program.cs](examples/Images/Program.cs#GenerationMetadata)]

## NSFW Filtering

[!code-csharp[Program.cs](examples/Images/Program.cs#NsfwFiltering)]

**Levels:** `None`, `Soft`, `Mature`, `Explicit`

Accessing `Mature` and `X` requires authentication.

## Sorting

[!code-csharp[Program.cs](examples/Images/Program.cs#SortingImages)]

**Options:** `MostReactions`, `MostComments`, `MostCollected`, `Newest`, `Oldest`, `Random`

## Statistics

[!code-csharp[Program.cs](examples/Images/Program.cs#ImageStatistics)]

## Downloading

[!code-csharp[Program.cs](examples/Images/Program.cs#DownloadingImages)]

For production, use `CivitaiSharp.Tools` for robust downloads with progress tracking.

## Next Steps

- [Models](models.md)
- [Error Handling](../common/error-handling.md)
- [Pagination](../common/pagination.md)
