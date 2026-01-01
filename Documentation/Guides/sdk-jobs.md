---
title: Jobs Service
description: Learn how to submit, track, and manage image generation jobs using the CivitaiSharp.Sdk Jobs service with fluent builders.
---

# Jobs Service

The Jobs service provides comprehensive functionality for submitting, tracking, and managing image generation jobs through the Civitai Generator API.

## Overview

The Jobs service provides two fluent builders:

1. **ImageGenerationBuilder** - Fluent, immutable builder for creating and submitting image generation jobs (accessed via `CreateImage()`)
2. **JobQueryBuilder** - Fluent, immutable builder for querying and managing jobs (accessed via `Query` property)

The ImageGenerationBuilder supports both text-to-image and image-to-image generation modes.

Both builders follow CivitaiSharp's immutable, thread-safe design pattern.

## Creating Jobs

### Basic Image Generation

Use the `CreateImage()` method to get a fluent builder:

```csharp
var result = await sdkClient.Jobs
    .CreateImage()
    .WithAir(new AirIdentifier("sdxl", AirAssetType.Checkpoint, "civitai", 4201, 130072))
    .WithPositivePrompt("a beautiful sunset over mountains")
    .WithNegativePrompt("blurry, low quality")
    .WithScheduler(Scheduler.EulerAncestral)
    .WithDimensions(1024, 1024)
    .WithSteps(30)
    .WithConfigurationScale(7.5m)
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
    .CreateImage()
    .WithAir(model)
    .WithPositivePrompt("detailed portrait")
    .WithScheduler(Scheduler.DpmPlusPlus2MKarras)
    .WithSeed(12345)
    .WithSteps(50)
    .WithConfigurationScale(8.5m)
    .WithQuantity(4)  // Generate 4 images
    .WithClipSkip(2)
    .WithCallbackUrl("https://example.tld/webhook")
    .WithRetries(3)
    .ExecuteAsync();
```

### Using Additional Networks (LoRAs)

Add LoRAs and other networks to enhance generation:

```csharp
var lora = new AirIdentifier("sdxl", AirAssetType.Lora, "civitai", 123456, 789);

var result = await sdkClient.Jobs
    .CreateImage()
    .WithAir(baseModel)
    .WithPositivePrompt("character portrait")
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
    .CreateImage()
    .WithAir(model)
    .WithPositivePrompt("person standing")
    .WithControlNet(builder => builder
        .WithModel(controlNetModel)
        .WithImage("https://example.tld/pose.png")
        .WithWeight(1.0m)
        .WithStartingControlStep(0.0m)
        .WithEndingControlStep(1.0m))
    .ExecuteAsync();
```

### Batch Job Submission

Submit multiple jobs at once:

```csharp
var landscapeJob = sdkClient.Jobs
    .CreateImage()
    .WithAir(firstCheckpoint)
    .WithPositivePrompt("landscape");

var portraitJob = sdkClient.Jobs
    .CreateImage()
    .WithAir(secondCheckpoint)
    .WithPositivePrompt("portrait");

var result = await landscapeJob.ExecuteBatchAsync([portraitJob]);

if (result is Result<JobStatusCollection>.Success success)
{
    Console.WriteLine($"Batch submitted with token: {success.Data.Token}");
}
```

### Complete Parameter Example

Demonstration of all available image generation parameters:

```csharp
var baseCheckpoint = new AirIdentifier("sdxl", AirAssetType.Checkpoint, "civitai", 4201, 130072);

var fullParameterJob = await sdkClient.Jobs
    .CreateImage()
    .WithAir(baseCheckpoint)
    .WithPositivePrompt("masterpiece, best quality, professional photograph, cyberpunk street scene, neon lights, rain, reflections, highly detailed, 8k uhd")
    .WithNegativePrompt("blurry, low quality, bad anatomy, deformed, watermark, signature, text, jpeg artifacts, worst quality, low resolution")
    .WithScheduler(Scheduler.DpmPlusPlus2MKarras)
    .WithDimensions(1024, 1536) // Portrait orientation
    .WithSteps(40) // Higher steps for quality
    .WithConfigurationScale(8.5m) // Strong prompt adherence
    .WithSeed(987654321) // Reproducible results
    .WithClipSkip(2) // Better artistic interpretation
    .WithQuantity(4) // Generate 4 variations
    .WithPriority(Priority.Default)
    .WithCallbackUrl("https://example.tld/webhook/generation-complete") // Async notification
    .WithRetries(3) // Retry failed jobs
    .WithTimeout(TimeSpan.FromMinutes(15)) // 15-minute timeout
    .ExecuteAsync();

if (fullParameterJob is Result<JobStatusCollection>.Success jobSuccess)
{
    Console.WriteLine($"Submitted {jobSuccess.Data.Jobs.Count} jobs");
    Console.WriteLine($"Batch token: {jobSuccess.Data.Token}");
}
```

