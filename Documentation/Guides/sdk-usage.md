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
    Console.WriteLine($"Images Generated: {success.Data.Images}");
    Console.WriteLine($"Total Cost: {success.Data.TotalCost:F2} Buzz");
    Console.WriteLine($"Period: {success.Data.StartDate} to {success.Data.EndDate}");
}
```

### Get Consumption for Specific Period

```csharp
var startDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var endDate = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc);

var result = await sdkClient.Usage.GetConsumptionAsync(startDate, endDate);

if (result is Result<ConsumptionDetails>.Success success)
{
    Console.WriteLine($"January 2026 Usage:");
    Console.WriteLine($"  Images: {success.Data.Images}");
    Console.WriteLine($"  Total Cost: {success.Data.TotalCost:F2} Buzz");
}
```

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

### Use Case 1: Simple Consumption Check

**Scenario**: Quick check of current account consumption status.

**When to use**: Before starting new work to verify sufficient credits.

```csharp
public async Task<ConsumptionDetails?> GetConsumptionStatusAsync()
{
    var result = await sdkClient.Usage.GetConsumptionAsync(cancellationToken: default);
    
    if (result is Result<ConsumptionDetails>.Success success)
    {
        return success.Data;
    }
    
    return null;
}

// Usage
var consumption = await GetConsumptionStatusAsync();
if (consumption is null)
{
    Console.WriteLine("Could not retrieve consumption data");
    return;
}

Console.WriteLine($"Total spent: {consumption.TotalCost:N2} Buzz");
Console.WriteLine($"Images generated: {consumption.Images}");

// Proceed with job submission
var jobResult = await sdkClient.Jobs
    .CreateImage()
    .WithAir(checkpoint)
    .WithPositivePrompt("detailed artwork")
    .WithDimensions(1024, 1024)
    .ExecuteAsync();
```

### Use Case 2: Budget Tracking

**Scenario**: Track spending over time against a monthly budget.

**When to use**: Production applications requiring cost monitoring.

```csharp
public sealed class BudgetTracker(ISdkClient sdkClient)
{
    public sealed record BudgetStatus(
        decimal TotalBudget,
        decimal? Consumed,
        decimal? Remaining,
        decimal? PercentageUsed,
        int? ImagesGenerated);
    
    public async Task<Result<BudgetStatus>> GetBudgetStatusAsync(
        decimal monthlyBudget,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var result = await sdkClient.Usage.GetConsumptionAsync(
            startDate: startDate,
            endDate: endDate,
            cancellationToken: cancellationToken);
        
        if (result is not Result<ConsumptionDetails>.Success success)
        {
            return new Result<BudgetStatus>.Failure(result.ErrorOrDefault!);
        }
        
        var consumed = success.Data.TotalCost ?? 0;
        var remaining = monthlyBudget - consumed;
        var percentageUsed = monthlyBudget > 0 ? (consumed / monthlyBudget) * 100 : 0;
        
        var status = new BudgetStatus(
            TotalBudget: monthlyBudget,
            Consumed: consumed,
            Remaining: remaining,
            PercentageUsed: percentageUsed,
            ImagesGenerated: success.Data.Images);
        
        return new Result<BudgetStatus>.Success(status);
    }
}

// Example Usage
var tracker = new BudgetTracker(sdkClient);
var monthlyBudget = 10000m;
var startOfMonth = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var now = DateTime.UtcNow;

var budgetResult = await tracker.GetBudgetStatusAsync(
    monthlyBudget,
    startOfMonth,
    now,
    cancellationToken);

if (budgetResult is Result<BudgetTracker.BudgetStatus>.Success success)
{
    var status = success.Data;
    Console.WriteLine($"Budget: {status.TotalBudget:N2} Buzz");
    Console.WriteLine($"Consumed: {status.Consumed:N2} Buzz ({status.PercentageUsed:N1}%)");
    Console.WriteLine($"Remaining: {status.Remaining:N2} Buzz");
    Console.WriteLine($"Images Generated: {status.ImagesGenerated}");
}
```

### Use Case 3: Cost Analysis and Reporting

**Scenario**: Detailed consumption analysis for reporting and optimization.

**When to use**: Monthly reviews, cost optimization, department chargebacks.

```csharp
public sealed class ConsumptionAnalyzer(ISdkClient sdkClient)
{
    public sealed record ConsumptionReport(
        DateTime PeriodStart,
        DateTime PeriodEnd,
        int DaysInPeriod,
        decimal? TotalCost,
        int? TotalImages,
        decimal? AverageCostPerImage,
        decimal? AverageCostPerDay,
        decimal? ProjectedMonthlyCost);
    
