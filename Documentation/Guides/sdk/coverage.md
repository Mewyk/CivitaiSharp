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

[!code-csharp[Program.cs](../examples/Common/Program.cs#CommonAirIdentifiers)]

Once you have an AIR identifier, check its availability:

[!code-csharp[Program.cs](examples/Coverage/Program.cs#CheckSingleModelAvailability)]

### Check Multiple Models

[!code-csharp[Program.cs](examples/Coverage/Program.cs#CheckMultipleModels)]

## Understanding Results

### ProviderAssetAvailability

The main result type containing availability information:

| Property | Type | Description |
|----------|------|-------------|
| `Availability` | `AvailabilityStatus` | The availability status (Available, Unavailable, Degraded) |
| `Workers` | `int` | Number of workers with this model loaded (0 means unavailable) |

### AvailabilityStatus

Enum values for model availability status:

| Value | Description | Meaning |
|-------|-------------|---------|
| `Available` | Model is available and ready for generation | Workers are loaded and ready |
| `Unavailable` | Model is not currently available | No workers have this model loaded |
| `Degraded` | Model is available but with limited capacity | Some workers available, may experience delays |

### Provider

Complete list of infrastructure providers:

| Value | Description | Use Case |
|-------|-------------|----------|
| `Civitai` | Civitai's first-party infrastructure | Primary recommended provider |
| `OctoML` | OctoML cloud provider | High-performance GPU infrastructure |
| `SaladML` | SaladML distributed computing | Cost-effective distributed processing |
| `PicFinder` | PicFinder specialized provider | Specialized image generation infrastructure |
| `RunPods` | RunPods cloud GPU provider | Flexible GPU cloud computing |
| `ValdiAI` | ValdiAI infrastructure | AI-optimized infrastructure |
| `OctoMLNext` | Next-generation OctoML | Enhanced OctoML infrastructure |
| `RunDiffusion` | RunDiffusion specialized provider | Diffusion model specialized infrastructure |
| `SaladShared` | SaladCloud shared resources | Shared distributed computing resources |

### JobSupport

Provider capability levels:

| Value | Description | Action Recommended |
|-------|-------------|-------------------|
| `Unsupported` | Provider does not support this model type | Try different provider or model |
| `Unavailable` | Provider supports but temporarily unavailable | Wait and retry, or use different provider |
| `Available` | Provider supports and ready to process | Safe to submit jobs |

[!code-csharp[Program.cs](examples/Coverage/Program.cs#AvailabilityStatusChecking)]

## Common Use Cases

[View the full guide for detailed use cases including Simple Availability Check, Complex Multi-Resource Validation, Load Balancing with Worker Count, Retry with Fallback Models, and Cached Coverage Checker examples]

## Practical Examples

### Pre-flight Check Before Job Submission

[!code-csharp[Program.cs](examples/Coverage/Program.cs#PreflightCheckBeforeJobSubmission)]

### Check All Resources Before Complex Job

[!code-csharp[Program.cs](examples/Coverage/Program.cs#CheckAllResourcesBeforeComplexJob)]

## Error Handling

Handle coverage check failures gracefully:

[!code-csharp[Program.cs](examples/Coverage/Program.cs#ErrorHandling)]

## Best Practices

1. **Cache coverage results** - Coverage rarely changes rapidly, cache to reduce API calls
2. **Batch checks** - Check multiple resources in one call when possible
3. **Make coverage optional** - Add latency only when needed based on context
4. **Resource discovery** - Identify consistently available resources

## Next Steps

- [Jobs Service](jobs.md) - Submit jobs with validated resources
- [Usage Service](usage.md) - Monitor API consumption
- [AIR Identifiers](air-identifier.md) - Learn about model identifiers
- [Error Handling](../core/error-handling.md) - Comprehensive error handling patterns