### Advanced Multi-LoRA Configuration

Combine multiple LoRAs with fine-tuned strengths and trigger words:

```csharp
var baseCheckpoint = new AirIdentifier("sdxl", AirAssetType.Checkpoint, "civitai", 133005, 348913);
var characterLoRA = new AirIdentifier("sdxl", AirAssetType.Lora, "civitai", 234567, 345678);
var clothingLoRA = new AirIdentifier("sdxl", AirAssetType.Lora, "civitai", 345678, 456789);
var styleLoRA = new AirIdentifier("sdxl", AirAssetType.Lora, "civitai", 456789, 567890);
var lightingLoRA = new AirIdentifier("sdxl", AirAssetType.Lora, "civitai", 567890, 678901);

var multiLoRAJob = await sdkClient.Jobs
    .CreateImage()
    .WithAir(baseCheckpoint)
    .WithPositivePrompt("cinematic_lighting photo of fantasy_character wearing medieval_armor in dramatic pose, volumetric fog, golden hour")
    .WithNegativePrompt("blurry, low quality, bad hands, bad face, deformed, ugly")
    .WithScheduler(Scheduler.DpmPlusPlus2MSdeKarras)
    .WithDimensions(768, 1024)
    .WithSteps(35)
    .WithConfigurationScale(7.5m)
    // Character LoRA - highest strength for defining features
    .WithAdditionalNetwork(characterLoRA, network => network
        .WithStrength(0.95m)
        .WithTriggerWord("fantasy_character"))
    // Clothing LoRA - high strength for accurate outfit
    .WithAdditionalNetwork(clothingLoRA, network => network
        .WithStrength(0.85m)
        .WithTriggerWord("medieval_armor"))
    // Style LoRA - medium strength for artistic influence
    .WithAdditionalNetwork(styleLoRA, network => network
        .WithStrength(0.65m)
        .WithTriggerWord("cinematic_style"))
    // Lighting LoRA - subtle strength to enhance atmosphere
    .WithAdditionalNetwork(lightingLoRA, network => network
        .WithStrength(0.45m)
        .WithTriggerWord("cinematic_lighting"))
    .WithQuantity(2)
    .ExecuteAsync();

if (multiLoRAJob is Result<JobStatusCollection>.Success loraSuccess)
{
    Console.WriteLine($"Multi-LoRA job token: {loraSuccess.Data.Token}");
}
```

### ControlNet with LoRA Integration

Combine pose control with style enhancement:

