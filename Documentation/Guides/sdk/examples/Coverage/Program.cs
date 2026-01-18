using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Extensions;
using CivitaiSharp.Sdk.Models.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCivitaiSdk(options =>
{
    options.ApiToken = builder.Configuration["Civitai:ApiToken"]!;
});

var host = builder.Build();
await host.StartAsync();

var sdkClient = host.Services.GetRequiredService<ISdkClient>();
var cancellationToken = CancellationToken.None;

#region CheckSingleModelAvailability
// Create AIR identifier using constructor
var checkpointModel = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    modelId: 4201,
    versionId: 130072);

// Or use the builder pattern for more flexibility
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

#region AvailabilityStatusChecking
// Complete example showing availability status checking
var model = new AirIdentifier(
    AirEcosystem.StableDiffusionXl,
    AirAssetType.Checkpoint,
    AirSource.Civitai,
    modelId: 4201,
    versionId: 130072);

var coverage = await sdkClient.Coverage.GetAsync(model);

if (coverage is Result<ProviderAssetAvailability>.Success result)
{
    // Check overall availability
    switch (result.Data.Availability)
    {
        case AvailabilityStatus.Available:
            Console.WriteLine("Model is ready for generation");
            Console.WriteLine($"Workers available: {result.Data.Workers}");
            break;
            
        case AvailabilityStatus.Degraded:
            Console.WriteLine("Model available but with limited capacity");
            Console.WriteLine($"Workers available: {result.Data.Workers}");
            Console.WriteLine("Expect longer queue times");
            break;
            
        case AvailabilityStatus.Unavailable:
            Console.WriteLine("Model is not available");
            Console.WriteLine("No workers currently have this model loaded");
            break;
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

#region SelectProviderBasedOnQueueDepth
async Task<bool> CheckModelAvailabilityAsync(AirIdentifier checkModel)
{
    var checkAvailabilityResult = await sdkClient.Coverage.GetAsync(checkModel);
    
    if (checkAvailabilityResult is not Result<ProviderAssetAvailability>.Success success)
    {
        return false;
    }
    
    // Check availability and worker count
    if (success.Data.Availability == AvailabilityStatus.Available && success.Data.Workers > 0)
    {
        Console.WriteLine($"Model available with {success.Data.Workers} workers");
        return true;
    }
    else if (success.Data.Availability == AvailabilityStatus.Degraded)
    {
        Console.WriteLine($"Model available but degraded ({success.Data.Workers} workers)");
        return true;
    }
    
    Console.WriteLine("Model not available");
    return false;
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

#region BatchChecksWhenPossible
// Good - single API call
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

// Less efficient - multiple API calls
foreach (var resource in allResources)
{
    await sdkClient.Coverage.GetAsync(resource);
}
#endregion

#region MakeCoverageOptional
async Task<Result<JobStatusCollection>> GenerateAsync(
    AirIdentifier generateModel,
    string prompt,
    bool validateCoverage = false)
{
    if (validateCoverage)
    {
        var coverageValidationResult = await sdkClient.Coverage.GetAsync(generateModel);
        if (coverageValidationResult is Result<ProviderAssetAvailability>.Success success &&
            success.Data.Availability != AvailabilityStatus.Available)
        {
            return new Result<JobStatusCollection>.Failure(
                Error.Create(ErrorCode.ResourceUnavailable, "Model not available")
            );
        }
    }
    
    return await sdkClient.Jobs
        .CreateImage()
        .WithAir(generateModel)
        .WithPositivePrompt(prompt)
        .ExecuteAsync();
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

Console.WriteLine("Coverage examples completed successfully.");

await host.StopAsync();
