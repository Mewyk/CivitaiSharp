using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi(_ => { });
var host = builder.Build();
var apiClient = host.Services.GetRequiredService<IApiClient>();

// #region page-size
// Set the number of results per page
var result = await apiClient.Models
    .WhereType(ModelType.Lora)
    .WithResultsLimit(25)
    .ExecuteAsync();

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
// For images, you can use page index-based pagination
var imagesPage1 = await apiClient.Images
    .WhereModelId(123456)
    .WithPageIndex(1)
    .WithResultsLimit(20)
    .ExecuteAsync();

if (imagesPage1 is Result<PagedResult<Image>>.Success imageSuccess)
{
    Console.WriteLine($"Page 1: {imageSuccess.Data.Items.Count} images");

    // Fetch page 2
    var imagesPage2 = await apiClient.Images
        .WhereModelId(123456)
        .WithPageIndex(2)
        .WithResultsLimit(20)
        .ExecuteAsync();

    if (imagesPage2 is Result<PagedResult<Image>>.Success page2Success)
    {
        Console.WriteLine($"Page 2: {page2Success.Data.Items.Count} images");
    }
}
// #endregion page-index
