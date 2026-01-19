---
title: Usage Service
description: Monitor API consumption, track credits, and analyze account usage with the CivitaiSharp.Sdk Usage service.
---

# Usage Service

The Usage service provides comprehensive monitoring of your Civitai Generator API consumption, including job counts, credit usage, and detailed breakdowns by time period.

## Overview

The Usage service allows you to:
- Track total API consumption over time
- Monitor credit usage and job counts
- Analyze usage patterns by date range
- Plan resource allocation based on historical data

## Basic Usage

### Get Current Consumption

[!code-csharp[Program.cs](examples/Usage/Program.cs#GetCurrentConsumption)]

### Get Consumption for Specific Period

[!code-csharp[Program.cs](examples/Usage/Program.cs#GetConsumptionForSpecificPeriod)]

## Understanding Results

The service returns `ConsumptionDetails` containing images generated, total cost, and period dates. All dates are in UTC.

## Common Use Cases

## Practical Examples

### Budget Monitoring

[!code-csharp[Program.cs](examples/Usage/Program.cs#BudgetMonitoring)]

### Rate Limiting Protection

[!code-csharp[Program.cs](examples/Usage/Program.cs#RateLimitingProtection)]

### Set Up Usage Alerts

[!code-csharp[Program.cs](examples/Usage/Program.cs#SetUpUsageAlerts)]

### Cache Usage Data

[!code-csharp[Program.cs](examples/Usage/Program.cs#CacheUsageData)]

## Error Handling

Handle usage query failures gracefully:

[!code-csharp[Program.cs](examples/Usage/Program.cs#ErrorHandling)]

## Best Practices

1. **Cache usage data** - Usage changes slowly, cache results to reduce API calls
2. **Use UTC dates** - Always use UTC to avoid timezone confusion  
3. **Separate monitoring** - Keep usage monitoring decoupled from core functionality
4. **Set up alerts** - Implement proactive alerting for budget thresholds

## Next Steps

- [Jobs Service](jobs.md) - Submit and manage generation jobs
- [Coverage Service](coverage.md) - Check resource availability
- [SDK Introduction](introduction.md) - Overview of all SDK services
- [Error Handling](../core/error-handling.md) - Comprehensive error handling patterns
