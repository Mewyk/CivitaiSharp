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

```csharp
var result = await sdkClient.Usage.GetConsumptionAsync();

if (result is Result<ConsumptionDetails>.Success success)
{
    Console.WriteLine($"Total Jobs: {success.Data.TotalJobs}");
    Console.WriteLine($"Total Credits: {success.Data.TotalCredits}");
    Console.WriteLine($"Date Range: {success.Data.StartDate} to {success.Data.EndDate}");
}
```

### Get Consumption for Specific Period

```csharp
var startDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var endDate = new DateTime(2025, 1, 31, 23, 59, 59, DateTimeKind.Utc);

var result = await sdkClient.Usage.GetConsumptionAsync(startDate, endDate);

if (result is Result<ConsumptionDetails>.Success success)
{
    Console.WriteLine($"January 2025 Usage:");
    Console.WriteLine($"  Jobs: {success.Data.TotalJobs}");
    Console.WriteLine($"  Credits: {success.Data.TotalCredits}");
}
```

## Understanding Results

### ConsumptionDetails

The main result type containing consumption statistics:

| Property | Type | Description |
|----------|------|-------------|
| `StartDate` | `DateTime` | Start of the reporting period (UTC) |
| `EndDate` | `DateTime` | End of the reporting period (UTC) |
| `TotalJobs` | `int` | Total number of jobs submitted |
| `TotalCredits` | `decimal` | Total credits consumed |
| `JobsByType` | `Dictionary<string, int>?` | Job counts by type (e.g., "textToImage") |
| `CreditsByType` | `Dictionary<string, decimal>?` | Credit usage by job type |

## Practical Examples

### Monitor Daily Usage

```csharp
public async Task<ConsumptionDetails?> GetTodayUsageAsync()
{
    var today = DateTime.UtcNow.Date;
    var tomorrow = today.AddDays(1);
    
    var result = await sdkClient.Usage.GetConsumptionAsync(today, tomorrow);
    
    if (result is Result<ConsumptionDetails>.Success success)
    {
        return success.Data;
    }
    
    Console.WriteLine("Failed to retrieve usage data");
    return null;
}
```

### Track Monthly Trends

```csharp
public async Task ShowMonthlyTrendsAsync()
{
    var currentMonth = DateTime.UtcNow.Date.AddDays(1 - DateTime.UtcNow.Day);
    
    for (int i = 0; i < 6; i++)
    {
        var month = currentMonth.AddMonths(-i);
        var nextMonth = month.AddMonths(1);
        
        var result = await sdkClient.Usage.GetConsumptionAsync(month, nextMonth);
        
        if (result is Result<ConsumptionDetails>.Success success)
        {
            Console.WriteLine($"{month:yyyy-MM}:");
            Console.WriteLine($"  Jobs: {success.Data.TotalJobs,6}");
            Console.WriteLine($"  Credits: {success.Data.TotalCredits,8:F2}");
        }
    }
}
```

### Calculate Average Cost Per Job

```csharp
public async Task<decimal?> GetAverageCostPerJobAsync(DateTime start, DateTime end)
{
    var result = await sdkClient.Usage.GetConsumptionAsync(start, end);
    
    if (result is Result<ConsumptionDetails>.Success success)
    {
        if (success.Data.TotalJobs == 0)
        {
            return null;
        }
        
        var avgCost = success.Data.TotalCredits / success.Data.TotalJobs;
        Console.WriteLine($"Average cost per job: {avgCost:F3} credits");
        return avgCost;
    }
    
    return null;
}
```

### Budget Monitoring

```csharp
public async Task<bool> CheckBudgetAsync(decimal monthlyBudget)
{
    var monthStart = DateTime.UtcNow.Date.AddDays(1 - DateTime.UtcNow.Day);
    var monthEnd = monthStart.AddMonths(1);
    
    var result = await sdkClient.Usage.GetConsumptionAsync(monthStart, monthEnd);
    
    if (result is not Result<ConsumptionDetails>.Success success)
    {
        Console.WriteLine("Failed to check budget");
        return false;
    }
    
    var percentUsed = (success.Data.TotalCredits / monthlyBudget) * 100;
    var remaining = monthlyBudget - success.Data.TotalCredits;
    
    Console.WriteLine($"Budget Status:");
    Console.WriteLine($"  Used: {success.Data.TotalCredits:F2} / {monthlyBudget:F2} credits ({percentUsed:F1}%)");
    Console.WriteLine($"  Remaining: {remaining:F2} credits");
    
    if (percentUsed >= 90)
    {
        Console.WriteLine("WARNING: Over 90% of budget used!");
        return false;
    }
    else if (percentUsed >= 75)
    {
        Console.WriteLine("CAUTION: Over 75% of budget used");
        return false;
    }
    
    return true;
}
```

### Usage by Job Type Analysis

```csharp
public async Task AnalyzeJobTypesAsync(DateTime start, DateTime end)
{
    var result = await sdkClient.Usage.GetConsumptionAsync(start, end);
    
    if (result is not Result<ConsumptionDetails>.Success success)
    {
        return;
    }
    
    if (success.Data.JobsByType is null || success.Data.CreditsByType is null)
    {
        Console.WriteLine("Detailed breakdown not available");
        return;
    }
    
    Console.WriteLine("Usage by Job Type:");
    Console.WriteLine($"{"Type",-20} {"Jobs",10} {"Credits",12} {"Avg Cost",12}");
    Console.WriteLine(new string('-', 60));
    
    foreach (var (jobType, count) in success.Data.JobsByType)
    {
        var credits = success.Data.CreditsByType.GetValueOrDefault(jobType, 0);
        var avgCost = count > 0 ? credits / count : 0;
        
        Console.WriteLine($"{jobType,-20} {count,10} {credits,12:F2} {avgCost,12:F3}");
    }
}
```

