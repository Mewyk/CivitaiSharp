using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Request;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi();
var host = builder.Build();

await host.StartAsync();

var apiClient = host.Services.GetRequiredService<IApiClient>();

#region PageSize
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
#endregion

#region CursorPagination
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
#endregion

#region PageIndex
// For models, tags, and creators, you can use page index-based pagination
var firstPageResult = await apiClient.Models
    .WhereType(ModelType.Lora)
    .WithPageIndex(1)
    .ExecuteAsync(resultsLimit: 20);

if (firstPageResult is Result<PagedResult<Model>>.Success firstSuccess)
{
    Console.WriteLine($"Page 1: {firstSuccess.Data.Items.Count} models");

    // Fetch page 2
    var secondPageResult = await apiClient.Models
        .WhereType(ModelType.Lora)
        .WithPageIndex(2)
        .ExecuteAsync(resultsLimit: 20);

    if (secondPageResult is Result<PagedResult<Model>>.Success secondSuccess)
    {
        Console.WriteLine($"Page 2: {secondSuccess.Data.Items.Count} models");
    }
}
#endregion

#region PaginationMetadata
var metadataResult = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync(resultsLimit: 10);

if (metadataResult is Result<PagedResult<Model>>.Success metadataSuccess)
{
    var metadata = metadataSuccess.Data.Metadata;

    if (metadata is not null)
    {
        Console.WriteLine($"Page {metadata.CurrentPage} of {metadata.TotalPages}");
        Console.WriteLine($"Total Items: {metadata.TotalItems}");
    }
}
#endregion

#region GetFirstResult
var firstResult = await apiClient.Models
    .WhereName("specific-model")
    .FirstOrDefaultAsync();

if (firstResult is Result<Model?>.Success { Data: { } model })
{
    Console.WriteLine($"Found: {model.Name}");
}
else
{
    Console.WriteLine("Model not found");
}
#endregion

await host.StopAsync();

#region AsyncEnumerationExtension
// Extension method example for async enumeration

public static class PaginationExtensions
{
    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
        this ModelBuilder builder,
        int pageSize = 20,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = null;
        
        do
        {
            var result = await builder.ExecuteAsync(pageSize, cursor, cancellationToken);
            
            if (result is not Result<PagedResult<Model>>.Success success)
            {
                yield break;
            }
            
            foreach (var item in success.Data.Items)
            {
                yield return (T)(object)item;
            }
            
            cursor = success.Data.Metadata?.NextCursor;
        }
        while (!string.IsNullOrEmpty(cursor));
    }
}
#endregion