    public async Task<Result<ConsumptionReport>> GenerateReportAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var result = await sdkClient.Usage.GetConsumptionAsync(
            startDate: startDate,
            endDate: endDate,
            cancellationToken: cancellationToken);
        
        if (result is not Result<ConsumptionDetails>.Success success)
        {
            return new Result<ConsumptionReport>.Failure(result.ErrorOrDefault!);
        }
        
        var data = success.Data;
        var daysInPeriod = (endDate - startDate).Days;
        var totalCost = data.TotalCost ?? 0;
        var totalImages = data.Images ?? 0;
        
        var avgCostPerDay = daysInPeriod > 0 ? totalCost / daysInPeriod : 0;
        var avgCostPerImage = totalImages > 0 ? totalCost / totalImages : 0;
        var projectedMonthlyCost = avgCostPerDay * 30;
        
        var report = new ConsumptionReport(
            PeriodStart: startDate,
            PeriodEnd: endDate,
            DaysInPeriod: daysInPeriod,
            TotalCost: totalCost,
            TotalImages: totalImages,
            AverageCostPerImage: avgCostPerImage,
            AverageCostPerDay: avgCostPerDay,
            ProjectedMonthlyCost: projectedMonthlyCost);
        
        return new Result<ConsumptionReport>.Success(report);
    }
    
    public async Task<Result<List<ConsumptionReport>>> GenerateMonthlyTrendAsync(
        int monthsBack,
        CancellationToken cancellationToken = default)
    {
        var reports = new List<ConsumptionReport>();
        var now = DateTime.UtcNow;
        
        for (var i = 0; i < monthsBack; i++)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            
            var result = await GenerateReportAsync(monthStart, monthEnd, cancellationToken);
            
            if (result is Result<ConsumptionReport>.Success success)
            {
                reports.Add(success.Data);
            }
        }
        
        reports.Reverse(); // Oldest first
        return new Result<List<ConsumptionReport>>.Success(reports);
    }
}

// Full Example - Monthly Report
var analyzer = new ConsumptionAnalyzer(sdkClient);

// Generate report for last month
var lastMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-1);
var lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);

var monthlyReportResult = await analyzer.GenerateReportAsync(lastMonthStart, lastMonthEnd, cancellationToken);

if (monthlyReportResult is Result<ConsumptionAnalyzer.ConsumptionReport>.Success monthlyReportSuccess)
{
    var monthlyReport = monthlyReportSuccess.Data;
    
    Console.WriteLine("=== Monthly Consumption Report ===");
    Console.WriteLine($"Period: {monthlyReport.PeriodStart:yyyy-MM-dd} to {monthlyReport.PeriodEnd:yyyy-MM-dd}");
    Console.WriteLine($"Days: {monthlyReport.DaysInPeriod}");
    Console.WriteLine();
    Console.WriteLine($"Total Cost: {monthlyReport.TotalCost:N2} Buzz");
    Console.WriteLine($"Total Images: {monthlyReport.TotalImages:N0}");
    Console.WriteLine($"Average Cost/Image: {monthlyReport.AverageCostPerImage:N2} Buzz");
    Console.WriteLine($"Average Cost/Day: {monthlyReport.AverageCostPerDay:N2} Buzz");
    Console.WriteLine();
    Console.WriteLine($"Projected Monthly: {monthlyReport.ProjectedMonthlyCost:N2} Buzz");
}

// Generate 6-month trend
var trendAnalysisResult = await analyzer.GenerateMonthlyTrendAsync(6, cancellationToken);

