---
title: Query a Job
description: Track and manage image generation jobs.
---

# Query a Job

Track and manage jobs via `sdkClient.Jobs.Query`.

## Query by ID

[!code-csharp[Program.cs](examples/Jobs/Program.cs#GetJobById)]

## Query by Token

[!code-csharp[Program.cs](examples/Jobs/Program.cs#GetJobsByToken)]

## Query with Options

[!code-csharp[Program.cs](examples/Jobs/Program.cs#QueryWithOptions)]

## Filter by Properties

[!code-csharp[Program.cs](examples/Jobs/Program.cs#FilterByProperties)]

## Cancel Job

[!code-csharp[Program.cs](examples/Jobs/Program.cs#CancelJob)]

## Cancel Batch

[!code-csharp[Program.cs](examples/Jobs/Program.cs#CancelBatchJobs)]

## Taint Job

[!code-csharp[Program.cs](examples/Jobs/Program.cs#TaintJob)]

## Next Steps

- [Create a Job](create-job.md)
- [Usage Service](usage.md)
