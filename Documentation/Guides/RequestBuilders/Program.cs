using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi();
var host = builder.Build();
var apiClient = host.Services.GetRequiredService<IApiClient>();

// #region filtering
// Filter by model type
var loraModels = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync();

// Combine multiple filters
var animeLorasByCreator = await apiClient.Models
    .WhereType(ModelType.Lora)
    .WhereTag("anime")
    .WhereUsername("civitai")
    .ExecuteAsync(resultsLimit: 20);
// #endregion filtering

// #region sorting
// Sort by highest rated in the last week
var topRated = await apiClient.Models
    .WhereType(ModelType.Checkpoint)
    .OrderBy(ModelSort.HighestRated)
    .WherePeriod(TimePeriod.Week)
    .ExecuteAsync(resultsLimit: 10);
// #endregion sorting

// #region limiting
// Return only 5 results per page
var limitedResults = await apiClient.Models
    .WhereName("sdxl")
    .ExecuteAsync(resultsLimit: 5);

if (limitedResults is Result<PagedResult<Model>>.Success success)
{
    Console.WriteLine($"Got {success.Data.Items.Count} models");
}
// #endregion limiting

// #region single-item
// Get a specific model by ID
var specificModel = await apiClient.Models.GetByIdAsync(123456);

if (specificModel is Result<Model>.Success modelSuccess)
{
    Console.WriteLine($"Model: {modelSuccess.Data.Name}");
}

// Get the first matching model
var firstMatch = await apiClient.Models
    .WhereName("example")
    .FirstOrDefaultAsync();

if (firstMatch is Result<Model?>.Success { Data: { } model })
{
    Console.WriteLine($"Found: {model.Name}");
}
// #endregion single-item
