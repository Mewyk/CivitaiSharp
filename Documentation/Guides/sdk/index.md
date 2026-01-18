# SDK Library

The **CivitaiSharp.Sdk** library extends the Core library with advanced features, utilities, and specialized functionality for working with Civitai resources. The primary focus is the **AIR (AI Resource)** system for standardized resource identification.

## Overview

The SDK library provides tools for working with AIR identifiers, which are URN-based identifiers that uniquely identify AI resources across different platforms and ecosystems. AIR makes it easy to reference models, versions, and other resources in a standardized format.

## Key Features

- **AIR Identifiers**: Parse and construct standardized AI resource identifiers
- **AIR Builder**: Fluent API for creating AIR identifiers
- **Resource Identification**: Unique identification across platforms
- **Ecosystem Support**: SDXL, Flux, Pony, and more
- **Version Tracking**: Link models to specific versions

## AIR Format

AIR identifiers follow this format:

```
urn:air:{ecosystem}:{type}:{source}:{id}@{version}
```

Example:
```
urn:air:sdxl:lora:civitai:328553@368189
```

## Quick Links

- [Introduction](introduction.md) - Getting started with the SDK
- [AIR Identifier](air-identifier.md) - Parsing and using AIR identifiers
- [AIR Builder](air-builder.md) - Creating AIR identifiers programmatically
- [Jobs](jobs.md) - Background job management
- [Coverage](coverage.md) - API coverage and supported features
- [Usage](usage.md) - Common usage patterns and examples

## Installation

```bash
dotnet add package CivitaiSharp.Sdk
```

Note: The SDK package includes the Core library as a dependency.

## Basic Example

```csharp
using CivitaiSharp.Sdk.Air;

// Parse an existing AIR identifier
var air = AirIdentifier.Parse("urn:air:sdxl:lora:civitai:328553@368189");

Console.WriteLine($"Ecosystem: {air.Ecosystem}");  // StableDiffusionXl
Console.WriteLine($"Type: {air.AssetType}");       // Lora
Console.WriteLine($"Source: {air.Source}");        // Civitai
Console.WriteLine($"Model ID: {air.ModelId}");     // 328553
Console.WriteLine($"Version ID: {air.VersionId}"); // 368189

// Build a new AIR identifier
var builder = new AirBuilder();
var newAir = builder
    .WithEcosystem(AirEcosystem.Flux1)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithModelId(12345)
    .WithVersionId(67890)
    .Build();

Console.WriteLine(newAir.ToString());
// Output: urn:air:flux1:checkpoint:civitai:12345@67890
```

## Next Steps

- Start with [Introduction](introduction.md) to understand AIR concepts
- Learn [AIR Identifier](air-identifier.md) for parsing and validation
- Use [AIR Builder](air-builder.md) to create identifiers programmatically
