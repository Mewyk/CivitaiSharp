using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCivitaiApi();

var provider = services.BuildServiceProvider();
var apiClient = provider.GetRequiredService<IApiClient>();

#region List All Tags
var allTags = await apiClient.Tags
    .WithResultsLimit(20)
    .ExecuteAsync();

if (allTags is Result<PagedResult<Tag>>.Success success)
{
    foreach (var tag in success.Data.Items)
    {
        Console.WriteLine($"{tag.Name}");
    }
}
#endregion

#region Search by Name
var searchResult = await apiClient.Tags
    .WhereName("anime")
    .WithResultsLimit(50)
    .ExecuteAsync();

if (searchResult is Result<PagedResult<Tag>>.Success searchSuccess)
{
    Console.WriteLine($"Found {searchSuccess.Data.Items.Count} anime-related tags");
    foreach (var tag in searchSuccess.Data.Items)
    {
        Console.WriteLine($"  {tag.Name}");
    }
}
#endregion

#region Pagination
var firstPage = await apiClient.Tags
    .WithResultsLimit(100)
    .ExecuteAsync();

if (firstPage is Result<PagedResult<Tag>>.Success first)
{
    Console.WriteLine($"Page 1: {first.Data.Items.Count} tags");
    
    var cursor = first.Data.Metadata?.NextCursor;
    if (cursor is not null)
    {
        var nextPage = await apiClient.Tags
            .WithResultsLimit(100)
            .ExecuteAsync(cursor: cursor);
        
        if (nextPage is Result<PagedResult<Tag>>.Success next)
        {
            Console.WriteLine($"Page 2: {next.Data.Items.Count} tags");
        }
    }
}
#endregion
