---
title: Coverage Service
description: Learn how to check model and resource availability on the Civitai generation infrastructure before submitting jobs.
---

# Coverage Service

The Coverage service allows you to check the availability of AI models and resources across the Civitai generation infrastructure before submitting jobs. This helps prevent job failures due to unavailable resources.

## Overview

The Coverage service provides methods to:
- Check availability of single or multiple models
- Identify which providers support specific models
- Get queue depth information for resource planning

## Basic Usage

### Check Single Model Availability

```csharp
// Create AIR identifier using constructor
var checkpointModel = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    modelId: 4201,
    versionId: 130072);

// Or use the builder pattern for more flexibility
var checkpointModelFromBuilder = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithSource(AirSource.Civitai)
    .WithModelId(4201)
    .WithVersionId(130072)
    .Build();

var coverageResult = await sdkClient.Coverage.GetAsync(checkpointModel, cancellationToken);

if (coverageResult is Result<ProviderAssetAvailability>.Success coverageSuccess)
{
    Console.WriteLine($"Availability: {coverageSuccess.Data.Availability}");
    Console.WriteLine($"Workers: {coverageSuccess.Data.Workers}");
    
    if (coverageSuccess.Data.Availability == AvailabilityStatus.Available)
    {
        Console.WriteLine($"Model is available with {coverageSuccess.Data.Workers} workers");
    }
}
```

### Check Multiple Models

```csharp
var checkpointModel = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    modelId: 4201,
    versionId: 130072);

var loraModel = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Lora,
    AirSource.Civitai,
    modelId: 328553,
    versionId: 368189);

var vaeModel = new AirIdentifier(
    AirEcosystem.StableDiffusion1,
    AirAssetType.Vae,
    AirSource.Civitai,
    modelId: 22354,
    versionId: 123456);

var modelsToCheck = new[] { checkpointModel, loraModel, vaeModel };

var batchCoverageResult = await sdkClient.Coverage.GetAsync(modelsToCheck, cancellationToken);

if (batchCoverageResult is Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success batchSuccess)
{
    foreach (var (modelIdentifier, availability) in batchSuccess.Data)
    {
        Console.WriteLine($"{modelIdentifier}: {availability.Availability} (Workers: {availability.Workers})");
    }
}
```

## Understanding Results

### ProviderAssetAvailability

The main result type containing availability information:

| Property | Type | Description |
|----------|------|-------------|
| `Availability` | `AvailabilityStatus` | The availability status (Available, Unavailable, Degraded) |
| `Workers` | `int` | Number of workers with this model loaded (0 means unavailable) |

### AvailabilityStatus

Enum values for availability status:

| Value | Description |
|-------|-------------|
| `Available` | Model is available and ready for generation |
| `Unavailable` | Model is not currently available |
| `Degraded` | Model is available but with limited capacity (may experience delays) |

## Common Use Cases

### Use Case 1: Simple Availability Check

**Scenario**: Check if a model is ready before submitting a single job.

**When to use**: Quick validation before simple image generation tasks.

```csharp
public async Task<bool> IsModelReadyAsync(AirIdentifier model, CancellationToken cancellationToken)
{
    var coverageResult = await sdkClient.Coverage.GetAsync(model, cancellationToken);
    
    return coverageResult is Result<ProviderAssetAvailability>.Success success &&
           success.Data.Availability == AvailabilityStatus.Available &&
           success.Data.Workers > 0;
}

// Usage example
var sdxlCheckpoint = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(4201)
    .WithVersionId(130072)
    .Build();

if (await IsModelReadyAsync(sdxlCheckpoint, cancellationToken))
{
    // Submit job
    var jobSubmissionResult = await sdkClient.Jobs
        .CreateImage()
        .WithAir(sdxlCheckpoint)
        .WithPositivePrompt("a beautiful landscape")
        .WithDimensions(1024, 1024)
        .ExecuteAsync(cancellationToken);
}
```

### Use Case 2: Complex Multi-Resource Validation

**Scenario**: Validate all resources (checkpoint + multiple LoRAs + ControlNet) before submitting a complex job.

**When to use**: Production applications where job failures are costly.

