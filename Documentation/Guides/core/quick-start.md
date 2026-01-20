---
title: Quick Start
description: Get started with CivitaiSharp.Core.
---

# Quick Start

## Setup

Register the API client:

[!code-csharp[Program.cs](examples/QuickStart/Program.cs#setup)]

## Query Models

Use the fluent builder pattern:

[!code-csharp[Program.cs](examples/QuickStart/Program.cs#query)]

## Handle Results

All operations return `Result<T>`:

[!code-csharp[Program.cs](examples/QuickStart/Program.cs#result)]

## Next Steps

- [Request Builders](request-builders.md)
- [Error Handling](../common/error-handling.md)
