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

#region ByType
// Query LoRA models only
var loraResult = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync(resultsLimit: 10);

if (loraResult is Result<PagedResult<Model>>.Success success)
{
    foreach (var model in success.Data.Items)
    {
        Console.WriteLine($"{model.Name} - {model.Type}");
    }
}
#endregion

#region ByTag
// Query models with a specific tag
var animeResult = await apiClient.Models
    .WhereTag("anime")
    .WhereType(ModelType.Lora)
    .ExecuteAsync(resultsLimit: 10);

if (animeResult is Result<PagedResult<Model>>.Success animeSuccess)
{
    Console.WriteLine($"Found {animeSuccess.Data.Items.Count} anime LoRAs");
}
#endregion

#region ByCreator
// Query models by a specific creator
var creatorResult = await apiClient.Models
    .WhereUsername("CreatorName")
    .ExecuteAsync(resultsLimit: 10);

if (creatorResult is Result<PagedResult<Model>>.Success creatorSuccess)
{
    Console.WriteLine($"Found {creatorSuccess.Data.Items.Count} models by CreatorName");
}
#endregion

#region ByName
// Search models by name
var searchResult = await apiClient.Models
    .WhereName("realistic")
    .WhereType(ModelType.Checkpoint)
    .ExecuteAsync(resultsLimit: 10);

if (searchResult is Result<PagedResult<Model>>.Success searchSuccess)
{
    Console.WriteLine($"Found {searchSuccess.Data.Items.Count} realistic checkpoints");
}
#endregion

#region ById
// Get a specific model by its ID
var modelResult = await apiClient.Models.GetByIdAsync(123456);

if (modelResult is Result<Model>.Success modelSuccess)
{
    var model = modelSuccess.Data;
    Console.WriteLine($"Name: {model.Name}");
    Console.WriteLine($"Type: {model.Type}");
    Console.WriteLine($"Creator: {model.Creator?.Username}");
    Console.WriteLine($"Downloads: {model.Stats?.DownloadCount}");
    Console.WriteLine($"Versions: {model.ModelVersions?.Count}");
}
else if (modelResult is Result<Model>.Failure failure)
{
    Console.WriteLine($"Model not found: {failure.Error.Message}");
}
#endregion

#region ByVersionId
// Get a specific model version by its version ID
var versionResult = await apiClient.Models.GetByVersionIdAsync(130072);

if (versionResult is Result<ModelVersion>.Success versionSuccess)
{
    var version = versionSuccess.Data;
    Console.WriteLine($"Version: {version.Name}");
    Console.WriteLine($"Base Model: {version.BaseModel}");
    Console.WriteLine($"AIR: {version.AirIdentifier}");
    Console.WriteLine($"Parent Model: {version.Model?.Name}");
    Console.WriteLine($"Downloads: {version.Stats?.DownloadCount}");
}
else if (versionResult is Result<ModelVersion>.Failure versionFailure)
{
    Console.WriteLine($"Version not found: {versionFailure.Error.Message}");
}
#endregion

#region ByVersionHash
// Get a model version by one of its file hashes (SHA256, AutoV2, CRC32, etc.)
var hashResult = await apiClient.Models.GetByVersionHashAsync("ABC123DEF456");

if (hashResult is Result<ModelVersion>.Success hashSuccess)
{
    var version = hashSuccess.Data;
    Console.WriteLine($"Found: {version.Model?.Name} - {version.Name}");
    Console.WriteLine($"AIR: {version.AirIdentifier}");
    Console.WriteLine($"Base Model: {version.BaseModel}");

    // Access file information
    foreach (var file in version.Files ?? [])
    {
        Console.WriteLine($"  File: {file.Name}");
        if (file.Hashes is { } hashes)
        {
            Console.WriteLine($"    SHA256: {hashes.Sha256}");
            Console.WriteLine($"    AutoV2: {hashes.AutoV2}");
        }
    }
}
else if (hashResult is Result<ModelVersion>.Failure hashFailure)
{
    Console.WriteLine($"Hash not found: {hashFailure.Error.Message}");
}
#endregion

#region Permissions
// Filter by usage permissions
var commercialFriendly = await apiClient.Models
    .WhereType(ModelType.Lora)
    .WhereAllowNoCredit(true)
    .WhereAllowDerivatives(true)
    .WhereAllowDifferentLicenses(true)
    .WhereCommercialUse(CommercialUsePermission.Sell)
    .ExecuteAsync(resultsLimit: 10);