```csharp
public sealed class ResourceValidator(ISdkClient sdkClient)
{
    public async Task<(bool IsValid, List<string> Issues)> ValidateResourcesAsync(
        AirIdentifier checkpoint,
        IReadOnlyList<AirIdentifier> loras,
        AirIdentifier? controlNet = null,
        CancellationToken cancellationToken = default)
    {
        var issues = new List<string>();
        
        // Collect all resources
        var allResources = new List<AirIdentifier> { checkpoint };
        allResources.AddRange(loras);
        if (controlNet is not null)
        {
            allResources.Add(controlNet);
        }
        
        // Single API call for all resources
        var result = await sdkClient.Coverage.GetAsync(allResources, cancellationToken);
        
        if (result is not Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success success)
        {
            issues.Add($"Failed to check coverage: {result.ErrorOrDefault?.Message}");
            return (false, issues);
        }
        
        // Validate each resource
        foreach (var (resource, availability) in success.Data)
        {
            var resourceType = resource.AssetType switch
            {
                AirAssetType.Checkpoint => "Checkpoint",
                AirAssetType.Lora => "LoRA",
                _ => "Resource"
            };
            
            switch (availability.Availability)
            {
                case AvailabilityStatus.Unavailable:
                    issues.Add($"{resourceType} '{resource}' is unavailable (0 workers)");
                    break;
                    
                case AvailabilityStatus.Degraded:
                    issues.Add($"{resourceType} '{resource}' is degraded ({availability.Workers} workers) - expect delays");
                    break;
                    
                case AvailabilityStatus.Available when availability.Workers < 3:
                    issues.Add($"{resourceType} '{resource}' has low capacity ({availability.Workers} workers) - may be slow");
                    break;
            }
        }
        
        var isValid = issues.All(i => !i.Contains("unavailable"));
        return (isValid, issues);
    }
}

// Full Example Usage
var validator = new ResourceValidator(sdkClient);

var sdxlCheckpoint = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(4201)
    .WithVersionId(130072)
    .Build();

var characterLora = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Lora)
    .WithModelId(123456)
    .WithVersionId(789012)
    .Build();

var styleLora = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Lora)
    .WithModelId(234567)
    .WithVersionId(890123)
    .Build();

var loraModels = new[] { characterLora, styleLora };

var (isValid, validationIssues) = await validator.ValidateResourcesAsync(
    sdxlCheckpoint, 
    loraModels,
    cancellationToken: cancellationToken);

if (!isValid)
{
    Console.WriteLine("Resource validation failed:");
    foreach (var issue in validationIssues)
    {
        Console.WriteLine($"  - {issue}");
    }
    return;
}

// All resources valid - proceed with job
var complexJobResult = await sdkClient.Jobs
    .CreateImage()
    .WithAir(sdxlCheckpoint)
    .WithPositivePrompt("detailed character portrait")
    .WithAdditionalNetwork(characterLora, NetworkBuilder.Create()
        .WithStrength(0.8m)
        .Build())
    .WithAdditionalNetwork(styleLora, NetworkBuilder.Create()
        .WithStrength(0.6m)
        .Build())
    .WithControlNet(ControlNetBuilder.Create()
        .WithImageUrl("https://example.tld/pose.png")
        .WithPreprocessor(ControlNetPreprocessor.Canny)
        .WithWeight(1.0m)
        .Build())
    .WithDimensions(768, 1024)
    .ExecuteAsync(cancellationToken);
```

### Use Case 3: Load Balancing with Worker Count

**Scenario**: Select the best available model variant based on worker availability.

**When to use**: When you have multiple versions/variants of a model and want optimal performance.

