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

// #region by-model
// Find images generated with a specific model
var imagesByModel = await apiClient.Images
    .WhereModelId(123456)
    .WithResultsLimit(10)
    .ExecuteAsync();

if (imagesByModel is Result<PagedResult<Image>>.Success success)
{
    foreach (var image in success.Data.Items)
    {
        Console.WriteLine($"Image {image.Id}: {image.Width}x{image.Height}");
    }
}
// #endregion by-model

// #region by-version
// Find images generated with a specific model version
var imagesByVersion = await apiClient.Images
    .WhereModelVersionId(789012)
    .WithResultsLimit(10)
    .ExecuteAsync();

if (imagesByVersion is Result<PagedResult<Image>>.Success versionSuccess)
{
    foreach (var image in versionSuccess.Data.Items)
    {
        Console.WriteLine($"Image {image.Id} by {image.Username}");
    }
}
// #endregion by-version

// #region by-creator
// Find images posted by a specific user
var imagesByCreator = await apiClient.Images
    .WhereUsername("civitai")
    .OrderBy(ImageSort.MostReactions)
    .WherePeriod(TimePeriod.Week)
    .WithResultsLimit(10)
    .ExecuteAsync();

if (imagesByCreator is Result<PagedResult<Image>>.Success creatorSuccess)
{
    foreach (var image in creatorSuccess.Data.Items)
    {
        var reactions = image.Stats?.LikeCount + image.Stats?.HeartCount;
        Console.WriteLine($"Image {image.Id}: {reactions} reactions");
    }
}
// #endregion by-creator

// #region pagination
// Paginate through image results using page index
var page1 = await apiClient.Images
    .WhereModelId(123456)
    .WithPageIndex(1)
    .WithResultsLimit(20)
    .ExecuteAsync();

if (page1 is Result<PagedResult<Image>>.Success page1Success)
{
    Console.WriteLine($"Page 1: {page1Success.Data.Items.Count} images");

    // Get the next page
    var page2 = await apiClient.Images
        .WhereModelId(123456)
        .WithPageIndex(2)
        .WithResultsLimit(20)
        .ExecuteAsync();

    if (page2 is Result<PagedResult<Image>>.Success page2Success)
    {
        Console.WriteLine($"Page 2: {page2Success.Data.Items.Count} images");
    }
}
// #endregion pagination
