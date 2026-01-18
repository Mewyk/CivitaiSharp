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

[!code-csharp[Program.cs](examples/Jobs/Program.cs#BasicImageGeneration)]

### Advanced Configuration

Configure additional parameters for more control:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#AdvancedConfiguration)]

### Using Additional Networks (LoRAs)

Add LoRAs and other networks to enhance generation:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#UsingAdditionalNetworks)]

### Using ControlNet

Guide generation with ControlNet:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#UsingControlNet)]

### Batch Job Submission

Submit multiple jobs at once:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#BatchJobSubmission)]

### Complete Parameter Example

Demonstration of all available image generation parameters:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#CompleteParameterExample)]

## Next Steps

- [Coverage Service](coverage.md) - Check model availability before submitting jobs
- [Usage Service](usage.md) - Monitor API consumption and credits