```csharp
var baseCheckpoint = new AirIdentifier("sdxl", AirAssetType.Checkpoint, "civitai", 4201, 130072);
var portraitStyleLoRA = new AirIdentifier("sdxl", AirAssetType.Lora, "civitai", 234567, 345678);
var detailEnhancerLoRA = new AirIdentifier("sdxl", AirAssetType.Lora, "civitai", 345678, 456789);
var controlNetPose = new AirIdentifier("sdxl", AirAssetType.ControlNet, "civitai", 456789, 567890);

var controlNetWithLoRAJob = await sdkClient.Jobs
    .CreateImage()
    .WithAir(baseCheckpoint)
    .WithPositivePrompt("professional_portrait of a business executive, sharp focus, studio lighting, detailed facial features, formal attire")
    .WithNegativePrompt("blurry, low quality, bad anatomy, distorted face, casual clothing")
    .WithScheduler(Scheduler.EulerAncestral)
    .WithDimensions(832, 1216) // Professional portrait ratio
    .WithSteps(40)
    .WithConfigurationScale(7.0m)
    // ControlNet for precise pose guidance
    .WithControlNet(controlNet => controlNet
        .WithModel(controlNetPose)
        .WithImage("https://example.tld/reference-pose.png")
        .WithWeight(1.0m) // Full control strength
        .WithStartingControlStep(0.0m) // Apply from beginning
        .WithEndingControlStep(0.75m)) // Release control near end for natural finish
    // Portrait style LoRA for professional look
    .WithAdditionalNetwork(portraitStyleLoRA, network => network
        .WithStrength(0.8m)
        .WithTriggerWord("professional_portrait"))
    // Detail enhancement LoRA for sharpness
    .WithAdditionalNetwork(detailEnhancerLoRA, network => network
        .WithStrength(0.6m))
    .WithCallbackUrl("https://example.tld/webhook/portrait-complete")
    .ExecuteAsync();

if (controlNetWithLoRAJob is Result<JobStatusCollection>.Success controlSuccess)
{
    Console.WriteLine($"ControlNet+LoRA job submitted: {controlSuccess.Data.Token}");
    
    // Wait for completion and retrieve results
    var completedJob = await sdkClient.Jobs.Query
        .WithWait()
        .WithDetailed()
        .GetByTokenAsync(controlSuccess.Data.Token);
    
    if (completedJob is Result<JobStatusCollection>.Success completed)
    {
        foreach (var job in completed.Data.Jobs)
        {
            if (job.Status == "succeeded" && job.Result?.BlobUrl is string imageUrl)
            {
                Console.WriteLine($"Generated portrait: {imageUrl}");
                
                // Optionally download the image
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                await File.WriteAllBytesAsync($"portrait_{job.JobId}.png", imageBytes);
            }
        }
    }
}
```

### Complex Batch Workflow

Submit multiple jobs with different configurations in a single batch:

```csharp
var checkpoint = new AirIdentifier("sdxl", AirAssetType.Checkpoint, "civitai", 4201, 130072);
var styleLoRARealistic = new AirIdentifier("sdxl", AirAssetType.Lora, "civitai", 234567, 345678);
var styleLoRAAnime = new AirIdentifier("sdxl", AirAssetType.Lora, "civitai", 345678, 456789);

// Job 1: Realistic landscape
var landscapeJob = sdkClient.Jobs
    .CreateImage()
    .WithAir(checkpoint)
    .WithPositivePrompt("breathtaking mountain landscape, golden hour, professional photography")
    .WithNegativePrompt("blurry, low quality")
    .WithScheduler(Scheduler.DpmPlusPlus2MKarras)
    .WithDimensions(1344, 768) // Landscape 16:9
    .WithSteps(30)
    .WithConfigurationScale(7.0m)
    .WithAdditionalNetwork(styleLoRARealistic, network => network
        .WithStrength(0.7m));

// Job 2: Anime character portrait
var animePortraitJob = sdkClient.Jobs
    .CreateImage()
    .WithAir(checkpoint)
    .WithPositivePrompt("anime character portrait, detailed face, colorful, masterpiece")
    .WithNegativePrompt("blurry, bad anatomy, low quality")
    .WithScheduler(Scheduler.EulerAncestral)
    .WithDimensions(768, 1024) // Portrait 3:4
    .WithSteps(35)
    .WithConfigurationScale(8.0m)
    .WithAdditionalNetwork(styleLoRAAnime, network => network
        .WithStrength(0.9m)
        .WithTriggerWord("anime_style"));

// Job 3: Abstract art
var abstractJob = sdkClient.Jobs
    .CreateImage()
    .WithAir(checkpoint)
    .WithPositivePrompt("abstract digital art, vibrant colors, geometric patterns, modern")
    .WithNegativePrompt("realistic, photographic, blurry")
    .WithScheduler(Scheduler.Euler)
    .WithDimensions(1024, 1024) // Square 1:1
    .WithSteps(40)
    .WithConfigurationScale(9.0m);

// Submit all jobs as a batch
var batchResult = await landscapeJob.ExecuteBatchAsync([animePortraitJob, abstractJob]);

if (batchResult is Result<JobStatusCollection>.Success batchSuccess)
{
    Console.WriteLine($"Batch of {batchSuccess.Data.Jobs.Count} jobs submitted");
    Console.WriteLine($"Batch token: {batchSuccess.Data.Token}");
    
    // Poll for status updates
    var statusResult = await sdkClient.Jobs.Query
        .WithDetailed()
        .GetByTokenAsync(batchSuccess.Data.Token);
    
    if (statusResult is Result<JobStatusCollection>.Success statusSuccess)
    {
        foreach (var job in statusSuccess.Data.Jobs)
        {
            Console.WriteLine($"Job {job.JobId}: {job.Status}");
        }
    }
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
    .CreateImage()
    .WithAir(model)
    .WithPositivePrompt("landscape")
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

### ImageGenerationBuilder Methods

Accessed through `sdkClient.Jobs.CreateImage()`:

#### Required Parameters

| Method | Description |
|--------|-------------|
| `WithAir(AirIdentifier)` | Set the base model (required) |
| `WithPositivePrompt(string)` | Set the positive prompt (required) |

#### Image Parameters

| Method | Description |
|--------|-------------|
| `WithNegativePrompt(string)` | Set negative prompt |
| `WithScheduler(Scheduler)` | Set sampling algorithm (e.g., Euler, EulerAncestral, DpmPlusPlus2MKarras) |
| `WithDimensions(int, int)` | Set width and height (must be multiples of 8, range: 64-2048) |
| `WithSteps(int)` | Set sampling steps (range: 1-100, default: 20) |
| `WithConfigurationScale(decimal)` | Set CFG scale (range: 1-30, default: 7) |
| `WithSeed(long)` | Set seed for reproducibility |
| `WithClipSkip(int)` | Set CLIP skip layers (range: 1-12) |

#### Additional Networks

| Method | Description |
|--------|-------------|
| `WithAdditionalNetwork(AirIdentifier, ImageJobNetworkParams)` | Add LoRA or embedding with network configuration |
| `WithAdditionalNetwork(AirIdentifier, ImageJobNetworkParamsBuilder)` | Add LoRA or embedding using a builder |
| `WithAdditionalNetwork(AirIdentifier, Func<ImageJobNetworkParamsBuilder, ImageJobNetworkParamsBuilder>)` | Add LoRA or embedding using a configuration action |

#### ControlNet

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
| `WithTimeout(TimeSpan)` | Set job timeout (default: 10 minutes) |

#### Custom Properties

| Method | Description |
|--------|-------------|
| `WithProperty(string, JsonElement)` | Add custom property for tracking/querying (use `JsonSerializer.SerializeToElement()` to convert values) |

#### Execution

| Method | Description |
|--------|-------------|
| `ExecuteAsync(CancellationToken)` | Submit the job and return job status collection with polling token |
| `ExecuteBatchAsync(IEnumerable<ImageGenerationBuilder>, CancellationToken)` | Submit multiple jobs as a batch |

## Builder Design Principles

### Immutability

The ImageGenerationBuilder is an immutable record. Each method returns a new instance:

```csharp
var baseJobConfiguration = sdkClient.Jobs.CreateImage()
    .WithAir(checkpointModel)
    .WithDimensions(1024, 1024);