### Rate Limiting Protection

```csharp
private DateTime? _lastUsageCheck;
private ConsumptionDetails? _cachedUsage;

public async Task<bool> CanSubmitJobAsync(decimal creditCost)
{
    // Cache usage checks (avoid API spam)
    if (_lastUsageCheck is null || 
        DateTime.UtcNow - _lastUsageCheck.Value > TimeSpan.FromMinutes(5))
    {
        var result = await sdkClient.Usage.GetConsumptionAsync();
        
        if (result is Result<ConsumptionDetails>.Success success)
        {
            _cachedUsage = success.Data;
            _lastUsageCheck = DateTime.UtcNow;
        }
    }
    
    if (_cachedUsage is null)
    {
        // If we can't check usage, allow the job
        return true;
    }
    
    // Example: limit to 1000 credits per day
    const decimal dailyLimit = 1000m;
    var todayStart = DateTime.UtcNow.Date;
    
    // Note: This is simplified - in production, track daily usage separately
    if (_cachedUsage.TotalCredits + creditCost > dailyLimit)
    {
        Console.WriteLine($"Daily limit would be exceeded: {_cachedUsage.TotalCredits + creditCost:F2} / {dailyLimit:F2}");
        return false;
    }
    
    return true;
}
```

## Error Handling

Handle usage query failures gracefully:

```csharp
var result = await sdkClient.Usage.GetConsumptionAsync();

switch (result)
{
    case Result<ConsumptionDetails>.Success success:
        Console.WriteLine($"Current usage: {success.Data.TotalCredits:F2} credits");
        break;
        
    case Result<ConsumptionDetails>.ApiError apiError:
        Console.WriteLine($"API Error: {apiError.Message}");
        // Usage tracking is non-critical, continue operation
        break;
        
    case Result<ConsumptionDetails>.NetworkError networkError:
        Console.WriteLine($"Network Error: {networkError.Exception.Message}");
        // Consider caching previous values or using defaults
        break;
}
```

## Best Practices

### 1. Cache Usage Data

Usage changes slowly - cache results to reduce API calls:

```csharp
private class UsageCache
{
    public ConsumptionDetails Data { get; set; } = null!;
    public DateTime LastUpdate { get; set; }
}

private UsageCache? _usageCache;
private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);

public async Task<ConsumptionDetails?> GetCachedUsageAsync()
{
    if (_usageCache is not null && 
        DateTime.UtcNow - _usageCache.LastUpdate < _cacheExpiry)
    {
        return _usageCache.Data;
    }
    
    var result = await sdkClient.Usage.GetConsumptionAsync();
    
    if (result is Result<ConsumptionDetails>.Success success)
    {
        _usageCache = new UsageCache
        {
            Data = success.Data,
            LastUpdate = DateTime.UtcNow
        };
        return success.Data;
    }
    
    return null;
}
```

### 2. Use UTC for Date Ranges

Always use UTC dates to avoid timezone confusion:

```csharp
// Good - explicit UTC
var start = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var end = new DateTime(2025, 1, 31, 23, 59, 59, DateTimeKind.Utc);
await sdkClient.Usage.GetConsumptionAsync(start, end);

// Bad - local time can cause issues
var start = new DateTime(2025, 1, 1);
await sdkClient.Usage.GetConsumptionAsync(start, start.AddMonths(1));
```

### 3. Separate Monitoring from Business Logic

Keep usage monitoring decoupled from core functionality:

```csharp
// Usage monitoring shouldn't block job submission
public async Task<Result<JobStatusCollection>> GenerateAsync(string prompt)
{
    // Monitor usage asynchronously (fire and forget)
    _ = Task.Run(async () =>
    {
        try
        {
            var usage = await sdkClient.Usage.GetConsumptionAsync();
            // Log, alert, or update dashboard
        }
        catch (Exception ex)
        {
            // Log error but don't propagate
            Console.WriteLine($"Usage monitoring failed: {ex.Message}");
        }
    });
    
    // Continue with job submission
    return await sdkClient.Jobs
        .CreateTextToImage()
        .WithModel(model)
        .WithPrompt(prompt)
        .ExecuteAsync();
}
```

### 4. Set Up Usage Alerts

Implement proactive alerting:

```csharp
public async Task CheckUsageAlertsAsync(decimal warningThreshold, decimal criticalThreshold)
{
    var monthStart = DateTime.UtcNow.Date.AddDays(1 - DateTime.UtcNow.Day);
    var result = await sdkClient.Usage.GetConsumptionAsync(monthStart, DateTime.UtcNow);
    
    if (result is not Result<ConsumptionDetails>.Success success)
    {
        return;
    }
    
    var usage = success.Data.TotalCredits;
    
    if (usage >= criticalThreshold)
    {
        await SendAlert("CRITICAL", $"Usage: {usage:F2} / {criticalThreshold:F2}");
    }
    else if (usage >= warningThreshold)
    {
        await SendAlert("WARNING", $"Usage: {usage:F2} / {warningThreshold:F2}");
    }
}

private Task SendAlert(string level, string message)
{
    // Send email, Slack notification, etc.
    Console.WriteLine($"[{level}] {message}");
    return Task.CompletedTask;
}
```

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

- [Jobs Service](sdk-jobs.md) - Submit and manage generation jobs
- [Coverage Service](sdk-coverage.md) - Check resource availability
- [SDK Introduction](sdk-introduction.md) - Overview of all SDK services
- [Error Handling](error-handling.md) - Comprehensive error handling patterns