if (trendAnalysisResult is Result<List<ConsumptionAnalyzer.ConsumptionReport>>.Success trendSuccess)
{
    Console.WriteLine("\n=== 6-Month Trend ===");
    Console.WriteLine($"{"Month",-12} {"Images",8} {"Cost",12} {"Avg/Image",12}");
    Console.WriteLine(new string('-', 46));
    
    foreach (var trendReport in trendSuccess.Data)
    {
        Console.WriteLine(
            $"{trendReport.PeriodStart:yyyy-MM,-12} " +
            $"{trendReport.TotalImages,8:N0} " +
            $"{trendReport.TotalCost,12:N2} " +
            $"{trendReport.AverageCostPerImage,12:N2}");
    }
}
```

### Use Case 4: Rate Limiting and Throttling

**Scenario**: Intelligent rate limiting based on remaining credits and cost projections.

**When to use**: Batch processing systems, automated workflows requiring cost control.

```csharp
public sealed class RateLimiter(ISdkClient sdkClient)
{
    public sealed record ThrottleRecommendation(
        bool ShouldThrottle,
        TimeSpan SuggestedDelay,
        string Reason);
    
    public async Task<Result<ThrottleRecommendation>> GetThrottleRecommendationAsync(
        decimal estimatedJobCost,
        decimal dailyBudgetLimit,
        CancellationToken cancellationToken = default)
    {
        // Get today's consumption
        var today = DateTime.UtcNow.Date;
        var result = await sdkClient.Usage.GetConsumptionAsync(
            startDate: today,
            endDate: today.AddDays(1),
            cancellationToken: cancellationToken);
        
        if (result is not Result<ConsumptionDetails>.Success success)
        {
            return new Result<ThrottleRecommendation>.Failure(result.ErrorOrDefault!);
        }
        
        var todaySpent = success.Data.TotalCost;
        var remaining = dailyBudgetLimit - todaySpent;
        var percentageUsed = dailyBudgetLimit > 0 ? (todaySpent / dailyBudgetLimit) * 100 : 0;
        
        // Calculate throttle recommendation
        var recommendation = percentageUsed switch
        {
            >= 95 => new ThrottleRecommendation(
                ShouldThrottle: true,
                SuggestedDelay: TimeSpan.FromHours(1),
                Reason: $"Daily budget 95% consumed ({todaySpent:N2}/{dailyBudgetLimit:N2} Buzz)"),
            
            >= 80 => new ThrottleRecommendation(
                ShouldThrottle: true,
                SuggestedDelay: TimeSpan.FromMinutes(30),
                Reason: $"Daily budget 80% consumed ({todaySpent:N2}/{dailyBudgetLimit:N2} Buzz)"),
            
            >= 60 when estimatedJobCost > remaining => new ThrottleRecommendation(
                ShouldThrottle: true,
                SuggestedDelay: TimeSpan.FromMinutes(15),
                Reason: $"Job cost ({estimatedJobCost:N2}) exceeds remaining budget ({remaining:N2} Buzz)"),
            
            _ => new ThrottleRecommendation(
                ShouldThrottle: false,
                SuggestedDelay: TimeSpan.Zero,
                Reason: $"Budget healthy ({percentageUsed:N1}% used)")
        };
        
        return new Result<ThrottleRecommendation>.Success(recommendation);
    }
}

// Full Example - Batch Processing with Rate Limiting
var rateLimiter = new RateLimiter(sdkClient);
var dailyBudgetLimit = 1000m; // 1,000 Buzz per day
var estimatedJobCost = 50m;

var prompts = new[]
{
    "beautiful landscape at sunset",
    "futuristic city skyline",
    "detailed character portrait",
    // ... many more prompts
};

foreach (var prompt in prompts)
{
    // Check if we should throttle
    var throttleRecommendationResult = await rateLimiter.GetThrottleRecommendationAsync(
        estimatedJobCost,
        dailyBudgetLimit,
        cancellationToken);
    
    if (throttleRecommendationResult is Result<RateLimiter.ThrottleRecommendation>.Success throttleSuccess)
    {
        var throttleRecommendation = throttleSuccess.Data;
        
        if (throttleRecommendation.ShouldThrottle)
        {
            Console.WriteLine($"THROTTLING: {throttleRecommendation.Reason}");
            Console.WriteLine($"Waiting {throttleRecommendation.SuggestedDelay.TotalMinutes:N0} minutes...");
            
            await Task.Delay(throttleRecommendation.SuggestedDelay, cancellationToken);
            continue;
        }
    }
    
    // Submit job
    var batchJobResult = await sdkClient.Jobs
        .CreateImage()
        .WithAir(checkpoint)
        .WithPositivePrompt(prompt)
        .WithDimensions(1024, 1024)
        .ExecuteAsync(cancellationToken);
    
    if (batchJobResult is Result<JobStatusCollection>.Success batchJobSuccess)
    {
        Console.WriteLine($"SUCCESS: Job submitted: {prompt.Substring(0, 30)}...");
    }
    
    // Small delay between jobs
    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
}
```

### Use Case 5: Multi-Project Cost Tracking

**Scenario**: Track consumption across multiple projects/teams with separate budgets.

**When to use**: Organizations with multiple departments or projects sharing one account.

```csharp
public sealed class ProjectCostTracker
{
    private readonly ISdkClient _sdkClient;
    private readonly Dictionary<string, ProjectBudget> _projects = [];
    
