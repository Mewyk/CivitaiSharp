---
title: AIR Builder
description: Learn how to use the AirBuilder to create AIR (Artificial Intelligence Resource) identifiers with a fluent API.
---

# AIR Builder

The `AirBuilder` class provides a fluent, validated approach to constructing AIR (Artificial Intelligence Resource) identifiers for Civitai models and assets. It ensures all required properties are set and validates input before building the identifier.

## Overview

AIR identifiers uniquely identify AI model assets across different ecosystems and platforms. The format is:

```
urn:air:{ecosystem}:{type}:{source}:{modelId}@{versionId}
```

Example:
```
urn:air:sdxl:lora:civitai:328553@368189
```

## Getting Started

### Installation

The `AirBuilder` is part of CivitaiSharp.Tools:

```bash
dotnet add package CivitaiSharp.Tools --prerelease
```

### Basic Usage

```csharp
using CivitaiSharp.Sdk.Air;

// Build an AIR identifier using the fluent API
var builder = new AirBuilder();

var airId = builder
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Lora)
    .WithModelId(328553)
    .WithVersionId(368189)
    .Build();

Console.WriteLine(airId.ToString());
// Output: urn:air:sdxl:lora:civitai:328553@368189
```

## Builder Methods

### WithEcosystem

Sets the model ecosystem (required):

```csharp
builder.WithEcosystem(AirEcosystem.StableDiffusionXl);
builder.WithEcosystem(AirEcosystem.Flux1);
builder.WithEcosystem(AirEcosystem.Pony);
```

Available ecosystems:
- `StableDiffusion1` - Stable Diffusion 1.x (sd1)
- `StableDiffusion2` - Stable Diffusion 2.x (sd2)
- `StableDiffusionXl` - Stable Diffusion XL (sdxl)
- `Flux1` - FLUX.1 (flux1)
- `Pony` - Pony Diffusion (pony)

### WithAssetType

Sets the asset type (required):

```csharp
builder.WithAssetType(AirAssetType.Lora);
builder.WithAssetType(AirAssetType.Checkpoint);
builder.WithAssetType(AirAssetType.Vae);
```

Available asset types:
- `Checkpoint` - Full model checkpoint
- `Lora` - LoRA (Low-Rank Adaptation)
- `Lycoris` - LyCORIS network
- `Vae` - VAE (Variational Autoencoder)
- `Embedding` - Textual Inversion embedding
- `Hypernetwork` - Hypernetwork

### WithSource

Sets the source platform (optional, defaults to `AirSource.Civitai`):

```csharp
// Explicitly set source (usually not needed)
builder.WithSource(AirSource.Civitai);

// Source defaults to AirSource.Civitai if not set
```

### WithModelId

Sets the model ID (required):

```csharp
builder.WithModelId(328553);

// Must be greater than 0
```

### WithVersionId

Sets the version ID (required):

```csharp
builder.WithVersionId(368189);

// Must be greater than 0
```

### Reset

Clears all properties to start building a new identifier:

```csharp
var builder = new AirBuilder()
    .WithEcosystem(AirEcosystem.Flux1)
    .WithAssetType(AirAssetType.Lora)
    .WithModelId(123)
    .WithVersionId(456);

// Reset to reuse the builder
builder.Reset()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(789)
    .WithVersionId(101);
```

### Build

Constructs the `AirIdentifier` (validates all required properties are set):

```csharp
var airId = builder.Build();

// Throws InvalidOperationException if:
// - Ecosystem is not set
// - AssetType is not set
// - ModelId is not set
// - VersionId is not set
```

## Validation

The builder performs validation at two stages:

### Input Validation

Each property setter validates its input:

```csharp
// ModelId must be > 0
builder.WithModelId(0); // Throws ArgumentOutOfRangeException

// VersionId must be > 0
builder.WithVersionId(-1); // Throws ArgumentOutOfRangeException

```

### Build Validation

The `Build()` method ensures all required properties are set:

```csharp
var builder = new AirBuilder()
    .WithEcosystem(AirEcosystem.Flux1)
    .WithModelId(123);

// Missing AssetType and VersionId
var airId = builder.Build(); // Throws InvalidOperationException
```

## Complete Examples

### Building from Civitai Model

