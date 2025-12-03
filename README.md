<p align="center">
  <img src="Resources/Logo/CivitaiSharp.png" alt="CivitaiSharp Logo - Temporary Ai generated image" width="120"/>
</p>

<h1 align="center" style="border-bottom: none;">CivitaiSharp</h1>

<p align="center">
  A modern, lightweight, and AOT-ready .NET 10 client library for all things Civitai.com
</p>

<p align="center">
  <!-- Status -->
  <img src="https://img.shields.io/badge/Status-Alpha-4682B4?style=flat&logo=beaker" alt="Status: Alpha"/>
  <!-- License -->
  <a href="https://opensource.org/licenses/MIT">
    <img src="https://img.shields.io/badge/License-MIT-4682B4?style=flat&logo=book" alt="License MIT"/>
  </a>
  <!-- .NET -->
  <a href="https://dotnet.microsoft.com/">
    <img src="https://img.shields.io/badge/.NET-10.0-4682B4?style=flat&logo=dotnet" alt=".NET 10"/>
  </a>
  <!-- GitHub -->
  <a href="https://github.com/Mewyk/CivitaiSharp">
    <img src="https://img.shields.io/badge/GitHub-Mewyk%2FCivitaiSharp-4682B4?style=flat&logo=github" alt="GitHub"/>
  </a>
  <!-- NuGet (dynamic) -->
  <a href="https://www.nuget.org/packages/CivitaiSharp.Core">
    <img src="https://img.shields.io/nuget/v/CivitaiSharp.Core?label=NuGet%20Core&logo=nuget" alt="NuGet Core"/>
  </a>
</p>

<p align="center">
  <strong>English</strong> |
  <a href="README.es-AR.md">Español (Argentina)</a> |
  <a href="README.ja.md">日本語</a>
</p>

<p align="center">
<strong>
  CivitaiSharp is currently in Alpha: APIs, features, and stability are subject to change.
</strong>
</p>

