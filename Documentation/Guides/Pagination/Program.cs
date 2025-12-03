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

// #region page-size
// Set the number of results per page
var result = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync(resultsLimit: 25);

if (result is Result<PagedResult<Model>>.Success success)
{
    Console.WriteLine($"Got {success.Data.Items.Count} items");

    // Check pagination metadata
    if (success.Data.Metadata is { } metadata)
    {
        Console.WriteLine($"Total items: {metadata.TotalItems}");
        Console.WriteLine($"Total pages: {metadata.TotalPages}");
        Console.WriteLine($"Has next page: {!string.IsNullOrEmpty(metadata.NextCursor)}");
    }
}
// #endregion page-size

// #region cursor-pagination
// Iterate through all pages using cursor-based pagination
var allModels = new List<Model>();
string? cursor = null;
const int pageSize = 20;

do
{
    var pageResult = await apiClient.Models
        .WhereType(ModelType.Lora)
        .WhereTag("anime")
        .ExecuteAsync(resultsLimit: pageSize, cursor: cursor);

    if (pageResult is Result<PagedResult<Model>>.Success pageSuccess)
    {
        allModels.AddRange(pageSuccess.Data.Items);
        cursor = pageSuccess.Data.Metadata?.NextCursor;

        Console.WriteLine($"Fetched page with {pageSuccess.Data.Items.Count} items. " +
                          $"Total so far: {allModels.Count}");

        // Stop after collecting 100 items for this example
        if (allModels.Count >= 100)
        {
            break;
        }
    }
    else
    {
        Console.WriteLine("Error fetching page, stopping pagination.");
        break;
    }
}
while (!string.IsNullOrEmpty(cursor));

Console.WriteLine($"Collected {allModels.Count} models total.");
// #endregion cursor-pagination

// #region page-index
// For models, tags, and creators, you can use page index-based pagination
var modelsPage1 = await apiClient.Models
    .WhereType(ModelType.Lora)
    .WithPageIndex(1)
    .ExecuteAsync(resultsLimit: 20);

if (modelsPage1 is Result<PagedResult<Model>>.Success modelSuccess)
{
    Console.WriteLine($"Page 1: {modelSuccess.Data.Items.Count} models");

    // Fetch page 2
    var modelsPage2 = await apiClient.Models
        .WhereType(ModelType.Lora)
        .WithPageIndex(2)
        .ExecuteAsync(resultsLimit: 20);

    if (modelsPage2 is Result<PagedResult<Model>>.Success page2Success)
    {
        Console.WriteLine($"Page 2: {page2Success.Data.Items.Count} models");
    }
}
// #endregion page-index