```csharp
using CivitaiSharp.Core;
using CivitaiSharp.Sdk.Air;

public class ModelService(IApiClient apiClient)
{
    public async Task<AirIdentifier?> GetAirIdAsync(int modelId)
    {
        // Fetch model from Civitai
        var result = await apiClient.Models.GetByIdAsync(modelId);
        if (result is not Result<Model>.Success success)
            return null;
        
        var model = success.Data;
        var version = model.ModelVersions?.FirstOrDefault();
        
        if (version is null)
            return null;
        
        // Build AIR identifier
        var builder = new AirBuilder();
        return builder
            .WithEcosystem(GetEcosystem(version.BaseModel))
            .WithAssetType(GetAssetType(model.Type))
            .WithModelId(model.Id)
            .WithVersionId(version.Id)
            .Build();
    }
    
    private AirEcosystem GetEcosystem(string baseModel) => baseModel switch
    {
        "SD 1.5" => AirEcosystem.StableDiffusion1,
        "SDXL 1.0" => AirEcosystem.StableDiffusionXl,
        "Flux.1" => AirEcosystem.Flux1,
        "Pony" => AirEcosystem.Pony,
        _ => AirEcosystem.StableDiffusion1
    };
    
    private AirAssetType GetAssetType(ModelType type) => type switch
    {
        ModelType.Checkpoint => AirAssetType.Checkpoint,
        ModelType.Lora => AirAssetType.Lora,
        ModelType.Vae => AirAssetType.Vae,
        ModelType.TextualInversion => AirAssetType.Embedding,
        ModelType.Hypernetwork => AirAssetType.Hypernetwork,
        _ => AirAssetType.Checkpoint
    };
}
```

### Batch Building

```csharp
using CivitaiSharp.Tools.Air;
using CivitaiSharp.Sdk.Air;

public class BatchProcessor
{
    public List<AirIdentifier> BuildMultipleIdentifiers()
    {
        var builder = new AirBuilder();
        var identifiers = new List<AirIdentifier>();
        
        // Build multiple identifiers efficiently
        foreach (var (ecosystem, assetType, modelId, versionId) in GetModelData())
        {
            var airId = builder
                .WithEcosystem(ecosystem)
                .WithAssetType(assetType)
                .WithModelId(modelId)
                .WithVersionId(versionId)
                .Build();
            
            identifiers.Add(airId);
            
        }
        
        return identifiers;
    }
    
    private IEnumerable<(AirEcosystem, AirAssetType, long, long)> GetModelData()
    {
        yield return (AirEcosystem.StableDiffusionXl, AirAssetType.Lora, 328553, 368189);
        yield return (AirEcosystem.Flux1, AirAssetType.Checkpoint, 123456, 789012);
        yield return (AirEcosystem.Pony, AirAssetType.Lora, 111111, 222222);
    }
}
```

### Builder with Error Handling

```csharp
using CivitaiSharp.Tools.Air;
using CivitaiSharp.Sdk.Air;

public AirIdentifier? TryBuildAirId(
    AirEcosystem ecosystem,
    AirAssetType assetType,
    long modelId,
    long versionId)
{
    try
    {
        var builder = new AirBuilder();
        return builder
            .WithEcosystem(ecosystem)
            .WithAssetType(assetType)
            .WithModelId(modelId)
            .WithVersionId(versionId)
            .Build();
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"Invalid ID: {ex.Message}");
        return null;
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Missing required property: {ex.Message}");
        return null;
    }
}
```

## Best Practices

### Reuse Builders

Reuse builder instances when creating multiple identifiers:

```csharp
// Good - reuses builder
var builder = new AirBuilder();
foreach (var data in modelData)
{
    var airId = builder
        .WithEcosystem(data.Ecosystem)
        .WithAssetType(data.AssetType)
        .WithModelId(data.ModelId)
        .WithVersionId(data.VersionId)
        .Build();
    
    ProcessAirId(airId);
    builder.Reset();
}

// Avoid - creates new builder each time
foreach (var data in modelData)
{
    var builder = new AirBuilder();
    var airId = builder.WithEcosystem(data.Ecosystem)...Build();
}
```

### Validate Early

Validate input before passing to builder methods:

```csharp
public AirIdentifier BuildFromUserInput(long modelId, long versionId)
{
    // Validate before building
    if (modelId <= 0)
        throw new ArgumentException("Model ID must be positive", nameof(modelId));
    
    if (versionId <= 0)
        throw new ArgumentException("Version ID must be positive", nameof(versionId));
    
    return new AirBuilder()
        .WithEcosystem(AirEcosystem.StableDiffusionXl)
        .WithAssetType(AirAssetType.Lora)
        .WithModelId(modelId)
        .WithVersionId(versionId)
        .Build();
}
```

### Use Method Chaining

Take advantage of the fluent API for concise code:

```csharp
// Preferred - fluent style
var airId = new AirBuilder()
    .WithEcosystem(AirEcosystem.Flux1)
    .WithAssetType(AirAssetType.Lora)
    .WithModelId(123)
    .WithVersionId(456)
    .Build();

// Avoid - verbose style
var builder = new AirBuilder();
builder.WithEcosystem(AirEcosystem.Flux1);
builder.WithAssetType(AirAssetType.Lora);
builder.WithModelId(123);
builder.WithVersionId(456);
var airId = builder.Build();
```

## Related Resources

- [AIR Identifier Guide](air-identifier.md) - Understanding AIR identifiers
- [CivitaiSharp.Sdk Introduction](sdk-introduction.md) - Working with Civitai's AI orchestration platform
- [Tools Introduction](tools-introduction.md) - Overview of CivitaiSharp.Tools utilities
