---
title: Jobs Service
description: Learn how to submit, track, and manage image generation jobs using the CivitaiSharp.Sdk Jobs service with fluent builders.
---

# Jobs Service

The Jobs service provides comprehensive functionality for submitting, tracking, and managing image generation jobs through the Civitai Generator API.

## Overview

The Jobs service provides two fluent builders:

1. **TextToImageBuilder** - Fluent, immutable builder for creating and submitting jobs (accessed via `CreateTextToImage()`)
2. **JobQueryBuilder** - Fluent, immutable builder for querying and managing jobs (accessed via `Query` property)

Both builders follow CivitaiSharp's immutable, thread-safe design pattern.

## Creating Jobs

### Basic Text-to-Image Generation

Use the `CreateTextToImage()` method to get a fluent builder:

```csharp
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
    Console.WriteLine($"Job submitted with token: {success.Data.Token}");
    foreach (var job in success.Data.Jobs)
    {
        Console.WriteLine($"Job ID: {job.JobId}, Status: {job.Status}");
    }
}
```

### Advanced Configuration

Configure additional parameters for more control:

```csharp
var result = await sdkClient.Jobs
    .CreateTextToImage()
    .WithModel(model)
    .WithPrompt("detailed portrait")
    .WithSeed(12345)
    .WithSteps(50)
    .WithCfgScale(8.5m)
    .WithQuantity(4)  // Generate 4 images
    .WithClipSkip(2)
    .WithCallbackUrl("https://myapp.com/webhook")
    .WithRetries(3)
    .ExecuteAsync();
```

### Using Additional Networks (LoRAs)

Add LoRAs and other networks to enhance generation:

```csharp
var lora = AirIdentifier.Parse("urn:air:sdxl:lora:civitai:123456@789");

var result = await sdkClient.Jobs
    .CreateTextToImage()
    .WithModel(baseModel)
    .WithPrompt("character portrait")
    .WithAdditionalNetwork(lora, builder => builder
        .WithStrength(0.8m)
        .WithTriggerWord("character"))
    .WithAdditionalNetwork(anotherLora, builder => builder
        .WithStrength(0.5m))
    .ExecuteAsync();
```

### Using ControlNet

Guide generation with ControlNet:

```csharp
var result = await sdkClient.Jobs
    .CreateTextToImage()
    .WithModel(model)
    .WithPrompt("person standing")
    .WithControlNet(builder => builder
        .WithModel(controlNetModel)
        .WithImage("https://example.com/pose.png")
        .WithWeight(1.0m)
        .WithStartingControlStep(0.0m)
        .WithEndingControlStep(1.0m))
    .ExecuteAsync();
```

### Batch Job Submission

Submit multiple jobs at once:

```csharp
var job1 = sdkClient.Jobs
    .CreateTextToImage()
    .WithModel(model1)
    .WithPrompt("landscape");

var job2 = sdkClient.Jobs
    .CreateTextToImage()
    .WithModel(model2)
    .WithPrompt("portrait");

var result = await job1.ExecuteBatchAsync([job2]);

if (result is Result<JobStatusCollection>.Success success)
{
    Console.WriteLine($"Batch submitted with token: {success.Data.Token}");
}
```

## Querying Jobs

### Query by Job ID

Retrieve a specific job's status:

```csharp
var result = await sdkClient.Jobs.Query
    .WithDetailed()
    .GetByIdAsync(jobId);

if (result is Result<JobStatus>.Success success)
{
    Console.WriteLine($"Status: {success.Data.Status}");
    if (success.Data.Result?.BlobUrl is not null)
    {
        Console.WriteLine($"Image URL: {success.Data.Result.BlobUrl}");
    }
}
```

### Query by Token

Retrieve all jobs from a batch submission:

```csharp
var result = await sdkClient.Jobs.Query
    .GetByTokenAsync(token);

if (result is Result<JobStatusCollection>.Success success)
{
    foreach (var job in success.Data.Jobs)
    {
        Console.WriteLine($"{job.JobId}: {job.Status}");
    }
}
```

