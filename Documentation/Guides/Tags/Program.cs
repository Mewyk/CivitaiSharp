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
    .ExecuteAsync(resultsLimit: 20);

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
    .WithPageIndex(1)
    .ExecuteAsync(resultsLimit: 100);

if (firstPage is Result<PagedResult<Tag>>.Success first)
{
    Console.WriteLine($"Page 1: {first.Data.Items.Count} tags");
    
    // Fetch page 2
    var secondPage = await apiClient.Tags
        .WithPageIndex(2)
        .ExecuteAsync(resultsLimit: 100);
    
    if (secondPage is Result<PagedResult<Tag>>.Success second)
    {
        Console.WriteLine($"Page 2: {second.Data.Items.Count} tags");
    }
}
#endregion
