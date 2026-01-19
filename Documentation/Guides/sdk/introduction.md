---
title: CivitaiSharp.Sdk Introduction
description: Learn about CivitaiSharp.Sdk, a high-level .NET client for the Civitai Generator API with image generation, job management, and usage tracking.
---

# CivitaiSharp.Sdk

CivitaiSharp.Sdk provides a high-level client for the [Civitai Generator API](https://developer.civitai.com/docs/api/generator) (orchestration endpoints). It simplifies image generation workflows with strongly-typed models and async job management.

## Key Features

- **Image Generation** - Submit text-to-image jobs with flexible parameters
- **Job Management** - Query, cancel, and retrieve job results
- **Coverage Service** - Check model and resource availability
- **Usage Tracking** - Monitor API consumption and credits
- **Typed Models** - Strongly-typed request and response models

## Getting Started

### Installation

```bash
dotnet add package CivitaiSharp.Sdk --prerelease
```

### Registration

> [!IMPORTANT]
> Unlike CivitaiSharp.Core which can access public endpoints anonymously, the SDK **always requires authentication**. All Generator API operations require a valid API token.

> [!TIP]
> **Use automatic configuration** - it's the cleanest and most secure approach. Place your API token in `appsettings.json` under the `CivitaiSdk` section, then register with one line:

[!code-csharp[Program.cs](../examples/Common/Program.cs#SdkBasicSetup)]

See [Configuration](configuration.md) for complete setup including secure token storage.

### Basic Usage

For complete working examples, see the [Jobs Service](jobs.md) guide which demonstrates all job creation and management features.

## Services

### Jobs Operations

Create and manage image generation jobs using fluent builders:

**Creating Jobs:**
- `CreateImage()` - Returns an `ImageGenerationBuilder` for configuring and submitting image generation jobs

**Querying Jobs:**
- `Query` - Returns a cached `JobQueryBuilder` for fluent job queries
  - `WithDetailed()` - Include original job specifications in response
  - `WithWait()` - Block until jobs complete (up to ~10 minutes)
  - `WhereProperty(key, value)` - Filter by custom properties
  - `GetByIdAsync(Guid id)` - Get job status by ID
  - `GetByTokenAsync(string token)` - Get job status by token
  - `ExecuteAsync()` - Query jobs by custom properties

**Job Management:**
- `Query.CancelAsync(Guid id)` - Cancel a specific job by ID
- `Query.CancelAsync(string token)` - Cancel all jobs in a batch by token
- `Query.TaintAsync(Guid id)` - Mark a job as tainted by ID
- `Query.TaintAsync(string token)` - Mark all jobs in a batch as tainted by token

See the [Jobs Service](jobs.md) guide for complete querying examples.

### Coverage Service

Check resource availability across providers:

- `GetAsync` - Get coverage information for models and resources

### Usage Service

Monitor API consumption:

- `GetConsumptionAsync` - Get account consumption statistics for a specified period

## Next Steps

- [Jobs Service](jobs.md) - Comprehensive guide to creating and querying jobs
- [Coverage Service](coverage.md) - Check model and resource availability
- [Usage Service](usage.md) - Monitor API consumption and credits
- [Configuration](../core/configuration.md) - Configure SDK client options
- [Quick Start](../core/quick-start.md) - Step-by-step guide to your first image generation
- [AIR Identifiers](air-identifier.md) - Learn about model resource identifiers