## Table of Contents
1. [Packages and Release Schedule](#1-packages-and-release-schedule)
2. [Installation](#2-installation)
3. [Quick Start](#3-quick-start)
4. [Configuration](#4-configuration)
5. [API Examples](#5-api-examples)
6. [Features](#6-features)
7. [Documentation](#7-documentation)
8. [Known API Quirks](#8-known-api-quirks)
9. [Versioning](#9-versioning)
10. [License](#10-license)
11. [Contributing](#11-contributing)


## 1. Packages and Release Schedule
| Package | Status | Description |
|---------|--------|-------------|
| **CivitaiSharp.Core** | Alpha | Public API client for models, images, tags, and creators |
| **CivitaiSharp.Sdk** | Alpha | Generator/Orchestration API client for image generation jobs |
| **CivitaiSharp.Tools** | Planned | Utilities for downloads, hashing, and HTML parsing |

> **Note:** Both Core and Sdk packages are currently in Alpha. APIs may change between minor versions.

> **Warning:** CivitaiSharp.Sdk is not fully tested and should not be used in production environments. Use at your own risk.


## 2. Installation
Install via NuGet:

```shell
dotnet add package CivitaiSharp.Core --prerelease
```


## 3. Quick Start
### Minimal Example
The simplest way to get started with CivitaiSharp.Core:

```csharp
using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCivitaiApi();

await using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IApiClient>();

var result = await client.Models.ExecuteAsync();
if (result.IsSuccess)
{
    foreach (var model in result.Value.Items)
        Console.WriteLine(model.Name);
}
```

### Full Example with Configuration

```csharp
using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCivitaiApi(options =>
{
    options.TimeoutSeconds = 120;
    options.StrictJsonParsing = true;
});

await using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IApiClient>();

var result = await client.Models
    .WhereType(ModelType.Checkpoint)
    .WhereNsfw(false)
    .OrderBy(ModelSort.MostDownloaded)
    .WithResultsLimit(10)
    .ExecuteAsync();

if (result.IsSuccess)
{
    foreach (var model in result.Value.Items)
        Console.WriteLine($"{model.Name} by {model.Creator?.Username}");
}
else
{
    Console.WriteLine($"Error: {result.ErrorInfo.Message}");
}
```


## 4. Configuration
### Using appsettings.json
CivitaiSharp.Core reads configuration from the `CivitaiApi` section by default.

<details>
<summary><strong>Minimal Configuration (appsettings.json)</strong></summary>

```json
{
  "CivitaiApi": {
  }
}
```

All settings have sensible defaults, so an empty section is valid.

</details>

<details>
<summary><strong>Full Configuration (appsettings.json)</strong></summary>

```json
{
  "CivitaiApi": {
    "BaseUrl": "https://civitai.com",
    "ApiVersion": "v1",
    "ApiKey": null,
    "TimeoutSeconds": 30,
    "StrictJsonParsing": false
  }
}
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BaseUrl` | `string` | `https://civitai.com` | Base URL for the Civitai API |
| `ApiVersion` | `string` | `v1` | API version path segment |
| `ApiKey` | `string?` | `null` | Optional API key for authenticated requests |
| `TimeoutSeconds` | `int` | `30` | HTTP request timeout (1-300 seconds) |
| `StrictJsonParsing` | `bool` | `false` | Throw on unmapped JSON properties |

> **Authentication Note:** The Core library can query public endpoints (models, images, tags, creators) without an API key. An API key is only required for authenticated features like favorites, hidden models, NSFW content, and higher rate limits. This is different from CivitaiSharp.Sdk which **always requires an API token** for all operations.

</details>

<details>
<summary><strong>Configuration with IConfiguration</strong></summary>

```csharp
using CivitaiSharp.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();
services.AddCivitaiApi(configuration);
// Or with a custom section name:
// services.AddCivitaiApi(configuration, "MyCustomSection");

await using var provider = services.BuildServiceProvider();
```

</details>


## 5. API Examples
CivitaiSharp.Core provides fluent builders for each endpoint. Each builder is immutable and thread-safe.

<details>
<summary><strong>Models Endpoint</strong></summary>

```csharp
// Get all models (default query)
var result = await client.Models.ExecuteAsync();

// Get a specific model by ID
var result = await client.Models.GetByIdAsync(12345);
if (result.IsSuccess)
    Console.WriteLine($"Model: {result.Value.Name}");

// Get the first matching model (efficient single-item retrieval)
var result = await client.Models
    .WhereName("SDXL")
    .FirstOrDefaultAsync();
if (result is { IsSuccess: true, Value: not null })
    Console.WriteLine($"Found: {result.Value.Name}");

// Search by name
var result = await client.Models
    .WhereName("SDXL")
    .ExecuteAsync();

// Filter by type and sort
var result = await client.Models
    .WhereType(ModelType.Checkpoint)
    .OrderBy(ModelSort.MostDownloaded)
    .WithResultsLimit(25)
    .ExecuteAsync();

// Filter by tag
var result = await client.Models
    .WhereTag("anime")
    .WhereNsfw(false)
    .ExecuteAsync();

// Filter by creator
var result = await client.Models
    .WhereUsername("Mewyk")
    .OrderBy(ModelSort.Newest)
    .ExecuteAsync();

// Filter by base model (string value, e.g., "SDXL 1.0", "SD 1.5", "Flux.1 D")
var result = await client.Models
    .WhereBaseModel("SDXL 1.0")
    .WhereType(ModelType.Lora)
    .ExecuteAsync();

// Filter by multiple base models
var result = await client.Models
    .WhereBaseModels("SDXL 1.0", "Pony")
    .ExecuteAsync();

// Filter by specific model IDs (ignored if query is also provided)
var result = await client.Models
    .WhereIds(12345, 67890, 11111)
    .ExecuteAsync();

// Get a specific model version by version ID
var versionResult = await client.Models.GetByVersionIdAsync(130072);
if (versionResult.IsSuccess)
{
    Console.WriteLine($"Version: {versionResult.Value.Name}");
    Console.WriteLine($"AIR: {versionResult.Value.AirIdentifier}");
}

// Get a model version by file hash (SHA256, AutoV2, CRC32, etc.)
var hashResult = await client.Models.GetByVersionHashAsync("ABC123DEF456");
if (hashResult.IsSuccess)
{
    Console.WriteLine($"Found: {hashResult.Value.Model?.Name}");
    Console.WriteLine($"AIR: {hashResult.Value.AirIdentifier}");
}
```

</details>

<details>
<summary><strong>Images Endpoint</strong></summary>

```csharp
// Get all images (default query)
var result = await client.Images.ExecuteAsync();

// Get the first matching image
var result = await client.Images
    .WhereModelId(12345)
    .FirstOrDefaultAsync();
if (result is { IsSuccess: true, Value: not null })
    Console.WriteLine($"Image URL: {result.Value.Url}");

// Filter by model ID
var result = await client.Images
    .WhereModelId(12345)
    .ExecuteAsync();

// Filter by model version ID
var result = await client.Images
    .WhereModelVersionId(67890)
    .OrderBy(ImageSort.Newest)
    .ExecuteAsync();

// Filter by username
var result = await client.Images
    .WhereUsername("Mewyk")
    .WhereNsfwLevel(ImageNsfwLevel.None)
    .WithResultsLimit(50)
    .ExecuteAsync();

// Filter by post ID
var result = await client.Images
    .WherePostId(11111)
    .ExecuteAsync();
```

</details>

<details>
<summary><strong>Tags Endpoint</strong></summary>

```csharp
// Get all tags (default query)
var result = await client.Tags.ExecuteAsync();

// Search tags by name
var result = await client.Tags
    .WhereName("portrait")
    .WithResultsLimit(100)
    .ExecuteAsync();
```

</details>

<details>
<summary><strong>Creators Endpoint</strong></summary>

```csharp
// Get all creators (default query)
var result = await client.Creators.ExecuteAsync();

// Search creators by name
var result = await client.Creators
    .WhereName("Mewyk")
    .WithResultsLimit(20)
    .ExecuteAsync();

// Page-based pagination (creators use pages, not cursors)
var result = await client.Creators
    .WithPageIndex(2)
    .WithResultsLimit(50)
    .ExecuteAsync();
```

</details>

<details>
<summary><strong>Pagination</strong></summary>

```csharp
// Cursor-based pagination (Models, Images, Tags)
string? cursor = null;
var allModels = new List<Model>();

do
{
    var result = await client.Models
        .WhereType(ModelType.Checkpoint)
        .WithResultsLimit(100)
        .ExecuteAsync(cursor: cursor);

    if (!result.IsSuccess)
        break;

    allModels.AddRange(result.Value.Items);
    cursor = result.Value.Metadata?.NextCursor;
    
} while (cursor is not null);

// Page-based pagination (Creators only)
var page1 = await client.Creators.WithPageIndex(1).ExecuteAsync();
var page2 = await client.Creators.WithPageIndex(2).ExecuteAsync();
```

</details>

<details>
<summary><strong>Error Handling</strong></summary>

```csharp
var result = await client.Models.ExecuteAsync();

// Pattern matching
var message = result switch
{
    { IsSuccess: true, Value: var page } => $"Found {page.Items.Count} models",
    { IsSuccess: false, ErrorInfo: var error } => error.Code switch
    {
        ErrorCode.RateLimited => "Too many requests, please slow down",
        ErrorCode.Unauthorized => "Invalid or missing API key",
        ErrorCode.NotFound => "Resource not found",
        _ => $"Error: {error.Code} - {error.Message}"
    }
};

// Traditional approach
if (result.IsSuccess)
{
    foreach (var model in result.Value.Items)
        Console.WriteLine(model.Name);
}
else
{
    Console.WriteLine($"Failed: {result.ErrorInfo.Message}");
}
```

</details>


## 6. Features
- **Modern .NET 10** - Built with nullable reference types, records, and primary constructors
- **Fluent Query Builders** - Type-safe, immutable builders for constructing API requests
- **Result Pattern** - Explicit success/failure handling with discriminated union
- **Built-in Resilience** - Retry policies, circuit breakers, rate limiting, and timeouts
- **Dependency Injection** - First-class support for `IHttpClientFactory` and Microsoft DI
- **Streaming Downloads** - Memory-efficient response handling with `ResponseHeadersRead`
- **Explicit JSON Contract** - All model properties use `[JsonPropertyName]` for type safety


## 7. Documentation
- [API Reference](https://CivitaiSharp.Mewyk.com/Docs/api/)
- [Getting Started Guide](https://CivitaiSharp.Mewyk.com/Guides/introduction.html)


## 8. Known API Quirks
CivitaiSharp interacts with the Civitai.com API, which has several known quirks. Some are mitigated automatically; others are documented and under investigation.

| Issue | Description |
|-------|-------------|
| Tags endpoint model count | Documented feature, but responses never include this field |
| `limit=0` parameter | Documented to return all results for various endpoints, but returns an error |
| Semantic errors with HTTP 200 | Errors, Not Found, and others, all return HTTP 200 |
| Endpoint inconsistencies | Intermittent throttling, unreported outages, undocumented rate limits |
| Variable metadata structures | Metadata format varies widely between responses |
| User existence issues | During partial outages, existing users may appear nonexistent |
| **Creator endpoint unreliability** | See details below |

### Creator Endpoint Reliability Issues

The `/api/v1/creators` endpoint is known to experience intermittent reliability issues:

- **HTTP 500 errors**: The endpoint frequently returns server errors, especially under load
- **Slow response times**: Requests may take significantly longer than other endpoints (10-30+ seconds)
- **Timeout failures**: Long response times can exceed client timeout thresholds

**Recommendations:**

1. **Implement generous timeouts**: Set timeouts of 60-120 seconds for Creator endpoint requests
2. **Use retry logic**: The built-in resilience handler will retry on 500 errors, but success is not guaranteed
3. **Handle failures gracefully**: Your application should degrade gracefully when Creator data is unavailable
4. **Cache results aggressively**: When requests succeed, cache the results to reduce API load

```csharp
// Example: Handling Creator endpoint unreliability
var result = await client.Creators.WithResultsLimit(10).ExecuteAsync();

if (!result.IsSuccess)
{
    // Log the error but continue with degraded functionality
    logger.LogWarning("Creator data unavailable: {Error}", result.ErrorInfo.Message);
    return GetCachedCreators(); // Fallback to cached data
}
```

Additional quirks are being tracked and will be addressed in future releases.


## 9. Versioning
CivitaiSharp follows **MAJOR.MINOR.PATCH** semantic versioning:

| Component | Description |
|-----------|-------------|
| **MAJOR** | Significant, breaking API changes |
| **MINOR** | New features; may include limited breaking changes unlikely to affect most users |
| **PATCH** | Backwards-compatible bug fixes and improvements |

Pre-release versions use the format: `MAJOR.MINOR.PATCH-alpha.N`

> **Note**: While in Alpha (0.x.x), APIs may change between minor versions. Stability is guaranteed from v1.0.0 onwards.


## 10. License
This repository is released under the [MIT License](LICENSE).


## 11. Contributing
Contributions are welcome. Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.


## Legal Notice

CivitaiSharp is an independent open-source project and is not affiliated with, sponsored by, endorsed by, or officially associated with Civitai.com or Civitai, Inc. The Civitai name and any related trademarks are the property of their respective owners. Use of these names is for identification purposes only and does not imply any endorsement or partnership.

---

<p align="center">
  <a href="https://github.com/Mewyk/CivitaiSharp">GitHub</a> |
  <a href="https://CivitaiSharp.Mewyk.com">Documentation</a> |
  <a href="https://civitai.com">Civitai</a>
</p>
