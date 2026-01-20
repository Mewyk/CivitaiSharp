namespace CivitaiSharp.Sdk.Extensions;

using System.Collections.Generic;
using System.Threading;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// Registers SDK-specific enum mappings with the Core's <see cref="ApiStringRegistry"/>.
/// Called automatically when <see cref="ServiceCollectionExtensions.AddCivitaiSdk(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action{Core.SdkOptions})"/> is invoked.
/// </summary>
/// <remarks>
/// <para>
/// This registry eliminates the need for reflection when converting enums to API strings,
/// making the library compatible with AOT compilation and trimming.
/// </para>
/// </remarks>
internal static class SdkApiStringRegistry
{
    private static bool _initialized;
    private static readonly Lock InitializationLock = new();

    /// <summary>
    /// Ensures the SDK enum mappings are registered with the Core registry.
    /// This method is thread-safe.
    /// </summary>
    internal static void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        lock (InitializationLock)
        {
            if (_initialized)
            {
                return;
            }

            Initialize();
            _initialized = true;
        }
    }

    private static void Initialize()
    {
        // Register Scheduler mappings
        ApiStringRegistry.Register(new Dictionary<Scheduler, string>
        {
            [Scheduler.Euler] = "euler",
            [Scheduler.EulerAncestral] = "euler_a",
            [Scheduler.LinearMultistep] = "lms",
            [Scheduler.Heun] = "heun",
            [Scheduler.DpmSolver2] = "dpm_2",
            [Scheduler.DpmSolver2Ancestral] = "dpm_2_a",
            [Scheduler.DpmPlusPlus2SAncestral] = "dpmpp_2s_a",
            [Scheduler.DpmPlusPlus2M] = "dpmpp_2m",
            [Scheduler.DpmPlusPlusSde] = "dpmpp_sde",
            [Scheduler.DpmPlusPlus2MSde] = "dpmpp_2m_sde",
            [Scheduler.DpmPlusPlus2MSdeKarras] = "dpmpp_2m_sde_karras",
            [Scheduler.DpmPlusPlus3MSde] = "dpmpp_3m_sde",
            [Scheduler.DpmPlusPlus3MSdeKarras] = "dpmpp_3m_sde_karras",
            [Scheduler.Ddim] = "ddim",
            [Scheduler.Plms] = "plms",
            [Scheduler.UniPc] = "uni_pc",
            [Scheduler.UniPcBh2] = "uni_pc_bh2",
            [Scheduler.Ddpm] = "ddpm",
            [Scheduler.Lcm] = "lcm",
        });

        // Register AvailabilityStatus mappings
        ApiStringRegistry.Register(new Dictionary<AvailabilityStatus, string>
        {
            [AvailabilityStatus.Available] = "Available",
            [AvailabilityStatus.Unavailable] = "Unavailable",
            [AvailabilityStatus.Degraded] = "Degraded",
        });

        // Register NetworkType mappings
        ApiStringRegistry.Register(new Dictionary<NetworkType, string>
        {
            [NetworkType.Lora] = "lora",
            [NetworkType.Lycoris] = "lycoris",
            [NetworkType.Dora] = "dora",
            [NetworkType.Embedding] = "embedding",
            [NetworkType.Vae] = "vae",
        });

        // Register ControlNetPreprocessor mappings
        ApiStringRegistry.Register(new Dictionary<ControlNetPreprocessor, string>
        {
            [ControlNetPreprocessor.Canny] = "Canny",
            [ControlNetPreprocessor.DepthZoe] = "DepthZoe",
            [ControlNetPreprocessor.SoftEdgePidinet] = "SoftedgePidinet",
            [ControlNetPreprocessor.Rembg] = "Rembg",
        });

        // Register AirAssetType mappings
        ApiStringRegistry.Register(new Dictionary<AirAssetType, string>
        {
            [AirAssetType.Checkpoint] = "checkpoint",
            [AirAssetType.Lora] = "lora",
            [AirAssetType.Lycoris] = "lycoris",
            [AirAssetType.Vae] = "vae",
            [AirAssetType.Embedding] = "embedding",
            [AirAssetType.Hypernetwork] = "hypernet",
        });

        // Register AirEcosystem mappings
        ApiStringRegistry.Register(new Dictionary<AirEcosystem, string>
        {
            [AirEcosystem.StableDiffusion1] = "sd1",
            [AirEcosystem.StableDiffusion2] = "sd2",
            [AirEcosystem.StableDiffusionXl] = "sdxl",
            [AirEcosystem.Flux1] = "flux1",
            [AirEcosystem.Pony] = "pony",
        });

        // Register AirSource mappings
        ApiStringRegistry.Register(new Dictionary<AirSource, string>
        {
            [AirSource.Civitai] = "civitai",
            [AirSource.HuggingFace] = "huggingface",
            [AirSource.OpenAi] = "openai",
            [AirSource.Leonardo] = "leonardo",
        });

        // Register JobEventType mappings
        ApiStringRegistry.Register(new Dictionary<JobEventType, string>
        {
            [JobEventType.Initialized] = "Initialized",
            [JobEventType.Claimed] = "Claimed",
            [JobEventType.Rejected] = "Rejected",
            [JobEventType.LateRejected] = "LateRejected",
            [JobEventType.ClaimExpired] = "ClaimExpired",
            [JobEventType.Updated] = "Updated",
            [JobEventType.Failed] = "Failed",
            [JobEventType.Succeeded] = "Succeeded",
            [JobEventType.Expired] = "Expired",
            [JobEventType.Deleted] = "Deleted",
        });

        // Register JobSupport mappings
        ApiStringRegistry.Register(new Dictionary<JobSupport, string>
        {
            [JobSupport.Unsupported] = "Unsupported",
            [JobSupport.Unavailable] = "Unavailable",
            [JobSupport.Available] = "Available",
        });

        // Register Provider mappings
        ApiStringRegistry.Register(new Dictionary<Provider, string>
        {
            [Provider.Civitai] = "Civitai",
            [Provider.OctoML] = "OctoML",
            [Provider.SaladML] = "SaladML",
            [Provider.PicFinder] = "PicFinder",
            [Provider.RunPods] = "RunPods",
            [Provider.ValdiAI] = "ValdiAI",
            [Provider.OctoMLNext] = "OctoMLNext",
            [Provider.RunDiffusion] = "RunDiffusion",
            [Provider.SaladShared] = "SaladShared",
        });
    }
}
