---
title: Create a Job
description: Submit image generation jobs.
---

# Create a Job

Submit image generation jobs via `sdkClient.Jobs.CreateImage()`.

## Basic Generation

[!code-csharp[Program.cs](examples/Jobs/Program.cs#BasicImageGeneration)]

## Advanced Configuration

[!code-csharp[Program.cs](examples/Jobs/Program.cs#AdvancedConfiguration)]

## Additional Networks (LoRAs)

[!code-csharp[Program.cs](examples/Jobs/Program.cs#UsingAdditionalNetworks)]

## ControlNet

[!code-csharp[Program.cs](examples/Jobs/Program.cs#UsingControlNet)]

## Batch Submission

[!code-csharp[Program.cs](examples/Jobs/Program.cs#BatchJobSubmission)]

## Complete Example

[!code-csharp[Program.cs](examples/Jobs/Program.cs#CompleteParameterExample)]

## Next Steps

- [Query a Job](query-job.md)
- [Coverage Service](coverage.md)
