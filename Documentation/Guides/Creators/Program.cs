using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCivitaiApi();

var provider = services.BuildServiceProvider();
var apiClient = provider.GetRequiredService<IApiClient>();

#region Search by Username
var searchResult = await apiClient.Creators
    .WhereName("popular")
    .ExecuteAsync(resultsLimit: 20);

if (searchResult is Result<PagedResult<Creator>>.Success success)
{
    foreach (var creator in success.Data.Items)
    {
        Console.WriteLine($"{creator.Username}: {creator.ModelCount ?? 0} models");
    }
}
#endregion

#region List All Creators
var allCreators = await apiClient.Creators
    .ExecuteAsync(resultsLimit: 50);

if (allCreators is Result<PagedResult<Creator>>.Success allSuccess)
{
    Console.WriteLine($"Found {allSuccess.Data.Items.Count} creators");
}
#endregion

#region Page-Based Pagination
// Get first page
var page1 = await apiClient.Creators
    .WithPageIndex(1)
    .ExecuteAsync(resultsLimit: 50);

if (page1 is Result<PagedResult<Creator>>.Success firstPage)
{
    Console.WriteLine($"Page 1: {firstPage.Data.Items.Count} creators");
    
    // Get second page
    var page2 = await apiClient.Creators
        .WithPageIndex(2)
        .ExecuteAsync(resultsLimit: 50);
    
    if (page2 is Result<PagedResult<Creator>>.Success secondPage)
    {
        Console.WriteLine($"Page 2: {secondPage.Data.Items.Count} creators");
    }
    
    // Navigate to specific page
    var page5 = await apiClient.Creators
        .WithPageIndex(5)
        .ExecuteAsync(resultsLimit: 50);
}
#endregion
