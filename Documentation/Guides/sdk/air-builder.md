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

The `AirBuilder` is part of the `CivitaiSharp.Sdk` package.

```bash
dotnet add package CivitaiSharp.Sdk --prerelease
```

### Basic Usage

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#BasicUsage)]

## Builder Methods

### WithEcosystem

Sets the model ecosystem (required):

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#WithEcosystem)]

Available ecosystems:
- `StableDiffusion1` - Stable Diffusion 1.x (sd1)
- `StableDiffusion2` - Stable Diffusion 2.x (sd2)
- `StableDiffusionXl` - Stable Diffusion XL (sdxl)
- `Flux1` - FLUX.1 (flux1)
- `Pony` - Pony Diffusion (pony)

### WithAssetType

Sets the asset type (required):

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#WithAssetType)]

Available asset types:
- `Checkpoint` - Full model checkpoint
- `Lora` - LoRA (Low-Rank Adaptation)
- `Lycoris` - LyCORIS network
- `Vae` - VAE (Variational Autoencoder)
- `Embedding` - Textual Inversion embedding
- `Hypernetwork` - Hypernetwork

### WithSource

Sets the source platform (optional, defaults to `AirSource.Civitai`):

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#WithSource)]

### WithModelId

Sets the model ID (required):

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#WithModelId)]

### WithVersionId

Sets the version ID (required):

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#WithVersionId)]

### Reset / Reuse

The `AirBuilder` is immutable and thread-safe: each `With*` method returns a new builder instance. There is no instance `Reset()` method. To "reset" or reuse a base configuration, either create a new `AirBuilder()` or keep a reusable base instance and call the fluent methods which return new instances.

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#ResetReuse)]

### Build

Constructs the `AirIdentifier` (validates all required properties are set):

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#Build)]

## Validation

The builder performs validation at two stages:

### Input Validation

Each property setter validates its input:

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#InputValidation)]

### Build Validation

The `Build()` method ensures all required properties are set:

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#BuildValidation)]

## Complete Examples

### Building from Civitai Model

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#BuildFromModel)]

### Batch Building

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#BatchBuilding)]

### Builder with Error Handling

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#BuilderErrorHandling)]

## Best Practices

### Reuse Builders

Reuse builder instances when creating multiple identifiers:

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#ReuseBuilders)]

### Validate Early

Validate input before passing to builder methods:

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#ValidateEarly)]

### Use Method Chaining

Take advantage of the fluent API for concise code:

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#MethodChaining)]

## Related Resources

- [AIR Identifier Guide](air-identifier.md) - Understanding AIR identifiers
- [CivitaiSharp.Sdk Introduction](introduction.md) - Working with Civitai's AI orchestration platform
- [Tools Introduction](../tools/introduction.md) - Overview of CivitaiSharp.Tools utilities
