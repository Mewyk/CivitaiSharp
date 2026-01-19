using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// See Common/Program.cs for setup patterns: #CoreBasicSetup, #CoreSetupWithApiKey, #ResultPatternMatching

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi();
var host = builder.Build();

await host.StartAsync();

var apiClient = host.Services.GetRequiredService<IApiClient>();

#region Filtering
// Filter by model type
var loraModels = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync();

// Combine multiple filters
var animeLorasByCreator = await apiClient.Models
    .WhereType(ModelType.Lora)
    .WhereTag("anime")
    .WhereUsername("Mewyk")
    .ExecuteAsync(resultsLimit: 20);
#endregion

#region Sorting
// Sort by highest rated in the last week
var topRated = await apiClient.Models
    .WhereType(ModelType.Checkpoint)
    .OrderBy(ModelSort.HighestRated)
    .WherePeriod(TimePeriod.Week)
    .ExecuteAsync(resultsLimit: 10);
#endregion

#region Limiting
// Return only 5 results per page
var limitedResults = await apiClient.Models
    .WhereName("sdxl")
    .ExecuteAsync(resultsLimit: 5);

if (limitedResults is Result<PagedResult<Model>>.Success success)
{
    Console.WriteLine($"Got {success.Data.Items.Count} models");
}
#endregion

#region SingleItem
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
#endregion

#region Immutability
var baseQuery = apiClient.Models.WhereType(ModelType.Lora);

// baseQuery is NOT modified - a new instance is created
var animeQuery = baseQuery.WhereTag("anime");
var realisticQuery = baseQuery.WhereTag("realistic");

// All three are independent queries
var baseResult = await baseQuery.ExecuteAsync(resultsLimit: 5);
var animeResult = await animeQuery.ExecuteAsync(resultsLimit: 5);
var realisticResult = await realisticQuery.ExecuteAsync(resultsLimit: 5);

Console.WriteLine($"Base query: {(baseResult.IsSuccess ? baseResult.ValueOrDefault!.Items.Count : 0)} items");
Console.WriteLine($"Anime query: {(animeResult.IsSuccess ? animeResult.ValueOrDefault!.Items.Count : 0)} items");
Console.WriteLine($"Realistic query: {(realisticResult.IsSuccess ? realisticResult.ValueOrDefault!.Items.Count : 0)} items");
#endregion

#region Validation
try
{
    // Throws ArgumentException - name cannot be null or whitespace
    var invalidName = apiClient.Models.WhereName("");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
}

try
{
    // Throws ArgumentOutOfRangeException - ID must be positive
    var invalidId = await apiClient.Models.GetByIdAsync(0);
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
}

try
{
    // Throws ArgumentOutOfRangeException - limit must be 1-100
    var invalidLimit = await apiClient.Models.ExecuteAsync(resultsLimit: 500);
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
}
#endregion

await host.StopAsync();
