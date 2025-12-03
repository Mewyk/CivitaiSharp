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

// #region by-type
// Query LoRA models only
var loraResult = await apiClient.Models
    .WhereType(ModelType.Lora)
    .WithResultsLimit(10)
    .ExecuteAsync();

if (loraResult is Result<PagedResult<Model>>.Success success)
{
    foreach (var model in success.Data.Items)
    {
        Console.WriteLine($"{model.Name} - {model.Type}");
    }
}
// #endregion by-type

// #region by-tag
// Query models with a specific tag
var animeResult = await apiClient.Models
    .WhereTag("anime")
    .WhereType(ModelType.Lora)
    .WithResultsLimit(10)
    .ExecuteAsync();

if (animeResult is Result<PagedResult<Model>>.Success animeSuccess)
{
    Console.WriteLine($"Found {animeSuccess.Data.Items.Count} anime LoRAs");
}
// #endregion by-tag

// #region by-creator
// Query models by a specific creator
var creatorResult = await apiClient.Models
    .WhereUsername("civitai")
    .WithResultsLimit(10)
    .ExecuteAsync();

if (creatorResult is Result<PagedResult<Model>>.Success creatorSuccess)
{
    Console.WriteLine($"Found {creatorSuccess.Data.Items.Count} models by civitai");
}
// #endregion by-creator

// #region by-name
// Search models by name
var searchResult = await apiClient.Models
    .WhereName("realistic")
    .WhereType(ModelType.Checkpoint)
    .WithResultsLimit(10)
    .ExecuteAsync();

if (searchResult is Result<PagedResult<Model>>.Success searchSuccess)
{
    Console.WriteLine($"Found {searchSuccess.Data.Items.Count} realistic checkpoints");
}
// #endregion by-name

// #region by-id
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
// #endregion by-id

// #region by-version-id
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
// #endregion by-version-id

// #region by-version-hash
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
// #endregion by-version-hash

// #region permissions
// Filter by usage permissions
var commercialFriendly = await apiClient.Models
    .WhereType(ModelType.Lora)
    .WhereAllowNoCredit(true)
    .WhereAllowDerivatives(true)
    .WhereAllowDifferentLicenses(true)
    .WhereCommercialUse(CommercialUsePermission.Sell)
    .WithResultsLimit(10)
    .ExecuteAsync();

if (commercialFriendly is Result<PagedResult<Model>>.Success permSuccess)
{
    Console.WriteLine($"Found {permSuccess.Data.Items.Count} commercially-friendly LoRAs");
    foreach (var model in permSuccess.Data.Items)
    {
        var perms = model.AllowCommercialUse ?? [];
        Console.WriteLine($"  - {model.Name}: {string.Join(", ", perms)}");
    }
}
// #endregion permissions
