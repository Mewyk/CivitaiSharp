---
title: Working with Tags
description: Query and search tags used to categorize models on Civitai using the TagBuilder API.
---

# Working with Tags

Tags are keywords used to categorize models on Civitai. The `TagBuilder` allows you to query available tags.

## Querying Tags

### List All Tags

[!code-csharp[Program.cs](examples/Tags/Program.cs#ListAllTags)]

### Search by Name

Filter tags by partial name match:

[!code-csharp[Program.cs](examples/Tags/Program.cs#SearchByName)]

### Pagination

Tags support pagination. See [Pagination](pagination.md) for details on page-based vs cursor-based pagination.

[!code-csharp[Program.cs](examples/Tags/Program.cs#Pagination)]

## The Tag Record

The `Tag` record contains the tag name and a link to retrieve models with that tag.

### Using Tags with Models

Tags are referenced in model queries to filter results:

[!code-csharp[Program.cs](examples/Tags/Program.cs#UsingTagsWithModels)]

### Model Tag Arrays

Models include a `Tags` property containing all associated tags:

[!code-csharp[Program.cs](examples/Tags/Program.cs#ModelTagArrays)]

## Common Use Cases

### List Multiple Tags

[!code-csharp[Program.cs](examples/Tags/Program.cs#FindPopularTags)]

> **Note**: Tags are returned in the order provided by the API, not sorted by popularity or usage frequency.

### Search Related Tags

[!code-csharp[Program.cs](examples/Tags/Program.cs#SearchRelatedTags)]

## Next Steps

- [Working with Models](models.md) - Filter models by tags
- [Working with Creators](creators.md) - Find content creators
- [Pagination](pagination.md) - Navigate large tag lists
