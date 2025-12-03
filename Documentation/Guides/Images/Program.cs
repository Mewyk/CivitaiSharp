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
    .ExecuteAsync(resultsLimit: 10);

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
    .ExecuteAsync(resultsLimit: 10);

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
    .ExecuteAsync(resultsLimit: 10);

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
// Paginate through image results using cursor-based pagination
string? cursor = null;
var allImages = new List<Image>();
const int maxImages = 40;

do
{
    var result = await apiClient.Images
        .WhereModelId(123456)
        .ExecuteAsync(resultsLimit: 20, cursor: cursor);

    if (result is Result<PagedResult<Image>>.Success success)
    {
        allImages.AddRange(success.Data.Items);
        cursor = success.Data.Metadata?.NextCursor;
        Console.WriteLine($"Fetched {success.Data.Items.Count} images. Total: {allImages.Count}");

        if (allImages.Count >= maxImages)
            break;
    }
    else
    {
        break;
    }
}
while (!string.IsNullOrEmpty(cursor));

Console.WriteLine($"Collected {allImages.Count} images total.");
// #endregion pagination
