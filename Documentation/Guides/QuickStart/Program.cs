using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// #region setup
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCivitaiApi(options =>
{
    // API key is optional for public queries
    // options.ApiKey = "your-api-key";
});

var host = builder.Build();
// #endregion setup

// #region query
var apiClient = host.Services.GetRequiredService<IApiClient>();

// Build a query for LoRA models tagged with "anime"
var result = await apiClient.Models
    .WhereType(ModelType.Lora)
    .WhereTag("anime")
    .WithResultsLimit(10)
    .ExecuteAsync();
// #endregion query

// The result is always checked with pattern matching
// because API calls can fail for various reasons

// #region handling
if (result is Result<PagedResult<Model>>.Success success)
{
    Console.WriteLine($"Found {success.Data.Items.Count} models:");
    foreach (var model in success.Data.Items)
    {
        Console.WriteLine($"  - {model.Name} (ID: {model.Id})");
    }
}
else if (result is Result<PagedResult<Model>>.Failure failure)
{
    Console.WriteLine($"Error: {failure.Error.Code} - {failure.Error.Message}");
}
// #endregion handling

await host.RunAsync();
