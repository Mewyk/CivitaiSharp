using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// This project contains common code patterns that can be referenced from multiple guides
// to avoid duplication across Core, SDK, and Tools documentation examples.

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi();
var host = builder.Build();
await host.StartAsync();

var apiClient = host.Services.GetRequiredService<IApiClient>();

#region CoreBasicSetup
// Minimal Core setup without API key
var minimalBuilder = Host.CreateApplicationBuilder(args);
minimalBuilder.Services.AddCivitaiApi();
var minimalHost = minimalBuilder.Build();
#endregion

#region CoreSetupWithApiKey
// Core setup with API key
var authenticatedBuilder = Host.CreateApplicationBuilder(args);
authenticatedBuilder.Services.AddCivitaiApi(options =>
{
    options.ApiKey = "your-api-key";
});
var authenticatedHost = authenticatedBuilder.Build();
#endregion

#region SdkBasicSetup
// SDK setup (always requires API token)
var sdkBuilder = Host.CreateApplicationBuilder(args);
sdkBuilder.Services.AddCivitaiSdk(options =>
{
    options.ApiToken = "your-api-token";
});
var sdkHost = sdkBuilder.Build();
#endregion

#region ResultPatternMatching
var result = await apiClient.Models
    .WhereType(ModelType.Lora)
    .ExecuteAsync(resultsLimit: 10);

if (result is Result<PagedResult<Model>>.Success success)
{
    foreach (var model in success.Data.Items)
    {
        Console.WriteLine($"{model.Name} - {model.Type}");
    }
}
else if (result is Result<PagedResult<Model>>.Failure failure)
{
    Console.WriteLine($"Error: {failure.Error.Code} - {failure.Error.Message}");
}
#endregion

#region ResultSwitchPattern
var switchResult = await apiClient.Models.GetByIdAsync(123456);

switch (switchResult)
{
    case Result<Model>.Success s:
        Console.WriteLine($"Found: {s.Data.Name}");
        break;
    case Result<Model>.Failure f:
        Console.WriteLine($"Error: {f.Error.Message}");
        break;
}
#endregion

#region CommonAirIdentifiers
// Common AIR identifiers used across examples
var sdxlCheckpoint = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    modelId: 4201,
    versionId: 130072);

var sdxlLora = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Lora,
    AirSource.Civitai,
    modelId: 328553,
    versionId: 368189);
#endregion

await host.StopAsync();