    public sealed record ProjectBudget(
        string ProjectId,
        string ProjectName,
        decimal MonthlyBudget,
        decimal CurrentSpend,
        int JobsSubmitted,
        DateTime LastUpdated);
    
    public ProjectCostTracker(ISdkClient sdkClient)
    {
        _sdkClient = sdkClient;
    }
    
    public void RegisterProject(string projectId, string name, decimal monthlyBudget)
    {
        _projects[projectId] = new ProjectBudget(
            ProjectId: projectId,
            ProjectName: name,
            MonthlyBudget: monthlyBudget,
            CurrentSpend: 0,
            JobsSubmitted: 0,
            LastUpdated: DateTime.UtcNow);
    }
    
    public async Task<bool> CanSubmitJobAsync(
        string projectId,
        decimal estimatedCost,
        CancellationToken cancellationToken = default)
    {
        if (!_projects.TryGetValue(projectId, out var project))
            return false;
        
        // Refresh account consumption
        var result = await _sdkClient.Usage.GetConsumptionAsync(cancellationToken: cancellationToken);
        
        if (result is not Result<ConsumptionDetails>.Success success)
            return false;
        
        // Check project budget
        var projectedSpend = project.CurrentSpend + estimatedCost;
        if (projectedSpend > project.MonthlyBudget)
        {
            Console.WriteLine($"Project '{project.ProjectName}' would exceed budget: " +
                            $"{projectedSpend:N2} > {project.MonthlyBudget:N2}");
            return false;
        }
        
        return true;
    }
    
    public void RecordJobCost(string projectId, decimal actualCost)
    {
        if (_projects.TryGetValue(projectId, out var project))
        {
            _projects[projectId] = project with
            {
                CurrentSpend = project.CurrentSpend + actualCost,
                JobsSubmitted = project.JobsSubmitted + 1,
                LastUpdated = DateTime.UtcNow
            };
        }
    }
    
    public IReadOnlyDictionary<string, ProjectBudget> GetAllProjects() => _projects;
    
    public void ResetMonthlyBudgets()
    {
        var keys = _projects.Keys.ToList();
        foreach (var projectId in keys)
        {
            var project = _projects[projectId];
            _projects[projectId] = project with
            {
                CurrentSpend = 0,
                JobsSubmitted = 0,
                LastUpdated = DateTime.UtcNow
            };
        }
    }
}

// Full Example
var projectTracker = new ProjectCostTracker(sdkClient);

// Register projects
projectTracker.RegisterProject("proj-001", "Marketing Campaign", monthlyBudget: 5000m);
projectTracker.RegisterProject("proj-002", "Product Development", monthlyBudget: 3000m);
projectTracker.RegisterProject("proj-003", "R&D Experiments", monthlyBudget: 2000m);

// Submit job for specific project
var targetProjectId = "proj-001";
var projectJobCost = 75m;

if (await projectTracker.CanSubmitJobAsync(targetProjectId, projectJobCost, cancellationToken))
{
    var projectJobResult = await sdkClient.Jobs
        .CreateImage()
        .WithAir(checkpoint)
        .WithPositivePrompt("marketing material, product showcase")
        .WithDimensions(1024, 1024)
        .ExecuteAsync(cancellationToken);
    
    if (projectJobResult is Result<JobStatusCollection>.Success projectJobSuccess)
    {
        // Record actual cost (in real scenario, get from completed job)
        projectTracker.RecordJobCost(targetProjectId, projectJobCost);
        Console.WriteLine($"SUCCESS: Job submitted for project: {targetProjectId}");
    }
}
else
{
    Console.WriteLine($"ERROR: Cannot submit job for project: {targetProjectId}");
}

