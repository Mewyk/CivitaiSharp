using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Extensions;
using CivitaiSharp.Sdk.Models.Results;
using CivitaiSharp.Sdk.Models.Usage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiSdk(builder.Configuration);
var host = builder.Build();
await host.StartAsync();

var sdkClient = host.Services.GetRequiredService<ISdkClient>();

#region GetCurrentConsumption
var result = await sdkClient.Usage.GetConsumptionAsync();

if (result is Result<ConsumptionDetails>.Success success)
{
    Console.WriteLine($"Images Generated: {success.Data.Images}");
    Console.WriteLine($"Total Cost: {success.Data.TotalCost:F2} Buzz");
    Console.WriteLine($"Period: {success.Data.StartDate} to {success.Data.EndDate}");
}
#endregion

#region GetConsumptionForSpecificPeriod
var startDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var endDate = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc);

var periodResult = await sdkClient.Usage.GetConsumptionAsync(startDate, endDate);

if (periodResult is Result<ConsumptionDetails>.Success periodSuccess)
{
    Console.WriteLine($"January 2026 Usage:");
    Console.WriteLine($"  Images: {periodSuccess.Data.Images}");
    Console.WriteLine($"  Total Cost: {periodSuccess.Data.TotalCost:F2} Buzz");
}
#endregion

#region BudgetMonitoring
async Task<bool> CheckBudgetAsync(decimal monthlyBudget)
{
    var monthStart = DateTime.UtcNow.Date.AddDays(1 - DateTime.UtcNow.Day);
    var monthEnd = monthStart.AddMonths(1);
    
    var budgetResult = await sdkClient.Usage.GetConsumptionAsync(monthStart, monthEnd);
    
    if (budgetResult is not Result<ConsumptionDetails>.Success success)
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
#endregion

#region RateLimitingProtection
DateTime? _lastUsageCheck = null;
ConsumptionDetails? _cachedUsage = null;

async Task<bool> CanSubmitJobAsync(decimal creditCost)
{
    // Cache usage checks (avoid API spam)
    if (_lastUsageCheck is null || 
        DateTime.UtcNow - _lastUsageCheck.Value > TimeSpan.FromMinutes(5))
    {
        var rateLimitResult = await sdkClient.Usage.GetConsumptionAsync();
        
        if (rateLimitResult is Result<ConsumptionDetails>.Success success)
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
#endregion

#region ErrorHandling
var errorResult = await sdkClient.Usage.GetConsumptionAsync();

switch (errorResult)
{
    case Result<ConsumptionDetails>.Success usageSuccess:
        Console.WriteLine($"Current usage: {usageSuccess.Data.TotalCost:F2} Buzz ({usageSuccess.Data.Images} images)");
        break;
        
    case Result<ConsumptionDetails>.Failure failure:
        Console.WriteLine($"Error: {failure.Error.Message}");
        // Usage tracking is non-critical, continue operation
        break;
}
#endregion

#region SeparateMonitoringFromBusinessLogic
async Task<Result<JobStatusCollection>> GenerateAsync(string prompt)
{
    var model = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, AirSource.Civitai, 4201, 130072);
    
    var monitoringTask = Task.Run(async () =>
    {
        try
        {
            var usage = await sdkClient.Usage.GetConsumptionAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Usage monitoring failed: {ex.Message}");
        }
    });
    
    return await sdkClient.Jobs
        .CreateImage()
        .WithAir(model)
        .WithPositivePrompt(prompt)
        .ExecuteAsync();
}
#endregion

#region SetUpUsageAlerts
async Task CheckUsageAlertsAsync(decimal warningThreshold, decimal criticalThreshold)
{
    var monthStart = DateTime.UtcNow.Date.AddDays(1 - DateTime.UtcNow.Day);
    var alertResult = await sdkClient.Usage.GetConsumptionAsync(monthStart, DateTime.UtcNow);
    
    if (alertResult is not Result<ConsumptionDetails>.Success success)
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

Task SendAlert(string level, string message)
{
    // Send email, Slack notification, etc.
    Console.WriteLine($"[{level}] {message}");
    return Task.CompletedTask;
}
#endregion

#region CacheUsageData
UsageCache? _usageCache = null;
var _cacheExpiry = TimeSpan.FromMinutes(5);

async Task<ConsumptionDetails?> GetCachedUsageAsync()
{
    if (_usageCache is not null && 
        DateTime.UtcNow - _usageCache.LastUpdate < _cacheExpiry)
    {
        return _usageCache.Data;
    }
    
    var cacheResult = await sdkClient.Usage.GetConsumptionAsync();
    
    if (cacheResult is Result<ConsumptionDetails>.Success cacheSuccess)
    {
        _usageCache = new UsageCache
        {
            Data = cacheSuccess.Data,
            LastUpdate = DateTime.UtcNow
        };
        return cacheSuccess.Data;
    }
    
    return null;
}
#endregion

// Demonstrate the examples
await CheckBudgetAsync(5000m);
await CanSubmitJobAsync(10m);
await GenerateAsync("example prompt");
await CheckUsageAlertsAsync(3000m, 4500m);
await GetCachedUsageAsync();

Console.WriteLine("Usage examples completed successfully.");

await host.StopAsync();

class UsageCache
{
    public required ConsumptionDetails Data { get; set; }
    public DateTime LastUpdate { get; set; }
}
