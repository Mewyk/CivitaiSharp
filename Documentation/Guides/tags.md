---
title: Working with Tags
description: Query and search tags used to categorize models on Civitai using the TagBuilder API.
---

# Working with Tags

Tags are keywords used to categorize models on Civitai. The `TagBuilder` allows you to query available tags.

## Querying Tags

### List All Tags

[!code-csharp[Program.cs](Tags/Program.cs#List-All-Tags)]

### Search by Name

Filter tags by partial name match:

[!code-csharp[Program.cs](Tags/Program.cs#Search-by-Name)]

### Pagination

Tags support cursor-based pagination with a default limit of 20 and maximum of 100:

[!code-csharp[Program.cs](Tags/Program.cs#Pagination)]

## The Tag Record

The `Tag` record contains basic information about a tag:

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | The tag name |
| `Link` | `string` | URL to retrieve models with this tag |

### Using Tags with Models

Tags are referenced in model queries to filter results:

```csharp
var animeModels = await apiClient.Models
    .WhereTag("anime")
    .ExecuteAsync(resultsLimit: 50);

if (animeModels is Result<PagedResult<Model>>.Success success)
{
    foreach (var model in success.Data.Items)
    {
        Console.WriteLine($"{model.Name}");
        
        if (model.Tags is { } tags)
        {
            Console.WriteLine($"  Tags: {string.Join(", ", tags)}");
        }
    }
}
```

### Model Tag Arrays

Models include a `Tags` property containing all associated tags:

```csharp
if (model.Tags is { } tags)
{
    foreach (var tag in tags)
    {
        Console.WriteLine($"  - {tag}");
    }
}
```

## Common Use Cases

### Find Popular Tags

```csharp
var popularTags = await apiClient.Tags
    .ExecuteAsync(resultsLimit: 100);

if (popularTags is Result<PagedResult<Tag>>.Success success)
{
    Console.WriteLine("Top 100 Tags:");
    foreach (var tag in success.Data.Items)
    {
        Console.WriteLine($"  {tag.Name}");
    }
}
```

### Search Related Tags

```csharp
var characterTags = await apiClient.Tags
    .WhereName("character")
    .ExecuteAsync(resultsLimit: 50);
```

## Best Practices

**Tag Matching**: The `WhereName` filter performs partial matching, so searching for "anime" will match "anime", "anime-style", "anime-character", etc.

**Link Property**: The `Link` property provides a convenience URL to retrieve models. However, using `Models.WhereTag(tagName)` is the recommended approach in code.

**Case Sensitivity**: Tag matching is case-insensitive on the API side.

## Next Steps

- [Working with Models](models.md) - Filter models by tags
- [Working with Creators](creators.md) - Find content creators
- [Pagination](pagination.md) - Navigate large tag lists
