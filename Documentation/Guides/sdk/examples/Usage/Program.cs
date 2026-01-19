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

Console.WriteLine("Usage examples completed successfully.");

await host.StopAsync();
