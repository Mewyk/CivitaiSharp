---
title: Quick Start
description: Get started with CivitaiSharp.Core in minutes. Learn how to set up dependency injection and query models from the Civitai API.
---

# Quick Start

This guide shows you how to get started with CivitaiSharp.Core to query models from the Civitai API.

## Prerequisites

- .NET 10 or higher
- CivitaiSharp.Core package installed

## Setting Up

CivitaiSharp uses dependency injection. Register the API client in your `IServiceCollection`:

[!code-csharp[Program.cs](QuickStart/Program.cs#setup)]

## Querying Models

Use the fluent builder pattern to construct queries:

[!code-csharp[Program.cs](QuickStart/Program.cs#query)]

## Handling Results

All API operations return a `Result<T>` type that can be either a success or failure:

[!code-csharp[Program.cs](QuickStart/Program.cs#handling)]

## Complete Example

[!code-csharp[Program.cs](QuickStart/Program.cs)]

## Next Steps

- [Configuration](configuration.md) - Learn about all configuration options
- [Request Builders](request-builders.md) - Master the fluent API pattern
- [Error Handling](error-handling.md) - Handle errors gracefully
