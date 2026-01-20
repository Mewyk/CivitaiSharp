---
title: Pagination
description: Navigate large result sets with cursor and page-based pagination.
---

# Pagination

**Cursor-based** (Images): Uses `NextCursor` strings

**Page-index** (Models, Tags, Creators): Uses `WithPageIndex(n)`

**Page size limits:** Models (1-100), Images (1-200), Tags (1-200), Creators (1-200)

## Pagination Metadata

[!code-csharp[Program.cs](../core/examples/Pagination/Program.cs#PaginationMetadata)]

## Set Page Size

[!code-csharp[Program.cs](../core/examples/Pagination/Program.cs#PageSize)]

## Cursor Pagination

[!code-csharp[Program.cs](../core/examples/Pagination/Program.cs#CursorPagination)]

## Page Index

[!code-csharp[Program.cs](../core/examples/Pagination/Program.cs#PageIndex)]

## Async Enumeration (Optional Pattern)

[!code-csharp[Program.cs](../core/examples/Pagination/Program.cs#AsyncEnumerationExtension)]

Not part of CivitaiSharp - implement in your codebase if needed.

## First Result

[!code-csharp[Program.cs](../core/examples/Pagination/Program.cs#GetFirstResult)]

## Next Steps

- [Request Builders](../core/request-builders.md)
- [Error Handling](error-handling.md)
