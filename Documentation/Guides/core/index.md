# Core Library

The **CivitaiSharp.Core** library provides the fundamental API client for interacting with the Civitai API. It includes request builders, models, response handling, and all the core functionality needed to query models, images, creators, and tags.

## Overview

The Core library is designed around a fluent request builder pattern with type-safe results for handling both successful responses and errors.

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
