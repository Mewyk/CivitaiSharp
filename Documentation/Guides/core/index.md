# Core Library

The **CivitaiSharp.Core** library provides the fundamental API client for interacting with the Civitai API. It includes request builders, models, response handling, and all the core functionality needed to query models, images, creators, and tags.

## Overview

The Core library is designed around a fluent request builder pattern that makes it easy to construct and execute API queries. All operations return a `Result<T>` type that handles both successful responses and errors in a type-safe manner.

## Key Features

- **Fluent Request Builders**: Chainable methods for building complex queries
- **Type-Safe Results**: Pattern matching for handling success and failure cases
- **Model Queries**: Search and retrieve models by type, tag, creator, and more
- **Image Queries**: Find images by model, version, or creator
- **Creator & Tag Management**: List and search creators and tags
- **Pagination Support**: Both cursor-based and page-index pagination
- **Error Handling**: Comprehensive error types with detailed information

## Quick Links

- [Introduction](introduction.md) - Getting started with the Core library
- [Getting API Key](getting-api-key.md) - How to configure authentication
- [Quick Start](quick-start.md) - Your first query in 5 minutes
- [Configuration](configuration.md) - Detailed configuration options
- [Request Builders](request-builders.md) - Building fluent queries
- [Models](models.md) - Querying and retrieving models
- [Images](images.md) - Working with image queries
- [Creators](creators.md) - Finding creators and their content
- [Tags](tags.md) - Searching and listing tags
- [Error Handling](error-handling.md) - Best practices for error handling
- [Pagination](pagination.md) - Navigating large result sets
- [API Quirks](api-quirks.md) - Important API behavior notes

## Installation

```bash
dotnet add package CivitaiSharp.Core
```

## Basic Example

```csharp
using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi();
var host = builder.Build();
await host.StartAsync();

var client = host.Services.GetRequiredService<IApiClient>();

var result = await client.Models
    .WhereType(ModelType.Lora)
    .WhereTag("anime")
    .ExecuteAsync(resultsLimit: 10);

if (result is Result<PagedResult<Model>>.Success success)
{
    foreach (var model in success.Data.Items)
    {
        Console.WriteLine($"{model.Name} by {model.Creator?.Username}");
    }
}

await host.StopAsync();
```

## Next Steps

- Start with [Introduction](introduction.md) to understand the core concepts
- Review [Request Builders](request-builders.md) to learn the query system
- Check [Error Handling](error-handling.md) for production-ready code
