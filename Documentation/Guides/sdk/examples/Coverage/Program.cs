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

#region PreflightCheckBeforeJobSubmission
async Task<Result<JobStatusCollection>> GenerateWithValidationAsync(
    AirIdentifier checkpointModelParam,
    string promptText)
{
    // Check availability first
    var coverageCheckResult = await sdkClient.Coverage.GetAsync(checkpointModelParam);
    
    if (coverageCheckResult is not Result<ProviderAssetAvailability>.Success coverageCheckSuccess)
    {
        return new Result<JobStatusCollection>.Failure(
            new Error(
                ErrorCode.ResourceUnavailable,
                "All checkpoint models are currently unavailable"
            )
        );
    }
    
    // Model is available, proceed with job submission
    return await sdkClient.Jobs
        .CreateImage()
        .WithAir(checkpointModelParam)
        .WithPositivePrompt(promptText)
        .WithDimensions(1024, 1024)
        .ExecuteAsync();
}
#endregion

// Demonstrate pre-flight check
var preflightResult = await GenerateWithValidationAsync(
    checkpointModel,
    "A serene mountain landscape at sunset");

if (preflightResult is Result<JobStatusCollection>.Success preflightSuccess)
{
    Console.WriteLine($"Job submitted successfully: {preflightSuccess.Data.JobsList.Count} jobs");
}
else if (preflightResult is Result<JobStatusCollection>.Failure preflightFailure)
{
    Console.WriteLine($"Failed to submit job: {preflightFailure.Error.Message}");
}

#region CheckAllResourcesBeforeComplexJob
async Task<bool> ValidateJobResourcesAsync(
    AirIdentifier baseCheckpoint,
    IEnumerable<AirIdentifier> loraModels)
{
    // Combine all resources
    var allResources = loraModels.Prepend(baseCheckpoint).ToArray();
    
    // Check coverage
    var checkResult = await sdkClient.Coverage.GetAsync(allResources);
    
    if (checkResult is not Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success success)
    {
        Console.WriteLine("Failed to check coverage");
        return false;
    }
    
    // Verify all resources are available
    var unavailable = success.Data
        .Where(kvp => kvp.Value.Availability != AvailabilityStatus.Available)
        .Select(kvp => kvp.Key)
        .ToArray();
    
    if (unavailable.Length > 0)
    {
        Console.WriteLine("Unavailable resources:");
        foreach (var resource in unavailable)
        {
            Console.WriteLine($"  - {resource}");
        }
        return false;
    }
    
    return true;
}
#endregion

// Demonstrate multi-resource validation
var loraResources = new[] { loraModel };
var validationPassed = await ValidateJobResourcesAsync(checkpointModelBatch, loraResources);
Console.WriteLine($"Resource validation: {(validationPassed ? "Passed" : "Failed")}");

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

#region CacheCoverageResults
var _coverageCache = new Dictionary<AirIdentifier, (DateTime Checked, bool Available)>();
var _cacheDuration = TimeSpan.FromMinutes(5);

async Task<bool> IsCachedAvailableAsync(AirIdentifier cacheModel)
{
    if (_coverageCache.TryGetValue(cacheModel, out var cached))
    {
        if (DateTime.UtcNow - cached.Checked < _cacheDuration)
        {
            return cached.Available;
        }
    }
    
    var cacheResult = await sdkClient.Coverage.GetAsync(cacheModel);
    
    if (cacheResult is Result<ProviderAssetAvailability>.Success success)
    {
        var isAvailable = success.Data.Availability == AvailabilityStatus.Available;
        _coverageCache[cacheModel] = (DateTime.UtcNow, isAvailable);
        return isAvailable;
    }
    
    return false;
}
#endregion

// Demonstrate cached coverage checking
var cachedIsAvailable1 = await IsCachedAvailableAsync(checkpointModel);
var cachedIsAvailable2 = await IsCachedAvailableAsync(checkpointModel); // Uses cache
Console.WriteLine($"Cached availability check: {cachedIsAvailable1} (second call cached: {cachedIsAvailable2})");

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

#region UseForResourceDiscovery
async Task<IEnumerable<AirIdentifier>> GetAvailableModelsAsync(
    IEnumerable<AirIdentifier> candidates)
{
    var discoveryResult = await sdkClient.Coverage.GetAsync(candidates);
    
    if (discoveryResult is not Result<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>.Success success)
    {
        return Array.Empty<AirIdentifier>();
    }
    
    return success.Data
        .Where(kvp => kvp.Value.Availability == AvailabilityStatus.Available)
        .Select(kvp => kvp.Key)
        .ToArray();
}
#endregion

// Demonstrate resource discovery
var candidateModels = new[] { checkpointModelBatch, loraModel, vaeModel };
var availableModels = await GetAvailableModelsAsync(candidateModels);
Console.WriteLine($"Available models: {availableModels.Count()} out of {candidateModels.Length}");

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
