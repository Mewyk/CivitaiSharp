---
title: Usage Service
description: Monitor API consumption and credits.
---

# Usage Service

Monitor Civitai Generator API consumption, credits, and usage patterns.

## Get Current Consumption

[!code-csharp[Program.cs](examples/Usage/Program.cs#GetCurrentConsumption)]

## Get Specific Period

[!code-csharp[Program.cs](examples/Usage/Program.cs#GetConsumptionForSpecificPeriod)]

## Error Handling

[!code-csharp[Program.cs](examples/Usage/Program.cs#ErrorHandling)]

## Best Practices

- Cache usage data
- Use UTC dates
- Separate monitoring from core functionality
- Set up budget alerts

## Next Steps

- [Create a Job](create-job.md)
- [Coverage Service](coverage.md)
- [SDK Introduction](introduction.md)
