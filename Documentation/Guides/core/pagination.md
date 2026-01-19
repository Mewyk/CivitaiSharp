---
title: Pagination
description: Navigate large result sets with CivitaiSharp's cursor-based and page-based pagination support for efficient API traversal.
---

# Pagination

CivitaiSharp supports two types of pagination: **cursor-based** (Images) and **page-index** (Models, Tags, Creators). Cursor-based uses `NextCursor` strings for efficient traversal. Page-index uses `WithPageIndex(n)` for direct page access.

Page size limits: Models (1-100), Images (1-200), Tags (1-200), Creators (1-200).

## Understanding Pagination

When you execute a query, the result includes pagination metadata:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#PaginationMetadata)]

The `PaginationMetadata` record contains total items, current page, page size, total pages, and next/previous cursors. See the API reference for complete details.

## Basic Pagination

### Setting Page Size

Use the `resultsLimit` parameter in `ExecuteAsync` to control how many items are returned per page:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#PageSize)]

### Iterating Through Pages

Use cursor-based pagination to iterate through all pages:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#CursorPagination)]

## Page-Based Pagination

The Models, Tags, and Creators endpoints support page index-based pagination:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#PageIndex)]


## Async Enumeration Pattern (Optional)

If you prefer a more convenient iteration pattern, you can implement a custom extension method in your application:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#AsyncEnumerationExtension)]

> [!NOTE]
> This extension is not part of the CivitaiSharp library. It's an example pattern you can implement in your own codebase for convenient async enumeration.

## Getting the First Result

When you only need the first result, use `FirstOrDefaultAsync`:

[!code-csharp[Program.cs](examples/Pagination/Program.cs#GetFirstResult)]

## Next Steps

- [Request Builders](request-builders.md) - Learn more about query building
- [Error Handling](error-handling.md) - Handle pagination errors gracefully
