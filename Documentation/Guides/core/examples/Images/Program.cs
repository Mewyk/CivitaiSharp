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

#region ByModel
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
#endregion

#region ByVersion
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
#endregion

#region ByCreator
// Find images posted by a specific user
var imagesByCreator = await apiClient.Images
    .WhereUsername("Mewyk")
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
#endregion

#region ByPost
// Find all images in a specific post
var imagesByPost = await apiClient.Images
    .WherePostId(12345)
    .ExecuteAsync();

if (imagesByPost is Result<PagedResult<Image>>.Success postSuccess)
{
    Console.WriteLine($"Found {postSuccess.Data.Items.Count} images in post");
}
#endregion

#region GenerationMetadata
var imagesWithMeta = await apiClient.Images
    .WhereModelId(123456)
    .ExecuteAsync(resultsLimit: 1);

if (imagesWithMeta is Result<PagedResult<Image>>.Success metaSuccess && metaSuccess.Data.Items.Count > 0)
{
    var image = metaSuccess.Data.Items[0];
    if (image.Meta is { } meta)
    {
        Console.WriteLine($"Prompt: {meta.PositivePrompt}");
        Console.WriteLine($"Steps: {meta.Steps}, CFG: {meta.ConfigurationScale}, Seed: {meta.Seed}");
        Console.WriteLine($"Sampler: {meta.Sampler}, Model: {meta.ModelName}");
    }
}
#endregion

#region NsfwFiltering
var safeImages = await apiClient.Images
    .WhereNsfwLevel(ImageNsfwLevel.None)
    .ExecuteAsync(resultsLimit: 10);

if (safeImages is Result<PagedResult<Image>>.Success safeSuccess)
{
    Console.WriteLine($"Found {safeSuccess.Data.Items.Count} safe images");
}
#endregion

#region SortingImages
var mostReacted = await apiClient.Images
    .OrderBy(ImageSort.MostReactions)
    .WherePeriod(TimePeriod.Week)
    .ExecuteAsync(resultsLimit: 20);

if (mostReacted is Result<PagedResult<Image>>.Success reactedSuccess)
{
    Console.WriteLine($"Found {reactedSuccess.Data.Items.Count} most reacted images");
}
#endregion

#region ImageStatistics
var imagesWithStats = await apiClient.Images
    .WhereModelId(123456)
    .ExecuteAsync(resultsLimit: 1);

if (imagesWithStats is Result<PagedResult<Image>>.Success statsSuccess && statsSuccess.Data.Items.Count > 0)
{
    var image = statsSuccess.Data.Items[0];
    if (image.Stats is { } stats)
    {
        Console.WriteLine($"Reactions - Likes: {stats.LikeCount}, Hearts: {stats.HeartCount}");
        Console.WriteLine($"Comments: {stats.CommentCount}");
    }
}
#endregion

#region DownloadingImages
var imageToDownload = await apiClient.Images
    .WhereModelId(123456)
    .ExecuteAsync(resultsLimit: 1);

if (imageToDownload is Result<PagedResult<Image>>.Success downloadSuccess && downloadSuccess.Data.Items.Count > 0)
{
    var image = downloadSuccess.Data.Items[0];
    using var httpClient = new HttpClient();
    var imageBytes = await httpClient.GetByteArrayAsync(image.Url);
    await File.WriteAllBytesAsync($"{image.Id}.png", imageBytes);
    Console.WriteLine($"Downloaded image {image.Id}");
}
#endregion

await host.StopAsync();