if (commercialFriendly is Result<PagedResult<Model>>.Success permSuccess)
{
    Console.WriteLine($"Found {permSuccess.Data.Items.Count} commercially-friendly LoRAs");
    foreach (var model in permSuccess.Data.Items)
    {
        var perms = model.AllowCommercialUse ?? [];
        Console.WriteLine($"  - {model.Name}: {string.Join(", ", perms)}");
    }
}
#endregion

#region WorkingWithVersions
var modelForVersions = await apiClient.Models.GetByIdAsync(123456);

if (modelForVersions is Result<Model>.Success successVersions)
{
    var modelWithVersions = successVersions.Data;

    foreach (var version in modelWithVersions.ModelVersions ?? [])
    {
        Console.WriteLine($"Version: {version.Name} (ID: {version.Id})");
        Console.WriteLine($"  Base Model: {version.BaseModel}");
        Console.WriteLine($"  Published: {version.PublishedAt}");
        Console.WriteLine($"  Status: {version.Status}");
        Console.WriteLine($"  Downloads: {version.Stats?.DownloadCount}");

        // Trigger words for generation
        if (version.TrainedWords is { } words && words.Count > 0)
        {
            Console.WriteLine($"  Trigger Words: {string.Join(", ", words)}");
        }

        // Files included in this version
        foreach (var file in version.Files ?? [])
        {
            Console.WriteLine($"  File: {file.Name} ({file.SizeKilobytes} KB)");
            Console.WriteLine($"    Type: {file.Type}");
            Console.WriteLine($"    Primary: {file.Primary}");
        }
    }
}
#endregion

#region VersionSpecificInformation
var modelForVersionInfo = await apiClient.Models.GetByIdAsync(123456);

if (modelForVersionInfo is Result<Model>.Success successVersionInfo)
{
    var modelWithVersionInfo = successVersionInfo.Data;
    var version = modelWithVersionInfo.ModelVersions?.FirstOrDefault();
    if (version is not null)
    {
        // Training information
        if (version.TrainingStatus is { } status)
        {
            Console.WriteLine($"Training Status: {status}");
        }

        // Early access restrictions
        if (version.EarlyAccessEndsAt is { } earlyAccess)
        {
            Console.WriteLine($"Early Access Until: {earlyAccess}");
        }

        // AIR identifier for generation
        if (version.AirIdentifier is { } air)
        {
            Console.WriteLine($"AIR: {air}");
        }

        // Version statistics
        if (version.Stats is { } stats)
        {
            Console.WriteLine($"Downloads: {stats.DownloadCount}");
            Console.WriteLine($"Thumbs Up: {stats.ThumbsUpCount}");
            Console.WriteLine($"Thumbs Down: {stats.ThumbsDownCount}");
        }
    }
}
#endregion

#region ModelStatistics
var modelForStats = await apiClient.Models.GetByIdAsync(123456);

if (modelForStats is Result<Model>.Success successStats)
{
    var modelWithStats = successStats.Data;
    if (modelWithStats.Stats is { } stats)
    {
        Console.WriteLine($"Downloads: {stats.DownloadCount}");
        Console.WriteLine($"Thumbs Up: {stats.ThumbsUpCount}");
        Console.WriteLine($"Thumbs Down: {stats.ThumbsDownCount}");
        Console.WriteLine($"Comments: {stats.CommentCount}");
        Console.WriteLine($"Tips Amount: {stats.TippedAmountCount}");
    }
}
#endregion

#region Sorting
var topRated = await apiClient.Models
    .OrderBy(ModelSort.HighestRated)
    .WherePeriod(TimePeriod.Month)
    .ExecuteAsync(resultsLimit: 10);

if (topRated is Result<PagedResult<Model>>.Success topSuccess)
{
    Console.WriteLine($"Found {topSuccess.Data.Items.Count} top-rated models");
}
#endregion

#region Favorites
// Query favorited models (requires API key)
var favorites = await apiClient.Models
    .WhereFavorites()
    .ExecuteAsync();

if (favorites is Result<PagedResult<Model>>.Success favSuccess)
{
    Console.WriteLine($"You have {favSuccess.Data.Items.Count} favorited models");
    foreach (var model in favSuccess.Data.Items)
    {
        Console.WriteLine($"  - {model.Name}");
    }
}
#endregion

#region HiddenModels
// Query hidden models (requires API key)
var hidden = await apiClient.Models
    .WhereHidden()
    .ExecuteAsync();

if (hidden is Result<PagedResult<Model>>.Success hiddenSuccess)
{
    Console.WriteLine($"You have hidden {hiddenSuccess.Data.Items.Count} models");
    foreach (var model in hiddenSuccess.Data.Items)
    {
        Console.WriteLine($"  - {model.Name}");
    }
}
#endregion

await host.StopAsync();
