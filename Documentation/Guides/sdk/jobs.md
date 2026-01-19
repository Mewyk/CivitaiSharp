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

## Querying Jobs

### Get Job by ID

Query a specific job by its unique identifier:

```csharp
var jobId = Guid.Parse("...");
var result = await sdkClient.Jobs.Query.GetByIdAsync(jobId);

if (result is Result<JobStatus>.Success success)
{
    Console.WriteLine($"Status: {success.Data.Status}");
    Console.WriteLine($"Job ID: {success.Data.JobId}");
}
```

### Get Jobs by Token

Query all jobs in a batch using the batch token:

```csharp
var batchToken = "...";
var result = await sdkClient.Jobs.Query.GetByTokenAsync(batchToken);

if (result is Result<JobStatusCollection>.Success success)
{
    foreach (var job in success.Data.JobsList)
    {
        Console.WriteLine($"Job {job.JobId}: {job.Status}");
    }
}
```

### Query with Options

Use the fluent query builder for advanced queries:

```csharp
var result = await sdkClient.Jobs.Query
    .WithDetailed()  // Include original job specifications
    .WithWait()      // Wait for jobs to complete (up to ~10 minutes)
    .GetByTokenAsync(batchToken);
```

### Filter by Custom Properties

Query jobs by custom properties:

```csharp
var result = await sdkClient.Jobs.Query
    .WhereProperty("userId", "12345")
    .WhereProperty("campaign", "winter-2026")
    .ExecuteAsync();
```

## Managing Jobs

### Cancel a Job

Cancel a specific job by ID:

```csharp
var jobId = Guid.Parse("...");
var result = await sdkClient.Jobs.Query.CancelAsync(jobId);

if (result is Result<Unit>.Success)
{
    Console.WriteLine("Job cancelled successfully");
}
```

### Cancel Batch Jobs

Cancel all jobs in a batch:

```csharp
var batchToken = "...";
var result = await sdkClient.Jobs.Query.CancelAsync(batchToken);
```

### Taint a Job

Mark a job as tainted (indicates problematic output):

```csharp
var jobId = Guid.Parse("...");
await sdkClient.Jobs.Query.TaintAsync(jobId);
```

## Next Steps

- [Coverage Service](coverage.md) - Check model availability before submitting jobs
- [Usage Service](usage.md) - Monitor API consumption and credits