```csharp
public sealed class ModelSelector(ISdkClient sdkClient)
{
    public async Task<AirIdentifier?> SelectBestAvailableModelAsync(
        IReadOnlyList<AirIdentifier> candidates,
        CancellationToken cancellationToken = default)
    {
        if (candidates.Count == 0)
            return null;
            
        var result = await sdkClient.Coverage.GetAsync(candidates, cancellationToken);
        
        if (result is not Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success success)
            return null;
        
        // Select model with highest worker count
        var bestModel = success.Data
            .Where(kvp => kvp.Value.Availability == AvailabilityStatus.Available)
            .OrderByDescending(kvp => kvp.Value.Workers)
            .Select(kvp => new { Model = kvp.Key, kvp.Value.Workers })
            .FirstOrDefault();
        
        if (bestModel is not null)
        {
            Console.WriteLine($"Selected model with {bestModel.Workers} workers");
            return bestModel.Model;
        }
        
        // Fallback: accept degraded if no fully available models
        var degradedModel = success.Data
            .Where(kvp => kvp.Value.Availability == AvailabilityStatus.Degraded)
            .OrderByDescending(kvp => kvp.Value.Workers)
            .Select(kvp => kvp.Key)
            .FirstOrDefault();
        
        if (degradedModel is not null)
        {
            Console.WriteLine("Warning: Using degraded model (all preferred models unavailable)");
        }
        
        return degradedModel;
    }
}

// Full Example
var selector = new ModelSelector(sdkClient);

// Multiple checkpoint options (different versions or similar models)
var sdxlVersion10 = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(4201)
    .WithVersionId(130072)
    .Build();

var sdxlVersion09 = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(4201)
    .WithVersionId(128713)
    .Build();

var ponyXlCheckpoint = new AirBuilder()
    .WithEcosystem(AirEcosystem.Pony)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(257749)
    .WithVersionId(290640)
    .Build();

var checkpointCandidates = new[] { sdxlVersion10, sdxlVersion09, ponyXlCheckpoint };

var bestAvailableCheckpoint = await selector.SelectBestAvailableModelAsync(
    checkpointCandidates,
    cancellationToken);

if (bestAvailableCheckpoint is null)
{
    Console.WriteLine("No available checkpoints found");
    return;
}

Console.WriteLine($"Using checkpoint: {bestAvailableCheckpoint}");

var selectionJobResult = await sdkClient.Jobs
    .CreateImage()
    .WithAir(bestAvailableCheckpoint)
    .WithPositivePrompt("high quality render")
    .WithDimensions(1024, 1024)
    .ExecuteAsync(cancellationToken);
```

### Use Case 4: Retry with Fallback Models

**Scenario**: Attempt job with preferred model, fallback to alternatives if unavailable.

**When to use**: Production systems requiring high reliability and automatic failover.

```csharp
public sealed class RobustJobSubmitter(ISdkClient sdkClient)
{
    public async Task<Result<JobStatusCollection>> SubmitWithFallbackAsync(
        IReadOnlyList<AirIdentifier> checkpointPriority,
        string prompt,
        int width = 1024,
        int height = 1024,
        CancellationToken cancellationToken = default)
    {
        // Check all candidates at once
        var coverageResult = await sdkClient.Coverage.GetAsync(checkpointPriority, cancellationToken);
        
        if (coverageResult is not Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success coverage)
        {
            Console.WriteLine("Coverage check failed, attempting with first checkpoint anyway");
            return await SubmitJobAsync(checkpointPriority[0], prompt, width, height, cancellationToken);
        }
        
        // Try checkpoints in priority order
        foreach (var checkpoint in checkpointPriority)
        {
            if (!coverage.Data.TryGetValue(checkpoint, out var availability))
                continue;
            
            if (availability.Availability == AvailabilityStatus.Available && availability.Workers > 0)
            {
                Console.WriteLine($"Using checkpoint: {checkpoint} ({availability.Workers} workers)");
                return await SubmitJobAsync(checkpoint, prompt, width, height, cancellationToken);
            }
        }
        
        // Check for degraded models as last resort
        foreach (var checkpoint in checkpointPriority)
        {
            if (coverage.Data.TryGetValue(checkpoint, out var availability) &&
                availability.Availability == AvailabilityStatus.Degraded &&
                availability.Workers > 0)
            {
                Console.WriteLine($"Warning: Using degraded checkpoint: {checkpoint} ({availability.Workers} workers)");
                return await SubmitJobAsync(checkpoint, prompt, width, height, cancellationToken);
            }
        }
        
        // All unavailable - return error
        return new Result<JobStatusCollection>.Failure(
            new Error(
                ErrorCode.ResourceUnavailable,
                "All checkpoint models are currently unavailable"
            )
        );
    }
    
    private Task<Result<JobStatusCollection>> SubmitJobAsync(
        AirIdentifier checkpoint,
        string prompt,
        int width,
        int height,
        CancellationToken cancellationToken)
    {
        return sdkClient.Jobs
            .CreateImage()
            .WithAir(checkpoint)
            .WithPositivePrompt(prompt)
            .WithDimensions(width, height)
            .ExecuteAsync(cancellationToken);
    }
}

// Full Example
var submitter = new RobustJobSubmitter(sdkClient);

// Define checkpoint priority (most preferred first)
var primaryCheckpoint = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(4201)
    .WithVersionId(130072)
    .Build();

var fallbackCheckpoint = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(133005)
    .WithVersionId(348913)
    .Build();

var secondaryFallbackCheckpoint = new AirBuilder()
    .WithEcosystem(AirEcosystem.Pony)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(257749)
    .WithVersionId(290640)
    .Build();

var checkpointPriority = new[] { primaryCheckpoint, fallbackCheckpoint, secondaryFallbackCheckpoint };

var fallbackSubmissionResult = await submitter.SubmitWithFallbackAsync(
    checkpointPriority,
    "masterpiece, high quality, detailed landscape",
    width: 1024,
    height: 768,
    cancellationToken);

if (fallbackSubmissionResult is Result<JobStatusCollection>.Success submissionSuccess)
{
    Console.WriteLine($"Job submitted successfully: {submissionSuccess.Data.Token}");
}
else
{
    Console.WriteLine($"Job submission failed: {fallbackSubmissionResult.ErrorOrDefault?.Message}");
}
```