// Both are independent - original is unchanged
var landscapeJob = baseJobConfiguration.WithPositivePrompt("landscape");
var portraitJob = baseJobConfiguration.WithPositivePrompt("portrait");
```

### Thread Safety

Because the builder is immutable, it's thread-safe and can be shared:

```csharp
// Safe to share across threads
private readonly ImageGenerationBuilder _baseJob;

public MyService(ISdkClient client)
{
    _baseJob = client.Jobs
        .CreateImage()
        .WithAir(model)
        .WithDimensions(1024, 1024)
        .WithSteps(30);
}

public Task<Result<JobStatusCollection>> GenerateAsync(string prompt)
    => _baseJob.WithPositivePrompt(prompt).ExecuteAsync();
```

### Validation

The builder validates parameters immediately:

```csharp
// Throws ArgumentException - prompt cannot be empty
builder.WithPositivePrompt("");

// Throws ArgumentOutOfRangeException - steps must be 1-100  
// (validated in ImageJobParamsBuilder)
builder.WithParams(p => p.WithSteps(0));

// Throws InvalidOperationException - model and prompt required
await sdkClient.Jobs.CreateImage().ExecuteAsync();
```

## Best Practices

### Use the Fluent Builder

Always use `CreateImage()` for type-safe, validated job creation:

```csharp
// Recommended - type-safe, validated, immutable
await sdkClient.Jobs
    .CreateImage()
    .WithAir(model)
    .WithPositivePrompt("landscape")
    .ExecuteAsync();

// Not recommended - manual construction requires JsonElement handling
var request = new ImageGenerationJobRequest
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
private readonly ImageGenerationBuilder _baseJob;

