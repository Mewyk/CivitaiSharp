---
title: Working with Creators
description: Search for Civitai creators.
---

# Working with Creators

Search for users who publish models on Civitai.

## Query Examples

**Search by Username:**
[!code-csharp[Program.cs](examples/Creators/Program.cs#SearchByUsername)]

**List All:**
[!code-csharp[Program.cs](examples/Creators/Program.cs#ListAllCreators)]

## Creator Information

[!code-csharp[Program.cs](examples/Creators/Program.cs#AccessingCreatorInformation)]

## Model Creators

[!code-csharp[Program.cs](examples/Creators/Program.cs#WorkingWithModelCreators)]

## Common Patterns

**List Creators:**
[!code-csharp[Program.cs](examples/Creators/Program.cs#FindPopularCreators)]

**Search Specific:**
[!code-csharp[Program.cs](examples/Creators/Program.cs#SearchForSpecificCreator)]

**Models by Creator:**
[!code-csharp[Program.cs](examples/Creators/Program.cs#GetAllModelsByCreator)]

## Endpoint Reliability Warning

The `/api/v1/creators` endpoint experiences reliability issues: HTTP 500 errors, slow responses (10-30s), and timeouts. Use generous timeouts (60-120s), implement retry logic, handle failures gracefully, and cache results.

[!code-csharp[Program.cs](examples/Creators/Program.cs#HandleCreatorEndpointUnreliability)]

## Next Steps

- [Models](models.md)
- [Tags](tags.md)
- [Pagination](../common/pagination.md)
