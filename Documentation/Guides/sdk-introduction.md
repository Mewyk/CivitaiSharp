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

Register the SDK client using dependency injection:

```csharp
services.AddCivitaiSdk(options =>
{
    options.ApiToken = "your-api-token";  // Required - SDK cannot operate without a token
});
```

### Basic Usage

```csharp
public class ImageGenerationService(ICivitaiSdkClient sdkClient)
{
    public async Task GenerateImageAsync()
    {
        var request = new TextToImageJobRequest
        {
            Model = "urn:air:sd1:checkpoint:civitai:4201@130072",
            Params = new ImageJobParams
            {
                Prompt = "a beautiful sunset over mountains",
                NegativePrompt = "blurry, low quality",
                Width = 512,
                Height = 512,
                Steps = 20,
                CfgScale = 7.0
            }
        };

        var result = await sdkClient.Jobs.SubmitAsync(request);

        if (result is Result<JobStatusCollection>.Success success)
        {
            Console.WriteLine($"Job submitted: {success.Data.Token}");
        }
    }
}
```

## Services

### Jobs Service

Submit and manage image generation jobs:

- `SubmitAsync` - Submit a text-to-image generation job
- `SubmitBatchAsync` - Submit multiple jobs as a batch
- `GetByIdAsync` - Get job status by ID
- `GetByTokenAsync` - Get job status by token
- `QueryAsync` - Query multiple jobs with filters
- `CancelByIdAsync` - Cancel a job by ID
- `CancelByTokenAsync` - Cancel jobs by batch token
- `TaintByIdAsync` - Mark a job as tainted by ID
- `TaintByTokenAsync` - Mark jobs as tainted by batch token

### Coverage Service

Check resource availability across providers:

- `GetAsync` - Get coverage information for models and resources

### Usage Service

Monitor API consumption:

- `GetConsumptionAsync` - Get account consumption statistics for a specified period

## Next Steps

- [Configuration](configuration.md) - Configure SDK client options
- [Quick Start](quick-start.md) - Step-by-step guide to your first image generation