### Use Case 5: Cached Coverage Checker

**Scenario**: High-frequency coverage checks with intelligent caching.

**When to use**: Applications that check coverage frequently (e.g., UI showing available models).

```csharp
public sealed class CachedCoverageChecker(ISdkClient sdkClient)
{
    private readonly record struct CacheEntry(
        ProviderAssetAvailability Data,
        DateTime Timestamp);
    
    private readonly Dictionary<AirIdentifier, CacheEntry> _cache = [];
    private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public async Task<Result<ProviderAssetAvailability>> GetWithCacheAsync(
        AirIdentifier model,
        CancellationToken cancellationToken = default)
    {
        // Check cache first
        if (_cache.TryGetValue(model, out var cached))
        {
            if (DateTime.UtcNow - cached.Timestamp < _cacheLifetime)
            {
                return new Result<ProviderAssetAvailability>.Success(cached.Data);
            }
        }
        
        // Fetch fresh data
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring lock
            if (_cache.TryGetValue(model, out cached) &&
                DateTime.UtcNow - cached.Timestamp < _cacheLifetime)
            {
                return new Result<ProviderAssetAvailability>.Success(cached.Data);
            }
            
            var result = await sdkClient.Coverage.GetAsync(model, cancellationToken);
            
            if (result is Result<ProviderAssetAvailability>.Success success)
            {
                _cache[model] = new CacheEntry(success.Data, DateTime.UtcNow);
            }
            
            return result;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>> GetBatchWithCacheAsync(
        IEnumerable<AirIdentifier> models,
        CancellationToken cancellationToken = default)
    {
        var modelList = models.ToList();
        var results = new Dictionary<AirIdentifier, ProviderAssetAvailability>();
        var toFetch = new List<AirIdentifier>();
        
        // Check cache
        foreach (var model in modelList)
        {
            if (_cache.TryGetValue(model, out var cached) &&
                DateTime.UtcNow - cached.Timestamp < _cacheLifetime)
            {
                results[model] = cached.Data;
            }
            else
            {
                toFetch.Add(model);
            }
        }
        
        // Fetch uncached models
        if (toFetch.Count > 0)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var fetchResult = await sdkClient.Coverage.GetAsync(toFetch, cancellationToken);
                
                if (fetchResult is Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success success)
                {
                    foreach (var (model, availability) in success.Data)
                    {
                        _cache[model] = new CacheEntry(availability, DateTime.UtcNow);
                        results[model] = availability;
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        return results;
    }
    
    public void ClearCache() => _cache.Clear();
    
    public void ClearCache(AirIdentifier model) => _cache.Remove(model);
}

// Full Example
var cachedChecker = new CachedCoverageChecker(sdkClient);

var sdxlCheckpointToCache = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(4201)
    .WithVersionId(130072)
    .Build();

// First call - fetches from API
var firstCheckResult = await cachedChecker.GetWithCacheAsync(sdxlCheckpointToCache, cancellationToken);
Console.WriteLine($"First check: {firstCheckResult.ValueOrDefault?.Availability} (from API)");

// Second call within 5 minutes - uses cache
var secondCheckResult = await cachedChecker.GetWithCacheAsync(sdxlCheckpointToCache, cancellationToken);
Console.WriteLine($"Second check: {secondCheckResult.ValueOrDefault?.Availability} (from cache)");

// Batch check with partial cache hits
var sdxlCheckpointForBatch = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(4201)
    .WithVersionId(130072)
    .Build();

var detailLoraForBatch = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Lora)
    .WithModelId(123456)
    .WithVersionId(789012)
    .Build();

var styleLoraForBatch = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Lora)
    .WithModelId(234567)
    .WithVersionId(890123)
    .Build();

var modelsToCheck = new[] { sdxlCheckpointForBatch, detailLoraForBatch, styleLoraForBatch };

var batchCoverageResults = await cachedChecker.GetBatchWithCacheAsync(modelsToCheck, cancellationToken);
foreach (var (model, availability) in batchCoverageResults)
{
    Console.WriteLine($"{model.AssetType}: {availability.Availability} ({availability.Workers} workers)");
}
```

