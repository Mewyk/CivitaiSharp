---
title: Coverage Service
description: Check model availability before submitting jobs.
---

# Coverage Service

Check AI model availability across Civitai infrastructure before submitting jobs.

## Check Single Model

[!code-csharp[Program.cs](examples/Coverage/Program.cs#CheckSingleModelAvailability)]

## Check Multiple Models

[!code-csharp[Program.cs](examples/Coverage/Program.cs#CheckMultipleModels)]

## Result Properties

**ProviderAssetAvailability:**
- `Availability` - Status (`Available`, `Unavailable`, `Degraded`)
- `Workers` - Worker count (0 = unavailable)

**AvailabilityStatus:**
- `Available` - Ready for generation
- `Unavailable` - Not available
- `Degraded` - Limited capacity

**JobSupport:**
- `Unsupported` - Provider doesn't support model
- `Unavailable` - Temporarily unavailable
- `Available` - Ready to process

## Availability Checking

[!code-csharp[Program.cs](examples/Coverage/Program.cs#AvailabilityStatusChecking)]

## Error Handling

[!code-csharp[Program.cs](examples/Coverage/Program.cs#ErrorHandling)]

## Best Practices

- Cache coverage results
- Batch checks when possible
- Make coverage checks optional
- Use for resource discovery

## Next Steps

- [Create a Job](create-job.md)
- [Usage Service](usage.md)
- [AIR Identifiers](air-identifier.md)
