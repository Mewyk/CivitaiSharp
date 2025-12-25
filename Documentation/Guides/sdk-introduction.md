---
title: CivitaiSharp.Sdk Introduction
description: Learn about CivitaiSharp.Sdk, a high-level .NET client for the Civitai Generator API with image generation, job management, and usage tracking.
---

# CivitaiSharp.Sdk

CivitaiSharp.Sdk provides a high-level client for the [Civitai Generator API](https://developer.civitai.com/docs/api/generator) (orchestration endpoints). It simplifies image generation workflows with strongly-typed models and async job management.

## Key Features

- **Image Generation** - Submit text-to-image jobs with flexible parameters
- **Job Management** - Query, cancel, and retrieve job results
- **Coverage Service** - Check model and resource availability
- **Usage Tracking** - Monitor API consumption and credits
- **Typed Models** - Strongly-typed request and response models

## Getting Started

### Installation

```bash
dotnet add package CivitaiSharp.Sdk --prerelease
```

### Registration

> [!IMPORTANT]
> Unlike CivitaiSharp.Core which can access public endpoints anonymously, the SDK **always requires authentication**. All Generator API operations require a valid API token.

Register the SDK client using dependency injection:

```csharp
services.AddCivitaiSdk(options =>
{
    options.ApiToken = "your-api-token";  // Required - SDK cannot operate without a token
});
```

### Basic Usage

```csharp
public class ImageGenerationService(ISdkClient sdkClient)
{
    public async Task GenerateImageAsync()
    {
        var result = await sdkClient.Jobs
            .CreateTextToImage()
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("a beautiful sunset over mountains")
            .WithNegativePrompt("blurry, low quality")
            .WithSize(1024, 1024)
            .WithSteps(30)
            .WithCfgScale(7.5m)
            .ExecuteAsync();

        if (result is Result<JobStatusCollection>.Success success)
        {
            Console.WriteLine($"Job submitted: {success.Data.Token}");
        }
    }
}
```

## Services

### Jobs Operations

Create and manage image generation jobs using fluent builders:

**Creating Jobs:**
- `CreateTextToImage()` - Returns a `TextToImageBuilder` for configuring and submitting jobs

**Querying Jobs:**
- `Query` - Returns a cached `JobQueryBuilder` for fluent job queries
  - `WithDetailed()` - Include original job specifications in response
  - `WithWait()` - Block until jobs complete (up to ~10 minutes)
  - `WhereProperty(key, value)` - Filter by custom properties
  - `GetByIdAsync(Guid id)` - Get job status by ID
  - `GetByTokenAsync(string token)` - Get job status by token
  - `ExecuteAsync()` - Query jobs by custom properties

**Job Management:**
- `Query.CancelAsync(Guid id)` - Cancel a specific job by ID
- `Query.CancelAsync(string token)` - Cancel all jobs in a batch by token
- `Query.TaintAsync(Guid id)` - Mark a job as tainted by ID
- `Query.TaintAsync(string token)` - Mark all jobs in a batch as tainted by token

**Example - Querying Jobs:**
```csharp
// Query by ID with detailed information
var result = await sdkClient.Jobs.Query
    .WithDetailed()
    .GetByIdAsync(jobId);

// Query by token and wait for completion (blocks up to ~10 minutes)
var result = await sdkClient.Jobs.Query
    .WithWait()
    .WithDetailed()
    .GetByTokenAsync(token);

// Query by custom properties
var result = await sdkClient.Jobs.Query
    .WhereProperty("userId", JsonSerializer.SerializeToElement("12345"))
    .WhereProperty("environment", JsonSerializer.SerializeToElement("production"))
    .ExecuteAsync();
```

### Coverage Service

Check resource availability across providers:

- `GetAsync` - Get coverage information for models and resources

### Usage Service

Monitor API consumption:

- `GetConsumptionAsync` - Get account consumption statistics for a specified period

## Next Steps

- [Jobs Service](sdk-jobs.md) - Comprehensive guide to creating and querying jobs
- [Coverage Service](sdk-coverage.md) - Check model and resource availability
- [Usage Service](sdk-usage.md) - Monitor API consumption and credits
- [Configuration](configuration.md) - Configure SDK client options
- [Quick Start](quick-start.md) - Step-by-step guide to your first image generation
- [AIR Identifiers](air-identifier.md) - Learn about model resource identifiers
