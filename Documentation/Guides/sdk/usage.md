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

### ConsumptionDetails

The main result type containing consumption statistics:

| Property | Type | Description |
|----------|------|-------------|
| `Images` | `int?` | Total number of images generated in the period |
| `TotalCost` | `decimal?` | Total Buzz spent in the period |
| `StartDate` | `DateTime?` | Start of the reporting period (UTC) |
| `EndDate` | `DateTime?` | End of the reporting period (UTC) |

## Common Use Cases

[View the full SDK coverage documentation for detailed use cases including Simple Consumption Check, Budget Tracking, Cost Analysis and Reporting, Rate Limiting and Throttling, and Multi-Project Cost Tracking examples]

## Practical Examples

### Monitor Daily Usage

[!code-csharp[Program.cs](examples/Usage/Program.cs#MonitorDailyUsage)]

### Track Monthly Trends

[!code-csharp[Program.cs](examples/Usage/Program.cs#TrackMonthlyTrends)]

### Calculate Average Cost Per Job

[!code-csharp[Program.cs](examples/Usage/Program.cs#CalculateAverageCostPerJob)]

### Budget Monitoring

[!code-csharp[Program.cs](examples/Usage/Program.cs#BudgetMonitoring)]

### Usage Summary Report

[!code-csharp[Program.cs](examples/Usage/Program.cs#UsageSummaryReport)]

### Rate Limiting Protection

[!code-csharp[Program.cs](examples/Usage/Program.cs#RateLimitingProtection)]

## Error Handling

Handle usage query failures gracefully:

[!code-csharp[Program.cs](examples/Usage/Program.cs#ErrorHandling)]

## Best Practices

### 1. Cache Usage Data

Usage changes slowly - cache results to reduce API calls:

[!code-csharp[Program.cs](examples/Usage/Program.cs#CacheUsageData)]

### 2. Use UTC for Date Ranges

Always use UTC dates to avoid timezone confusion:

[!code-csharp[Program.cs](examples/Usage/Program.cs#UseUtcForDateRanges)]

### 3. Separate Monitoring from Business Logic

Keep usage monitoring decoupled from core functionality:

[!code-csharp[Program.cs](examples/Usage/Program.cs#SeparateMonitoringFromBusinessLogic)]

### 4. Set Up Usage Alerts

Implement proactive alerting:

[!code-csharp[Program.cs](examples/Usage/Program.cs#SetUpUsageAlerts)]

## API Reference

### Methods

| Method | Parameters | Returns | Description |
|--------|------------|---------|-------------|
| `GetConsumptionAsync` | `DateTime? startDate, DateTime? endDate, CancellationToken` | `Result<ConsumptionDetails>` | Get consumption statistics for specified period (defaults to all-time if dates not provided) |

### Notes

- All dates should be in UTC
- If `startDate` is null, uses beginning of time
- If `endDate` is null, uses current time
- Results may be cached by the API for a few minutes

## Next Steps

- [Jobs Service](jobs.md) - Submit and manage generation jobs
- [Coverage Service](coverage.md) - Check resource availability
- [SDK Introduction](introduction.md) - Overview of all SDK services
- [Error Handling](../core/error-handling.md) - Comprehensive error handling patterns
