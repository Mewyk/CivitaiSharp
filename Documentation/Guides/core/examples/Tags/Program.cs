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

#region ListAllTags
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

#region SearchByName
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

#region UsingTagsWithModels
var animeModels = await apiClient.Models
    .WhereTag("anime")
    .ExecuteAsync(resultsLimit: 50);

if (animeModels is Result<PagedResult<Model>>.Success success)
{
    foreach (var model in success.Data.Items)
    {
        Console.WriteLine($"{model.Name}");
        
        if (model.Tags is { } tags)
        {
            Console.WriteLine($"  Tags: {string.Join(", ", tags)}");
        }
    }
}
#endregion

#region ModelTagArrays
// Example from a model instance
if (animeModels is Result<PagedResult<Model>>.Success modelSuccess && modelSuccess.Data.Items.Count > 0)
{
    var model = modelSuccess.Data.Items[0];
    if (model.Tags is { } tags)
    {
        foreach (var tag in tags)
        {
            Console.WriteLine($"  - {tag}");
        }
    }
}
#endregion

#region FindPopularTags
var popularTags = await apiClient.Tags
    .ExecuteAsync(resultsLimit: 100);

if (popularTags is Result<PagedResult<Tag>>.Success popularSuccess)
{
    Console.WriteLine("First 100 Tags:");
    foreach (var tag in popularSuccess.Data.Items)
    {
        Console.WriteLine($"  {tag.Name}");
    }
}
#endregion

#region SearchRelatedTags
var characterTags = await apiClient.Tags
    .WhereName("character")
    .ExecuteAsync(resultsLimit: 50);
#endregion

await host.StopAsync();
