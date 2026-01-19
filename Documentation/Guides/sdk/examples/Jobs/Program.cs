using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Extensions;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Models.Results;
using CivitaiSharp.Sdk.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiSdk(builder.Configuration);
var host = builder.Build();
await host.StartAsync();

var sdkClient = host.Services.GetRequiredService<ISdkClient>();

#region BasicImageGeneration
var result = await sdkClient.Jobs
    .CreateImage()
    .WithAir(new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, AirSource.Civitai, 4201, 130072))
    .WithPositivePrompt("a beautiful sunset over mountains")
    .WithNegativePrompt("blurry, low quality")
    .WithScheduler(Scheduler.EulerAncestral)
    .WithDimensions(1024, 1024)
    .WithSteps(30)
    .WithConfigurationScale(7.5m)
    .ExecuteAsync();

if (result is Result<JobStatusCollection>.Success success)
{
    Console.WriteLine($"Job submitted with token: {success.Data.Token}");
    foreach (var job in success.Data.JobsList)
    {
        Console.WriteLine($"Job ID: {job.JobId}, Cost: {job.Cost}");
    }
}
#endregion

#region AdvancedConfiguration
var advancedModel = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, AirSource.Civitai, 4201, 130072);

var advancedResult = await sdkClient.Jobs
    .CreateImage()
    .WithAir(advancedModel)
    .WithPositivePrompt("detailed portrait")
    .WithScheduler(Scheduler.DpmPlusPlus2MSdeKarras)
    .WithSeed(12345)
    .WithSteps(50)
    .WithConfigurationScale(8.5m)
    .WithQuantity(4)  // Generate 4 images
    .WithClipSkip(2)
    .WithCallbackUrl("https://example.tld/webhook")
    .WithRetries(3)
    .ExecuteAsync();
#endregion

#region UsingAdditionalNetworks
var baseModel = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, AirSource.Civitai, 4201, 130072);
var lora = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Lora, AirSource.Civitai, 123456, 789);
var anotherLora = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Lora, AirSource.Civitai, 234567, 890);

var loraResult = await sdkClient.Jobs
    .CreateImage()
    .WithAir(baseModel)
    .WithPositivePrompt("character portrait")
    .WithAdditionalNetwork(lora, NetworkBuilder.Create()
        .WithStrength(0.8m)
        .WithTriggerWord("character")
        .Build())
    .WithAdditionalNetwork(anotherLora, NetworkBuilder.Create()
        .WithStrength(0.5m)
        .Build())
    .ExecuteAsync();
#endregion

#region UsingControlNet
var controlNetModel = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, AirSource.Civitai, 4201, 130072);

var controlNetResult = await sdkClient.Jobs
    .CreateImage()
    .WithAir(controlNetModel)
    .WithPositivePrompt("person standing")
    .WithControlNet(ControlNetBuilder.Create()
        .WithImageUrl("https://example.tld/pose.png")
        .WithPreprocessor(ControlNetPreprocessor.Canny)
        .WithWeight(1.0m)
        .WithStartStep(0.0m)
        .WithEndStep(1.0m)
        .Build())
    .ExecuteAsync();
#endregion

#region BatchJobSubmission
var firstCheckpoint = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, AirSource.Civitai, 4201, 130072);
var secondCheckpoint = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, AirSource.Civitai, 101055, 128078);

var landscapeJob = sdkClient.Jobs
    .CreateImage()
    .WithAir(firstCheckpoint)
    .WithPositivePrompt("landscape");

var portraitJob = sdkClient.Jobs
    .CreateImage()
    .WithAir(secondCheckpoint)
    .WithPositivePrompt("portrait");

var batchResult = await landscapeJob.ExecuteBatchAsync([portraitJob]);

if (batchResult is Result<JobStatusCollection>.Success batchSuccess)
{
    Console.WriteLine($"Batch submitted with token: {batchSuccess.Data.Token}");
}
#endregion

#region CompleteParameterExample
var baseCheckpoint = new AirIdentifier(AirEcosystem.StableDiffusionXl, AirAssetType.Checkpoint, AirSource.Civitai, 4201, 130072);

var fullParameterJob = await sdkClient.Jobs
    .CreateImage()
    .WithAir(baseCheckpoint)
    .WithPositivePrompt("masterpiece, best quality, professional photograph, cyberpunk street scene, neon lights, rain, reflections, highly detailed, 8k uhd")
    .WithNegativePrompt("blurry, low quality, bad anatomy, deformed, watermark, signature, text, jpeg artifacts, worst quality, low resolution")
    .WithScheduler(Scheduler.DpmPlusPlus2MSdeKarras)
    .WithDimensions(1024, 1536) // Portrait orientation
    .WithSteps(40) // Higher steps for quality
    .WithConfigurationScale(8.5m) // Strong prompt adherence
    .WithSeed(987654321) // Reproducible results
    .WithClipSkip(2) // Better artistic interpretation
    .WithQuantity(4) // Generate 4 variations
    .WithPriority(Priority.Default)
    .WithCallbackUrl("https://example.tld/webhook/generation-complete") // Async notification
    .WithRetries(3) // Retry failed jobs
    .WithTimeout(TimeSpan.FromMinutes(15)) // 15-minute timeout
    .ExecuteAsync();

if (fullParameterJob is Result<JobStatusCollection>.Success jobSuccess)
{
    Console.WriteLine($"Submitted {jobSuccess.Data.JobsList.Count} jobs");
    Console.WriteLine($"Batch token: {jobSuccess.Data.Token}");
}
#endregion

#region GetJobById
var jobId = Guid.Parse("12345678-1234-1234-1234-1234567890ab");
var jobResult = await sdkClient.Jobs.Query.GetByIdAsync(jobId);

if (jobResult is Result<JobStatus>.Success jobStatus)
{
    Console.WriteLine($"Status: {jobStatus.Data.Status}");
    Console.WriteLine($"Job ID: {jobStatus.Data.JobId}");
}
#endregion

#region GetJobsByToken
var batchToken = "your-batch-token-here";
var tokenResult = await sdkClient.Jobs.Query.GetByTokenAsync(batchToken);

if (tokenResult is Result<JobStatusCollection>.Success tokenSuccess)
{
    foreach (var job in tokenSuccess.Data.JobsList)
    {
        Console.WriteLine($"Job {job.JobId}: {job.Status}");
    }
}
#endregion

#region QueryWithOptions
var detailedResult = await sdkClient.Jobs.Query
    .WithDetailed()  // Include original job specifications
    .WithWait()      // Wait for jobs to complete (up to ~10 minutes)
    .GetByTokenAsync(batchToken);
#endregion

#region FilterByProperties
var filteredResult = await sdkClient.Jobs.Query
    .WhereProperty("userId", "12345")
    .WhereProperty("campaign", "winter-2026")
    .ExecuteAsync();
#endregion

#region CancelJob
var jobToCancel = Guid.Parse("12345678-1234-1234-1234-1234567890ab");
var cancelResult = await sdkClient.Jobs.Query.CancelAsync(jobToCancel);

if (cancelResult is Result<Unit>.Success)
{
    Console.WriteLine("Job cancelled successfully");
}
#endregion

#region CancelBatchJobs
var batchToCancel = "batch-token-to-cancel";
var batchCancelResult = await sdkClient.Jobs.Query.CancelAsync(batchToCancel);
#endregion

#region TaintJob
var jobToTaint = Guid.Parse("12345678-1234-1234-1234-1234567890ab");
await sdkClient.Jobs.Query.TaintAsync(jobToTaint);
#endregion

Console.WriteLine("Jobs examples completed successfully.");

await host.StopAsync();