### Wait for Completion

Block until jobs complete (up to ~10 minutes):

```csharp
var result = await sdkClient.Jobs.Query
    .WithWait()
    .WithDetailed()
    .GetByTokenAsync(token);

// Jobs will be in completed/failed state when this returns
if (result is Result<JobStatusCollection>.Success success)
{
    var completed = success.Data.Jobs.Where(j => j.Status == "succeeded");
    Console.WriteLine($"Completed: {completed.Count()} jobs");
}
```

### Query by Custom Properties

Filter jobs using custom properties set during submission:

```csharp
// When submitting, add custom properties
var submitResult = await sdkClient.Jobs
    .CreateTextToImage()
    .WithModel(model)
    .WithPrompt("landscape")
    .WithProperty("userId", JsonSerializer.SerializeToElement("12345"))
    .WithProperty("environment", JsonSerializer.SerializeToElement("production"))
    .WithProperty("requestId", JsonSerializer.SerializeToElement(789))
    .ExecuteAsync();

// Later, query by those properties
var queryResult = await sdkClient.Jobs.Query
    .WhereProperty("userId", JsonSerializer.SerializeToElement("12345"))
    .WhereProperty("environment", JsonSerializer.SerializeToElement("production"))
    .ExecuteAsync();

if (queryResult is Result<JobStatusCollection>.Success success)
{
    Console.WriteLine($"Found {success.Data.Jobs.Count} matching jobs");
}
```

### Query Multiple Property Types

The WhereProperties method accepts a dictionary of JsonElement values for flexible filtering:

```csharp
var propertyFilters = new Dictionary<string, JsonElement>
{
    ["category"] = JsonSerializer.SerializeToElement("portrait"),
    ["priority"] = JsonSerializer.SerializeToElement(5),
    ["highQuality"] = JsonSerializer.SerializeToElement(true)
};

var result = await sdkClient.Jobs.Query
    .WithDetailed()
    .WhereProperties(propertyFilters)
    .ExecuteAsync();
```

### Query with Advanced JsonElement

For complex property values, use `JsonElement` directly:

```csharp
using var doc = JsonDocument.Parse(@"{""nested"": {""value"": 123}}");
var element = doc.RootElement.GetProperty("nested");

var result = await sdkClient.Jobs.Query
    .WhereProperty("complexData", element)
    .ExecuteAsync();
```

## Job Management

### Cancel Jobs

Cancel by ID:

```csharp
var result = await sdkClient.Jobs.Query.CancelAsync(jobId, force: true);

if (result is Result<Unit>.Success)
{
    Console.WriteLine("Job cancelled successfully");
}
```

Cancel by token:

```csharp
var result = await sdkClient.Jobs.Query.CancelAsync(token, force: true);
```

### Taint Jobs

Mark jobs as tainted (for quality control):

```csharp
await sdkClient.Jobs.Query.TaintAsync(jobId);
await sdkClient.Jobs.Query.TaintAsync(token);
```

## Jobs Operations API Reference

### JobQueryBuilder Methods

These methods are accessed through `sdkClient.Jobs.Query`:

| Method | Parameters | Description |
|--------|------------|-------------|
| `WithDetailed()` | - | Returns a new builder configured to include detailed job specifications |
| `WithWait()` | - | Returns a new builder configured to wait for completion (blocks up to ~10 min) |
| `WhereProperty` | `string key, JsonElement value` | Returns a new builder with an added property filter (uses AND logic) |
| `WhereProperties` | `IReadOnlyDictionary<string, JsonElement> properties` | Returns a new builder with multiple property filters added |
| `GetByIdAsync` | `Guid jobId, CancellationToken` | Get status of a specific job by its ID |
| `GetByTokenAsync` | `string token, CancellationToken` | Get status of jobs by batch token |
| `ExecuteAsync` | `CancellationToken` | Query jobs matching all configured property filters (at least one required) |
| `CancelAsync` | `Guid jobId, bool force, CancellationToken` | Cancel a specific job by ID |
| `CancelAsync` | `string token, bool force, CancellationToken` | Cancel all jobs in a batch by token |
| `TaintAsync` | `Guid jobId, CancellationToken` | Mark a specific job as tainted by ID |
| `TaintAsync` | `string token, CancellationToken` | Mark all jobs in a batch as tainted by token |

