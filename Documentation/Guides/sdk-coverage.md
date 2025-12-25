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
var model = AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072");

var result = await sdkClient.Coverage.GetAsync(model);

if (result is Result<ProviderAssetAvailability>.Success success)
{
    Console.WriteLine($"Available: {success.Data.Available}");
    foreach (var provider in success.Data.Providers)
    {
        Console.WriteLine($"Provider: {provider.Key}");
        Console.WriteLine($"  Available: {provider.Value.Available}");
        Console.WriteLine($"  Queue Position: {provider.Value.QueuePosition}");
    }
}
```

### Check Multiple Models

```csharp
var models = new[]
{
    AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"),
    AirIdentifier.Parse("urn:air:sdxl:lora:civitai:328553@368189"),
    AirIdentifier.Parse("urn:air:sd1:vae:civitai:22354@123456")
};

var result = await sdkClient.Coverage.GetAsync(models);

if (result is Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success success)
{
    foreach (var (model, availability) in success.Data)
    {
        Console.WriteLine($"{model}: {availability.Available}");
    }
}
```

## Understanding Results

### ProviderAssetAvailability

The main result type containing overall availability and provider-specific details:

| Property | Type | Description |
|----------|------|-------------|
| `Available` | `bool` | Whether the resource is available on any provider |
| `Providers` | `Dictionary<string, ProviderJobStatus>` | Provider-specific availability details |

### ProviderJobStatus

Provider-specific information:

| Property | Type | Description |
|----------|------|-------------|
| `Available` | `bool` | Whether this specific provider has the resource |
| `QueuePosition` | `int?` | Current queue depth (null if not available) |

## Practical Examples

### Pre-flight Check Before Job Submission

```csharp
public async Task<Result<JobStatusCollection>> GenerateWithValidationAsync(
    AirIdentifier model,
    string prompt)
{
    // Check availability first
    var coverageResult = await sdkClient.Coverage.GetAsync(model);
    
    if (coverageResult is not Result<ProviderAssetAvailability>.Success coverageSuccess)
    {
        return Result<JobStatusCollection>.FromError(
            "Failed to check model availability",
            coverageResult.Error);
    }
    
    if (!coverageSuccess.Data.Available)
    {
        return Result<JobStatusCollection>.FromApiError(
            "Model not available on generation infrastructure");
    }
    
    // Model is available, proceed with job submission
    return await sdkClient.Jobs
        .CreateTextToImage()
        .WithModel(model)
        .WithPrompt(prompt)
        .WithSize(1024, 1024)
        .ExecuteAsync();
}
```

### Check All Resources Before Complex Job

```csharp
public async Task<bool> ValidateJobResourcesAsync(
    AirIdentifier baseModel,
    IEnumerable<AirIdentifier> loras)
{
    // Combine all resources
    var allResources = loras.Prepend(baseModel).ToArray();
    
    // Check coverage
    var result = await sdkClient.Coverage.GetAsync(allResources);
    
    if (result is not Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success success)
    {
        Console.WriteLine("Failed to check coverage");
        return false;
    }
    
    // Verify all resources are available
    var unavailable = success.Data
        .Where(kvp => !kvp.Value.Available)
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
public async Task<string?> FindBestProviderAsync(AirIdentifier model)
{
    var result = await sdkClient.Coverage.GetAsync(model);
    
    if (result is not Result<ProviderAssetAvailability>.Success success)
    {
        return null;
    }
    
    // Find provider with shortest queue
    var bestProvider = success.Data.Providers
        .Where(p => p.Value.Available)
        .OrderBy(p => p.Value.QueuePosition ?? int.MaxValue)
        .Select(p => p.Key)
        .FirstOrDefault();
    
    if (bestProvider is not null)
    {
        var queuePos = success.Data.Providers[bestProvider].QueuePosition;
        Console.WriteLine($"Best provider: {bestProvider} (Queue: {queuePos ?? 0})");
    }
    
    return bestProvider;
}
```

## Error Handling

Handle coverage check failures gracefully:

```csharp
var result = await sdkClient.Coverage.GetAsync(model);

switch (result)
{
    case Result<ProviderAssetAvailability>.Success success:
        if (success.Data.Available)
        {
            Console.WriteLine("Model is available");
        }
        else
        {
            Console.WriteLine("Model not available on any provider");
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
        _coverageCache[model] = (DateTime.UtcNow, success.Data.Available);
        return success.Data.Available;
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
        if (coverageResult is Result<ProviderAssetAvailability>.Success { Data.Available: false })
        {
            return Result<JobStatusCollection>.FromApiError("Model not available");
        }
    }
    
    return await sdkClient.Jobs
        .CreateTextToImage()
        .WithModel(model)
        .WithPrompt(prompt)
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
        .Where(kvp => kvp.Value.Available)
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