## Practical Examples

### Pre-flight Check Before Job Submission

```csharp
public async Task<Result<JobStatusCollection>> GenerateWithValidationAsync(
    AirIdentifier checkpointModel,
    string promptText)
{
    // Check availability first
    var coverageResult = await sdkClient.Coverage.GetAsync(checkpointModel);
    
    if (coverageResult is not Result<ProviderAssetAvailability>.Success coverageSuccess)
    {
        return Result<JobStatusCollection>.FromError(
            "Failed to check model availability",
            coverageResult.Error);
    }
    
    if (coverageSuccess.Data.Availability != AvailabilityStatus.Available)
    {
        return Result<JobStatusCollection>.FromApiError(
            "Model not available on generation infrastructure");
    }
    
    // Model is available, proceed with job submission
    return await sdkClient.Jobs
        .CreateImage()
        .WithAir(checkpointModel)
        .WithPositivePrompt(promptText)
        .WithDimensions(1024, 1024)
        .ExecuteAsync();
}
```

### Check All Resources Before Complex Job

```csharp
public async Task<bool> ValidateJobResourcesAsync(
    AirIdentifier baseCheckpoint,
    IEnumerable<AirIdentifier> loraModels)
{
    // Combine all resources
    var allResources = loraModels.Prepend(baseCheckpoint).ToArray();
    
    // Check coverage
    var result = await sdkClient.Coverage.GetAsync(allResources);
    
    if (result is not Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success success)
    {
        Console.WriteLine("Failed to check coverage");
        return false;
    }
    
    // Verify all resources are available
    var unavailable = success.Data
        .Where(kvp => kvp.Value.Availability != AvailabilityStatus.Available)
        .Select(kvp => kvp.Key)
        .ToArray();
    
    if (unavailable.Length > 0)
    {
        Console.WriteLine("Unavailable resources:");
        foreach (var resource in unavailable)
        {
            Console.WriteLine($"  - {resource}");
        }
        return false;
    }
    
    return true;
}
```

### Select Provider Based on Queue Depth

```csharp
public async Task<bool> CheckModelAvailabilityAsync(AirIdentifier model)
{
    var result = await sdkClient.Coverage.GetAsync(model);
    
    if (result is not Result<ProviderAssetAvailability>.Success success)
    {
        return false;
    }
    
    // Check availability and worker count
    if (success.Data.Availability == AvailabilityStatus.Available && success.Data.Workers > 0)
    {
        Console.WriteLine($"Model available with {success.Data.Workers} workers");
        return true;
    }
    else if (success.Data.Availability == AvailabilityStatus.Degraded)
    {
        Console.WriteLine($"Model available but degraded ({success.Data.Workers} workers)");
        return true;
    }
    
    Console.WriteLine("Model not available");
    return false;
}
```

