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

#region SearchByUsername
var searchResult = await apiClient.Creators
    .WhereName("johndoe")
    .ExecuteAsync(resultsLimit: 20);

if (searchResult is Result<PagedResult<Creator>>.Success success)
{
    foreach (var creator in success.Data.Items)
    {
        Console.WriteLine($"{creator.Username}: {creator.ModelCount ?? 0} models");
    }
}
#endregion

#region ListAllCreators
var allCreators = await apiClient.Creators
    .ExecuteAsync(resultsLimit: 50);

if (allCreators is Result<PagedResult<Creator>>.Success allSuccess)
{
    Console.WriteLine($"Found {allSuccess.Data.Items.Count} creators");
}
#endregion

#region PageBasedPagination
var firstPage = await apiClient.Creators
    .WithPageIndex(1)
    .ExecuteAsync(resultsLimit: 50);

if (firstPage is Result<PagedResult<Creator>>.Success firstPageSuccess)
{
    Console.WriteLine($"Page 1: {firstPageSuccess.Data.Items.Count} creators");
}
#endregion

#region AccessingCreatorInformation
if (allCreators is Result<PagedResult<Creator>>.Success creatorInfoSuccess)
{
    foreach (var creator in creatorInfoSuccess.Data.Items)
    {
        Console.WriteLine($"{creator.Username}: {creator.ModelCount ?? 0} models");
    }
}
#endregion

#region WorkingWithModelCreators
var models = await apiClient.Models
    .WhereUsername("johndoe")
    .ExecuteAsync();

if (models is Result<PagedResult<Model>>.Success modelSuccess)
{
    foreach (var model in modelSuccess.Data.Items)
    {
        if (model.Creator is { } creator)
        {
            Console.WriteLine($"{model.Name} by {creator.Username}");
            Console.WriteLine($"  Creator has {creator.ModelCount ?? 0} models");
        }
    }
}
#endregion

#region FindPopularCreators
var topCreators = await apiClient.Creators
    .ExecuteAsync(resultsLimit: 50);

if (topCreators is Result<PagedResult<Creator>>.Success topSuccess)
{
    foreach (var creator in topSuccess.Data.Items)
    {
        Console.WriteLine($"{creator.Username}: {creator.ModelCount ?? 0} models");
    }
}
#endregion

#region SearchForSpecificCreator
var searchSpecific = await apiClient.Creators
    .WhereName("artist")
    .ExecuteAsync(resultsLimit: 50);
#endregion

#region GetAllModelsByCreator
var creatorModels = await apiClient.Models
    .WhereUsername("artistname")
    .OrderBy(ModelSort.Newest)
    .ExecuteAsync(resultsLimit: 100);

if (creatorModels is Result<PagedResult<Model>>.Success creatorModelSuccess)
{
    Console.WriteLine($"Found {creatorModelSuccess.Data.Items.Count} models");
    
    foreach (var model in creatorModelSuccess.Data.Items)
    {
        Console.WriteLine($"  {model.Name}");
        Console.WriteLine($"    Downloads: {model.Stats?.DownloadCount ?? 0}");
    }
}
#endregion

#region HandleCreatorEndpointUnreliability
var unreliableResult = await apiClient.Creators.ExecuteAsync(resultsLimit: 10);

if (!unreliableResult.IsSuccess)
{
    Console.WriteLine("Creator data unavailable, using fallback");
}
#endregion

await host.StopAsync();
