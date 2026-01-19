---
title: Create a Job
description: Learn how to create and submit image generation jobs using the CivitaiSharp.Sdk Jobs service.
---

# Create a Job

The Jobs service provides a fluent interface for creating and submitting image generation jobs via `sdkClient.Jobs.CreateImage()`. It supports both text-to-image and image-to-image generation modes.

## Basic Image Generation

Use the `CreateImage()` method to get a fluent builder:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#BasicImageGeneration)]

## Advanced Configuration

Configure additional parameters for more control:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#AdvancedConfiguration)]

## Using Additional Networks (LoRAs)

Add LoRAs and other networks to enhance generation:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#UsingAdditionalNetworks)]

## Using ControlNet

Guide generation with ControlNet:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#UsingControlNet)]

## Batch Job Submission

Submit multiple jobs at once:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#BatchJobSubmission)]

## Complete Parameter Example

Demonstration of all available image generation parameters:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#CompleteParameterExample)]

## Next Steps

- [Query a Job](query-job.md) - Track status and manage submitted jobs
- [Coverage Service](coverage.md) - Check model availability before submitting jobs
