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
// SDK setup - API token automatically loaded from appsettings.json CivitaiSdk section
var sdkBuilder = Host.CreateApplicationBuilder(args);
sdkBuilder.Services.AddCivitaiSdk(sdkBuilder.Configuration);
var sdkHost = sdkBuilder.Build();
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

#region UsageCache
public class UsageCache
{
    public required CivitaiSharp.Sdk.Models.Usage.ConsumptionDetails Data { get; set; }
    public DateTime LastUpdate { get; set; }
}
#endregion
