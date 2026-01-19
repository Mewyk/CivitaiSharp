using System;
using System.Collections.Generic;
using System.Linq;
using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Air;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Setup for BuildFromModelAsync example
var host = Host.CreateApplicationBuilder(args);
host.Services.AddCivitaiApi();
var app = host.Build();
await app.StartAsync();
var apiClient = app.Services.GetRequiredService<IApiClient>();

#region BasicUsage
var builder = new AirBuilder();

var airId = builder
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Lora)
    .WithModelId(328553)
    .WithVersionId(368189)
    .Build();

Console.WriteLine(airId.ToString());
#endregion

#region WithEcosystem
var ecosystemBuilder = new AirBuilder();
ecosystemBuilder.WithEcosystem(AirEcosystem.StableDiffusionXl);
ecosystemBuilder.WithEcosystem(AirEcosystem.Flux1);
ecosystemBuilder.WithEcosystem(AirEcosystem.Pony);
#endregion

#region WithAssetType
var assetBuilder = new AirBuilder();
assetBuilder.WithAssetType(AirAssetType.Lora);
assetBuilder.WithAssetType(AirAssetType.Checkpoint);
assetBuilder.WithAssetType(AirAssetType.Vae);
#endregion

#region WithSource
var sourceBuilder = new AirBuilder();
sourceBuilder.WithSource(AirSource.Civitai);
#endregion

#region WithModelId
var modelIdBuilder = new AirBuilder();
modelIdBuilder.WithModelId(328553);
#endregion

#region WithVersionId
var versionIdBuilder = new AirBuilder();
versionIdBuilder.WithVersionId(368189);
#endregion

#region ResetReuse
var baseBuilder = new AirBuilder()
    .WithEcosystem(AirEcosystem.Flux1)
    .WithAssetType(AirAssetType.Lora);

var fluxLora = baseBuilder
    .WithModelId(123)
    .WithVersionId(456)
    .Build();

// baseBuilder is unchanged, can be reused to build different configurations
var anotherFluxLora = baseBuilder
    .WithModelId(789)
    .WithVersionId(101)
    .Build();
#endregion

#region MethodChaining
var chainedIdentifier = new AirBuilder()
    .WithEcosystem(AirEcosystem.Flux1)
    .WithAssetType(AirAssetType.Lora)
    .WithModelId(123)
    .WithVersionId(456)
    .Build();
#endregion

#region ReuseBuilders
var reuseBase = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl);

var modelData = new[]
{
    (AirAssetType.Lora, 328553L, 368189L),
    (AirAssetType.Checkpoint, 123456L, 789012L)
};

foreach (var data in modelData)
{
    var reuseAirId = reuseBase
        .WithAssetType(data.Item1)
        .WithModelId(data.Item2)
        .WithVersionId(data.Item3)
        .Build();
    
    Console.WriteLine(reuseAirId);
}
#endregion

#region BatchBuilding
var batchBuilder = new AirBuilder();
var identifiers = new List<AirIdentifier>();

(AirEcosystem, AirAssetType, long, long)[] batchData = [
    (AirEcosystem.StableDiffusionXl, AirAssetType.Lora, 328553L, 368189L),
    (AirEcosystem.Flux1, AirAssetType.Checkpoint, 123456L, 789012L),
    (AirEcosystem.Pony, AirAssetType.Lora, 111111L, 222222L)
];

foreach (var (ecosystem, assetType, modelId, versionId) in batchData)
{
    var batchAirId = batchBuilder
        .WithEcosystem(ecosystem)
        .WithAssetType(assetType)
        .WithModelId(modelId)
        .WithVersionId(versionId)
        .Build();

    identifiers.Add(batchAirId);
}
#endregion

#region BuildFromModel
async Task<AirIdentifier?> BuildFromModelAsync(IApiClient apiClient, int modelId)
{
    var result = await apiClient.Models.GetByIdAsync(modelId);
    if (result is not Result<Model>.Success success)
    {
        return null;
    }

    var model = success.Data;
    var version = model.ModelVersions?.FirstOrDefault();

    if (version is null)
    {
        return null;
    }

    var fromModelBuilder = new AirBuilder();
    return fromModelBuilder
        .WithEcosystem(GetEcosystem(version.BaseModel))
        .WithAssetType(GetAssetType(model.Type))
        .WithModelId(model.Id)
        .WithVersionId(version.Id)
        .Build();
}

AirEcosystem GetEcosystem(string baseModel) => baseModel switch
{
    "SD 1.5" => AirEcosystem.StableDiffusion1,
    "SDXL 1.0" => AirEcosystem.StableDiffusionXl,
    "Flux.1" => AirEcosystem.Flux1,
    "Pony" => AirEcosystem.Pony,
    _ => AirEcosystem.StableDiffusion1,
};

AirAssetType GetAssetType(ModelType type) => type switch
{
    ModelType.Checkpoint => AirAssetType.Checkpoint,
    ModelType.Lora => AirAssetType.Lora,
    ModelType.Vae => AirAssetType.Vae,
    ModelType.TextualInversion => AirAssetType.Embedding,
    ModelType.Hypernetwork => AirAssetType.Hypernetwork,
    _ => AirAssetType.Checkpoint,
};
#endregion

// Demonstrate building from Civitai API model data
var airFromModel = await BuildFromModelAsync(apiClient, 328553);
if (airFromModel != null)
{
    Console.WriteLine($"Built AIR identifier from model: {airFromModel}");
}

await app.StopAsync();
