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

[!code-csharp[Program.cs](AirBuilder/Program.cs#basic-usage)]

## Builder Methods

### WithEcosystem

Sets the model ecosystem (required):

[!code-csharp[Program.cs](AirBuilder/Program.cs#with-ecosystem)]

Available ecosystems:
- `StableDiffusion1` - Stable Diffusion 1.x (sd1)
- `StableDiffusion2` - Stable Diffusion 2.x (sd2)
- `StableDiffusionXl` - Stable Diffusion XL (sdxl)
- `Flux1` - FLUX.1 (flux1)
- `Pony` - Pony Diffusion (pony)

### WithAssetType

Sets the asset type (required):

[!code-csharp[Program.cs](AirBuilder/Program.cs#with-asset-type)]

Available asset types:
- `Checkpoint` - Full model checkpoint
- `Lora` - LoRA (Low-Rank Adaptation)
- `Lycoris` - LyCORIS network
- `Vae` - VAE (Variational Autoencoder)
- `Embedding` - Textual Inversion embedding
- `Hypernetwork` - Hypernetwork

### WithSource

Sets the source platform (optional, defaults to `AirSource.Civitai`):

[!code-csharp[Program.cs](AirBuilder/Program.cs#with-source)]

### WithModelId

Sets the model ID (required):

[!code-csharp[Program.cs](AirBuilder/Program.cs#with-model-id)]

### WithVersionId

Sets the version ID (required):

[!code-csharp[Program.cs](AirBuilder/Program.cs#with-version-id)]

### Reset / Reuse

The `AirBuilder` is immutable and thread-safe: each `With*` method returns a new builder instance. There is no instance `Reset()` method. To "reset" or reuse a base configuration, either create a new `AirBuilder()` or keep a reusable base instance and call the fluent methods which return new instances.

[!code-csharp[Program.cs](AirBuilder/Program.cs#reset-reuse)]

### Build

Constructs the `AirIdentifier` (validates all required properties are set):

[!code-csharp[Program.cs](AirBuilder/Program.cs#build)]

## Validation

The builder performs validation at two stages:

### Input Validation

Each property setter validates its input:

[!code-csharp[Program.cs](AirBuilder/Program.cs#input-validation)]

### Build Validation

The `Build()` method ensures all required properties are set:

[!code-csharp[Program.cs](AirBuilder/Program.cs#build-validation)]

## Complete Examples

### Building from Civitai Model

[!code-csharp[Program.cs](AirBuilder/Program.cs#build-from-model)]

### Batch Building

[!code-csharp[Program.cs](AirBuilder/Program.cs#batch-building)]

### Builder with Error Handling

[!code-csharp[Program.cs](AirBuilder/Program.cs#builder-error-handling)]

## Best Practices

### Reuse Builders

Reuse builder instances when creating multiple identifiers:

[!code-csharp[Program.cs](AirBuilder/Program.cs#reuse-builders)]

### Validate Early

Validate input before passing to builder methods:

[!code-csharp[Program.cs](AirBuilder/Program.cs#validate-early)]

### Use Method Chaining

Take advantage of the fluent API for concise code:

[!code-csharp[Program.cs](AirBuilder/Program.cs#method-chaining)]

## Related Resources

- [AIR Identifier Guide](air-identifier.md) - Understanding AIR identifiers
- [CivitaiSharp.Sdk Introduction](sdk-introduction.md) - Working with Civitai's AI orchestration platform
- [Tools Introduction](tools-introduction.md) - Overview of CivitaiSharp.Tools utilities