## Error Handling

Handle coverage check failures gracefully:

```csharp
var result = await sdkClient.Coverage.GetAsync(model);

switch (result)
{
    case Result<ProviderAssetAvailability>.Success success:
        if (success.Data.Availability == AvailabilityStatus.Available)
        {
            Console.WriteLine($"Model is available ({success.Data.Workers} workers)");
        }
        else
        {
            Console.WriteLine($"Model status: {success.Data.Availability}");
        }
        break;
        
    case Result<ProviderAssetAvailability>.ApiError apiError:
        Console.WriteLine($"API Error: {apiError.Message}");
        // Proceed anyway - coverage check is optional
        break;
        
    case Result<ProviderAssetAvailability>.NetworkError networkError:
        Console.WriteLine($"Network Error: {networkError.Exception.Message}");
        // Retry or proceed with caution
        break;
}
```

## Best Practices

### 1. Cache Coverage Results

Coverage rarely changes rapidly - cache results to reduce API calls:

```csharp
private readonly Dictionary<AirIdentifier, (DateTime Checked, bool Available)> _coverageCache = new();
private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

public async Task<bool> IsCachedAvailableAsync(AirIdentifier model)
{
    if (_coverageCache.TryGetValue(model, out var cached))
    {
        if (DateTime.UtcNow - cached.Checked < _cacheDuration)
        {
            return cached.Available;
        }
    }
    
    var result = await sdkClient.Coverage.GetAsync(model);
    
    if (result is Result<ProviderAssetAvailability>.Success success)
    {
        var isAvailable = success.Data.Availability == AvailabilityStatus.Available;
        _coverageCache[model] = (DateTime.UtcNow, isAvailable);
        return isAvailable;
    }
    
    return false;
}
```

### 2. Batch Checks When Possible

Check multiple resources in one call:

```csharp
// Good - single API call
var allResources = new[] { baseModel }.Concat(loras);
await sdkClient.Coverage.GetAsync(allResources);

// Less efficient - multiple API calls
foreach (var resource in allResources)
{
    await sdkClient.Coverage.GetAsync(resource);
}
```

### 3. Make Coverage Optional

Coverage checks add latency - make them optional based on context:

```csharp
public async Task<Result<JobStatusCollection>> GenerateAsync(
    AirIdentifier model,
    string prompt,
    bool validateCoverage = false)
{
    if (validateCoverage)
    {
        var coverageResult = await sdkClient.Coverage.GetAsync(model);
        if (coverageResult is Result<ProviderAssetAvailability>.Success success &&
            success.Data.Availability != AvailabilityStatus.Available)
        {
            return Result<JobStatusCollection>.FromApiError("Model not available");
        }
    }
    
    return await sdkClient.Jobs
        .CreateImage()
        .WithAir(model)
        .WithPositivePrompt(prompt)
        .ExecuteAsync();
}
```

### 4. Use for Resource Discovery

Identify which resources are consistently available:

```csharp
public async Task<IEnumerable<AirIdentifier>> GetAvailableModelsAsync(
    IEnumerable<AirIdentifier> candidates)
{
    var result = await sdkClient.Coverage.GetAsync(candidates);
    
    if (result is not Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success success)
    {
        return Array.Empty<AirIdentifier>();
    }
    
    return success.Data
        .Where(kvp => kvp.Value.Availability == AvailabilityStatus.Available)
        .Select(kvp => kvp.Key)
        .ToArray();
}
```

## API Reference

### Methods

| Method | Parameters | Returns | Description |
|--------|------------|---------|-------------|
| `GetAsync` | `IEnumerable<AirIdentifier> models, CancellationToken` | `Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>` | Check availability of multiple models |
| `GetAsync` | `AirIdentifier model, CancellationToken` | `Result<ProviderAssetAvailability>` | Check availability of a single model |

## Next Steps

- [Jobs Service](sdk-jobs.md) - Submit jobs with validated resources
- [Usage Service](sdk-usage.md) - Monitor API consumption
- [AIR Identifiers](air-identifier.md) - Learn about model identifiers
- [Error Handling](error-handling.md) - Comprehensive error handling patterns
