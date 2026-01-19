---
title: Query a Job
description: Learn how to track, manage, and cancel image generation jobs using the CivitaiSharp.Sdk Jobs service.
---

# Query a Job

The Jobs service provides a fluent interface for querying and managing jobs via `sdkClient.Jobs.Query`.

## Querying Jobs

### Get Job by ID

Query a specific job by its unique identifier:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#GetJobById)]

### Get Jobs by Token

Query all jobs in a batch using the batch token:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#GetJobsByToken)]

### Query with Options

Use the fluent query methods for advanced behaviors:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#QueryWithOptions)]

### Filter by Custom Properties

Query jobs by custom properties:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#FilterByProperties)]

## Managing Jobs

### Cancel a Job

Cancel a specific job by ID:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#CancelJob)]

### Cancel Batch Jobs

Cancel all jobs in a batch:

[!code-csharp[Program.cs](examples/Jobs/Program.cs#CancelBatchJobs)]

### Taint a Job

Mark a job as tainted (indicates problematic output):

[!code-csharp[Program.cs](examples/Jobs/Program.cs#TaintJob)]

## Next Steps

- [Create a Job](create-job.md) - Submit new image generation jobs
- [Usage Service](usage.md) - Monitor API consumption and credits
