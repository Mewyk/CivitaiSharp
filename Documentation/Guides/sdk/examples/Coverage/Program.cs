using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Extensions;
using CivitaiSharp.Sdk.Models.Coverage;
using CivitaiSharp.Sdk.Models.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiSdk(builder.Configuration);
var host = builder.Build();
await host.StartAsync();

var sdkClient = host.Services.GetRequiredService<ISdkClient>();
var cancellationToken = CancellationToken.None;

#region CheckSingleModelAvailability
var checkpointModel = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    modelId: 4201,
    versionId: 130072);

var checkpointModelFromBuilder = new AirBuilder()
    .WithEcosystem(AirEcosystem.StableDiffusionXl)
    .WithAssetType(AirAssetType.Checkpoint)
    .WithSource(AirSource.Civitai)
    .WithModelId(4201)
    .WithVersionId(130072)
    .Build();

var coverageResult = await sdkClient.Coverage.GetAsync(checkpointModel, cancellationToken);

if (coverageResult is Result<ProviderAssetAvailability>.Success coverageSuccess)
{
    Console.WriteLine($"Availability: {coverageSuccess.Data.Availability}");
    Console.WriteLine($"Workers: {coverageSuccess.Data.Workers}");
    
    if (coverageSuccess.Data.Availability == AvailabilityStatus.Available)
    {
        Console.WriteLine($"Model is available with {coverageSuccess.Data.Workers} workers");
    }
}
#endregion

#region CheckMultipleModels
var checkpointModelBatch = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    modelId: 4201,
    versionId: 130072);

var loraModel = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Lora,
    AirSource.Civitai,
    modelId: 328553,
    versionId: 368189);

var vaeModel = new AirIdentifier(
    AirEcosystem.StableDiffusion1,
    AirAssetType.Vae,
    AirSource.Civitai,
    modelId: 22354,
    versionId: 123456);

var modelsToCheck = new[] { checkpointModelBatch, loraModel, vaeModel };

var batchCoverageResult = await sdkClient.Coverage.GetAsync(modelsToCheck, cancellationToken);

if (batchCoverageResult is Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success batchSuccess)
{
    foreach (var (modelIdentifier, availability) in batchSuccess.Data)
    {
        Console.WriteLine($"{modelIdentifier}: {availability.Availability} (Workers: {availability.Workers})");
    }
}
#endregion

#region ErrorHandling
var errorHandlingModel = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    modelId: 4201,
    versionId: 130072);

var errorResult = await sdkClient.Coverage.GetAsync(errorHandlingModel);

switch (errorResult)
{
    case Result<ProviderAssetAvailability>.Success success:
        if (success.Data.Availability == AvailabilityStatus.Available)
        {
            Console.WriteLine($"Model is available ({success.Data.Workers} workers)");
        }
        else
        {
            Console.WriteLine($"Model status: {success.Data.Availability}");
        }
        break;
        
    case Result<ProviderAssetAvailability>.Failure failure:
        Console.WriteLine($"Error: {failure.Error.Message}");
        // Proceed anyway - coverage check is optional
        break;
}
#endregion

#region BatchChecksWhenPossible
var baseModel = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    modelId: 4201,
    versionId: 130072);

var loras = new[]
{
    new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Lora, AirSource.Civitai, 328553, 368189),
    new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Lora, AirSource.Civitai, 123456, 789012)
};

var allResources = new[] { baseModel }.Concat(loras);
await sdkClient.Coverage.GetAsync(allResources);

foreach (var resource in allResources)
{
    await sdkClient.Coverage.GetAsync(resource);
}
#endregion

#region AvailabilityStatusChecking
// Check if a model is available and get status
var availabilityModel = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    modelId: 4201,
    versionId: 130072);

var availabilityResult = await sdkClient.Coverage.GetAsync(availabilityModel);

if (availabilityResult is Result<ProviderAssetAvailability>.Success availabilitySuccess)
{
    var status = availabilitySuccess.Data.Availability;
    
    switch (status)
    {
        case AvailabilityStatus.Available:
            Console.WriteLine($"Model is ready ({availabilitySuccess.Data.Workers} workers)");
            break;
        case AvailabilityStatus.Unavailable:
            Console.WriteLine("Model is not available");
            break;
        default:
            Console.WriteLine($"Model status: {status}");
            break;
    }
}
#endregion

Console.WriteLine("Coverage examples completed successfully.");

await host.StopAsync();
