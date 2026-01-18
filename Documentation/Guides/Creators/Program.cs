using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi();
var host = builder.Build();

await host.StartAsync();

var apiClient = host.Services.GetRequiredService<IApiClient>();

// #region Search by Username
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
// #endregion

// #region List All Creators
var allCreators = await apiClient.Creators
    .ExecuteAsync(resultsLimit: 50);

if (allCreators is Result<PagedResult<Creator>>.Success allSuccess)
{
    Console.WriteLine($"Found {allSuccess.Data.Items.Count} creators");
}
// #endregion

// #region Page-Based Pagination
// Get first page
var firstPage = await apiClient.Creators
    .WithPageIndex(1)
    .ExecuteAsync(resultsLimit: 50);

if (firstPage is Result<PagedResult<Creator>>.Success firstPageSuccess)
{
    Console.WriteLine($"Page 1: {firstPageSuccess.Data.Items.Count} creators");
    
    // Get second page
    var secondPage = await apiClient.Creators
        .WithPageIndex(2)
        .ExecuteAsync(resultsLimit: 50);
    
    if (secondPage is Result<PagedResult<Creator>>.Success secondPageSuccess)
    {
        Console.WriteLine($"Page 2: {secondPageSuccess.Data.Items.Count} creators");
    }
    
    // Navigate to specific page
    var specificPage = await apiClient.Creators
        .WithPageIndex(5)
        .ExecuteAsync(resultsLimit: 50);
}
// #endregion

await host.StopAsync();