public ImageService(ISdkClient client)
{
    _baseJob = client.Jobs
        .CreateImage()
        .WithAir(commonModel)
        .WithDimensions(1024, 1024)
        .WithSteps(30);
}

public Task<Result<JobStatusCollection>> GenerateAsync(string prompt)
    => _baseJob.WithPositivePrompt(prompt).ExecuteAsync();
```

### Use Custom Properties for Tracking

Add metadata to jobs for easy filtering:

```csharp
await sdkClient.Jobs
    .CreateImage()
    .WithAir(model)
    .WithPositivePrompt(prompt)
    .WithProperty("userId", JsonSerializer.SerializeToElement(userId))
    .WithProperty("sessionId", JsonSerializer.SerializeToElement(sessionId))
    .WithProperty("timestamp", JsonSerializer.SerializeToElement(DateTime.UtcNow.Ticks))
    .ExecuteAsync();
```

### Handle Async Operations Properly

**Why polling is necessary:** Image generation jobs typically take 30 seconds to several minutes to complete, depending on model complexity, queue position, and service load. The API processes requests asynchronously, so you must either poll for results or use webhook callbacks (see [Webhook Callbacks Guide](sdk-webhooks.md) for an alternative approach).

**Choosing a polling strategy:**

CivitaiSharp provides two approaches for checking job status:

#### Option 1: Manual Polling with Task.Delay

Use manual polling when you need:
- Custom retry logic or exponential backoff
- Specific polling intervals based on your requirements
- Integration with existing async workflows or state machines
- Fine-grained control over cancellation and timeout behavior

```csharp
var submitResult = await sdkClient.Jobs
    .CreateImage()
    .WithAir(model)
    .WithPositivePrompt(prompt)
    .ExecuteAsync();

if (submitResult is Result<JobStatusCollection>.Success success)
{
    var token = success.Data.Token;
    
    // Poll every 5 seconds until complete
    while (true)
    {
        await Task.Delay(5000); // Wait 5 seconds between checks
        
        var statusResult = await sdkClient.Jobs.Query.GetByTokenAsync(token);
        
        if (statusResult is Result<JobStatus>.Success statusSuccess)
        {
            var status = statusSuccess.Data;
            
            // Check if job is complete (scheduled = false)
            if (!status.Scheduled)
            {
                if (status.Result?.BlobUrl is string imageUrl)
                {
                    Console.WriteLine($"Job complete! Image URL: {imageUrl}");
                }
                break;
            }
            
            Console.WriteLine($"Job still processing. Position in queue: {status.Position}");
        }
    }
}
```

**Best practices for manual polling:**
- Poll every 5-10 seconds to balance responsiveness and API load
- Consider exponential backoff for longer-running jobs (5s → 10s → 20s)
- Implement a maximum retry limit to avoid infinite loops
- Use `CancellationToken` to support graceful cancellation

#### Option 2: Automatic Waiting with WithWait()

Use the `WithWait()` parameter when you want:
- Simplified code without manual polling loops
- The API to handle polling automatically (up to ~10 minutes)
- Convenience for scripts, CLIs, or simple applications
- Blocking behavior where the call waits until the job completes

```csharp
var submitResult = await sdkClient.Jobs
    .CreateImage()
    .WithAir(model)
    .WithPositivePrompt(prompt)
    .ExecuteAsync();

if (submitResult is Result<JobStatusCollection>.Success success)
{
    var token = success.Data.Token;
    
    // Single call that waits for completion (up to ~10 minutes)
    var completedResult = await sdkClient.Jobs.Query
        .WithWait()
        .GetByTokenAsync(token);
    
    if (completedResult is Result<JobStatus>.Success completedSuccess)
    {
        var status = completedSuccess.Data;
        
        if (status.Result?.BlobUrl is string imageUrl)
        {
            Console.WriteLine($"Job complete! Image URL: {imageUrl}");
        }
    }
}
```

**Trade-offs:**
- **WithWait()** blocks the calling thread but is simpler to implement
- **Manual polling** requires more code but offers flexibility and control
- **Webhooks** (recommended for production) eliminate polling entirely - see [Webhook Callbacks Guide](sdk-webhooks.md)

## Error Handling

All methods return `Result<T>` for consistent error handling:

```csharp
var result = await sdkClient.Jobs
    .CreateImage()
    .WithAir(model)
    .WithPositivePrompt(prompt)
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