### TextToImageBuilder Methods

Accessed through `sdkClient.Jobs.CreateTextToImage()`:

#### Required Parameters

| Method | Description |
|--------|-------------|
| `WithModel(AirIdentifier)` | Set the base model (required) |
| `WithPrompt(string)` | Set the positive prompt (required) |

#### Image Parameters

| Method | Description |
|--------|-------------|
| `WithNegativePrompt(string)` | Set negative prompt |
| `WithSize(int, int)` | Set width and height (must be multiples of 8, range: 64-2048) |
| `WithSteps(int)` | Set sampling steps (range: 1-100, default: 20) |
| `WithCfgScale(decimal)` | Set CFG scale (range: 1-30, default: 7) |
| `WithSeed(long)` | Set seed for reproducibility |
| `WithClipSkip(int)` | Set CLIP skip layers (range: 1-12) |

#### Additional Networks

| Method | Description |
|--------|-------------|
| `WithAdditionalNetwork(AirIdentifier, ImageJobNetworkParams)` | Add LoRA or embedding with network configuration |
| `WithAdditionalNetwork(AirIdentifier, ImageJobNetworkParamsBuilder)` | Add LoRA or embedding using a builder |
| `WithAdditionalNetwork(AirIdentifier, Func<ImageJobNetworkParamsBuilder, ImageJobNetworkParamsBuilder>)` | Add LoRA or embedding using a configuration action |

### ControlNet

| Method | Description |
|--------|-------------|
| `WithControlNet(ImageJobControlNet)` | Add ControlNet configuration |
| `WithControlNet(ImageJobControlNetBuilder)` | Add ControlNet using a builder |
| `WithControlNet(Func<ImageJobControlNetBuilder, ImageJobControlNetBuilder>)` | Add ControlNet using a configuration action |

#### Job Configuration

| Method | Description |
|--------|-------------|
| `WithQuantity(int)` | Number of images to generate (range: 1-10, default: 1) |
| `WithPriority(Priority)` | Set job priority configuration |
| `WithCallbackUrl(string)` | Set webhook URL for completion notification |
| `WithRetries(int)` | Set automatic retry count on failure (default: 0) |
| `WithTimeout(string)` | Set job timeout in HH:mm:ss format (default: "00:10:00") |

#### Custom Properties

| Method | Description |
|--------|-------------|
| `WithProperty(string, JsonElement)` | Add custom property for tracking/querying (use `JsonSerializer.SerializeToElement()` to convert values) |

#### Execution

| Method | Description |
|--------|-------------|
| `ExecuteAsync(CancellationToken)` | Submit the job and return job status collection with polling token |
| `ExecuteBatchAsync(IEnumerable<TextToImageBuilder>, CancellationToken)` | Submit multiple jobs as a batch |

## Builder Design Principles

### Immutability

The TextToImageBuilder is an immutable record. Each method returns a new instance:

```csharp
var baseJob = sdkClient.Jobs.CreateTextToImage()
    .WithModel(model)
    .WithSize(1024, 1024);

// Both are independent - original is unchanged
var job1 = baseJob.WithPrompt("landscape");
var job2 = baseJob.WithPrompt("portrait");
```

### Thread Safety

Because the builder is immutable, it's thread-safe and can be shared:

```csharp
// Safe to share across threads
private readonly TextToImageJobBuilder _baseJob;

public MyService(ISdkClient client)
{
    _baseJob = client.Jobs
        .CreateTextToImage()
        .WithModel(model)
        .WithSize(1024, 1024)
        .WithSteps(30);
}

public Task<Result<JobStatusCollection>> GenerateAsync(string prompt)
    => _baseJob.WithPrompt(prompt).ExecuteAsync();
```