// Generate project report
Console.WriteLine("\n=== Project Budget Report ===");
Console.WriteLine($"{"Project",-25} {"Budget",12} {"Spent",12} {"Remaining",12} {"Jobs",8}");
Console.WriteLine(new string('-', 70));

foreach (var (_, project) in projectTracker.GetAllProjects())
{
    var remaining = project.MonthlyBudget - project.CurrentSpend;
    var percentUsed = project.MonthlyBudget > 0 
        ? (project.CurrentSpend / project.MonthlyBudget) * 100 
        : 0;
    
    Console.WriteLine(
        $"{project.ProjectName,-25} " +
        $"{project.MonthlyBudget,12:N2} " +
        $"{project.CurrentSpend,12:N2} " +
        $"{remaining,12:N2} " +
        $"{project.JobsSubmitted,8:N0}");
    
    if (percentUsed >= 90)
    {
        Console.WriteLine($"  WARNING: {percentUsed:N1}% of budget used");
    }
}
```

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
            Console.WriteLine($"  Images: {success.Data.Images,6}");
            Console.WriteLine($"  Total Cost: {success.Data.TotalCost,8:F2} Buzz");
        }
    }
}
```

### Calculate Average Cost Per Job

```csharp
public async Task<decimal?> GetAverageCostPerImageAsync(DateTime start, DateTime end)
{
    var result = await sdkClient.Usage.GetConsumptionAsync(start, end);
    
    if (result is Result<ConsumptionDetails>.Success success)
    {
        var images = success.Data.Images ?? 0;
        var totalCost = success.Data.TotalCost ?? 0;
        var avgCost = images > 0 ? totalCost / images : 0;
        Console.WriteLine($"Average cost per image: {avgCost:F3} Buzz");
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
    
    var percentUsed = (success.Data.TotalCost / monthlyBudget) * 100;
    var remaining = monthlyBudget - success.Data.TotalCost;
    
    Console.WriteLine($"Budget Status:");
    Console.WriteLine($"  Used: {success.Data.TotalCost:F2} / {monthlyBudget:F2} Buzz ({percentUsed:F1}%)");
    Console.WriteLine($"  Remaining: {remaining:F2} Buzz");
    
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

### Usage Summary Report

```csharp
public async Task ShowUsageSummaryAsync(DateTime start, DateTime end)
{
    var result = await sdkClient.Usage.GetConsumptionAsync(start, end);
    
    if (result is not Result<ConsumptionDetails>.Success success)
    {
        Console.WriteLine("Failed to retrieve usage data");
        return;
    }
    
    var data = success.Data;
    Console.WriteLine($"Usage Summary ({data.StartDate:yyyy-MM-dd} to {data.EndDate:yyyy-MM-dd}):");
    Console.WriteLine($"  Total Images: {data.Images}");
    Console.WriteLine($"  Total Cost: {data.TotalCost:F2} Buzz");
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
    
    // Example: limit to 1000 Buzz per day
    const decimal dailyLimit = 1000m;
    var todayStart = DateTime.UtcNow.Date;
    var currentTotalCost = _cachedUsage.TotalCost ?? 0;
    
    // Note: This is simplified - in production, track daily usage separately
    if (currentTotalCost + creditCost > dailyLimit)
    {
        Console.WriteLine($"Daily limit would be exceeded: {currentTotalCost + creditCost:F2} / {dailyLimit:F2}");
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
        Console.WriteLine($"Current usage: {success.Data.TotalCost:F2} Buzz ({success.Data.Images} images)");
        break;
        
    case Result<ConsumptionDetails>.Failure failure:
        Console.WriteLine($"Error: {failure.Error.Message}");
        // Usage tracking is non-critical, continue operation
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
var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var end = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc);
await sdkClient.Usage.GetConsumptionAsync(start, end);

// Bad - local time can cause issues
var start = new DateTime(2026, 1, 1);
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
        .CreateImage()
        .WithAir(model)
        .WithPositivePrompt(prompt)
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
    
    var usage = success.Data.TotalCost;
    
    if (usage >= criticalThreshold)
    {
        await SendAlert("CRITICAL", $"Usage: {usage:F2} / {criticalThreshold:F2} Buzz");
    }
    else if (usage >= warningThreshold)
    {
        await SendAlert("WARNING", $"Usage: {usage:F2} / {warningThreshold:F2} Buzz");
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