### Validation

The builder validates parameters immediately:

```csharp
// Throws ArgumentException - prompt cannot be empty
builder.WithPrompt("");

// Throws ArgumentOutOfRangeException - steps must be 1-100  
// (validated in ImageJobParamsBuilder)
builder.WithParams(p => p.WithSteps(0));

// Throws InvalidOperationException - model and prompt required
await sdkClient.Jobs.CreateTextToImage().ExecuteAsync();
```

## Best Practices

### Use the Fluent Builder

Always use `CreateTextToImage()` for type-safe, validated job creation:

```csharp
// Recommended - type-safe, validated, immutable
await sdkClient.Jobs
    .CreateTextToImage()
    .WithModel(model)
    .WithPrompt("landscape")
    .ExecuteAsync();

// Not recommended - manual construction requires JsonElement handling
var request = new TextToImageJobRequest
{
    Model = model,
    Params = new ImageJobParams { Prompt = "landscape" }
};
// No direct submit method exists for manually constructed requests
```

### Cache Base Configurations

Take advantage of immutability to cache common configurations:

```csharp
// Cache common configurations
private readonly TextToImageJobBuilder _baseJob;

public ImageService(ISdkClient client)
{
    _baseJob = client.Jobs
        .CreateTextToImage()
        .WithModel(commonModel)
        .WithSize(1024, 1024)
        .WithSteps(30);
}

public Task<Result<JobStatusCollection>> GenerateAsync(string prompt)
    => _baseJob.WithPrompt(prompt).ExecuteAsync();
```

### Use Custom Properties for Tracking

Add metadata to jobs for easy filtering:

```csharp
await sdkClient.Jobs
    .CreateTextToImage()
    .WithModel(model)
    .WithPrompt(prompt)
    .WithProperty("userId", JsonSerializer.SerializeToElement(userId))
    .WithProperty("sessionId", JsonSerializer.SerializeToElement(sessionId))
    .WithProperty("timestamp", JsonSerializer.SerializeToElement(DateTime.UtcNow.Ticks))
    .ExecuteAsync();
```

### Handle Async Operations Properly

Jobs are asynchronous - use appropriate polling strategies:

```csharp
// Good - properly async with polling
var submitResult = await sdkClient.Jobs
    .CreateTextToImage()
    .WithModel(model)
    .WithPrompt(prompt)
    .ExecuteAsync();

if (submitResult is Result<JobStatusCollection>.Success success)
{
    var token = success.Data.Token;
    
    // Option 1: Poll with delay
    await Task.Delay(5000);
    var status = await sdkClient.Jobs.Query.GetByTokenAsync(token);
    
    // Option 2: Use wait parameter (blocks up to ~10 min)
    var completed = await sdkClient.Jobs.Query
        .WithWait()
        .GetByTokenAsync(token);
}
```

## Error Handling

All methods return `Result<T>` for consistent error handling:

```csharp
var result = await sdkClient.Jobs
    .CreateTextToImage()
    .WithModel(model)
    .WithPrompt(prompt)
    .ExecuteAsync();

switch (result)
{
    case Result<JobStatusCollection>.Success success:
        Console.WriteLine($"Submitted: {success.Data.Token}");
        break;
        
    case Result<JobStatusCollection>.ApiError apiError:
        Console.WriteLine($"API Error: {apiError.Message}");
        break;
        
    case Result<JobStatusCollection>.NetworkError networkError:
        Console.WriteLine($"Network Error: {networkError.Exception.Message}");
        break;
}
```

## Next Steps

- [SDK Introduction](sdk-introduction.md) - Overview of all SDK services
- [AIR Identifiers](air-identifier.md) - Learn about model identifiers
- [Error Handling](error-handling.md) - Comprehensive error handling patterns
