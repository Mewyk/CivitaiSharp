namespace CivitaiSharp.Sdk.Extensions;

using System.Collections.Generic;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// Registers SDK-specific enum mappings with the Core's <see cref="ApiStringRegistry"/>.
/// Called automatically when <see cref="ServiceCollectionExtensions.AddCivitaiSdk(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action{SdkClientOptions})"/> is invoked.
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
    private static readonly object InitializationLock = new();

    /// <summary>
    /// Ensures the SDK enum mappings are registered with the Core registry.
    /// This method is idempotent and thread-safe.
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
            [ControlNetPreprocessor.Canny] = "canny",
            [ControlNetPreprocessor.Depth] = "depth",
            [ControlNetPreprocessor.DepthLeres] = "depth_leres",
            [ControlNetPreprocessor.DepthMidas] = "depth_midas",
            [ControlNetPreprocessor.DepthZoe] = "depth_zoe",
            [ControlNetPreprocessor.SoftEdgeHed] = "softedge_hed",
            [ControlNetPreprocessor.SoftEdgePidinet] = "softedge_pidinet",
            [ControlNetPreprocessor.Lineart] = "lineart",
            [ControlNetPreprocessor.LineartAnime] = "lineart_anime",
            [ControlNetPreprocessor.Openpose] = "openpose",
            [ControlNetPreprocessor.OpenposeFace] = "openpose_face",
            [ControlNetPreprocessor.OpenposeFull] = "openpose_full",
            [ControlNetPreprocessor.MediapipeFace] = "mediapipe_face",
            [ControlNetPreprocessor.NormalBae] = "normal_bae",
            [ControlNetPreprocessor.Segmentation] = "seg",
            [ControlNetPreprocessor.Shuffle] = "shuffle",
            [ControlNetPreprocessor.Tile] = "tile",
            [ControlNetPreprocessor.Inpaint] = "inpaint",
            [ControlNetPreprocessor.Mlsd] = "mlsd",
            [ControlNetPreprocessor.Scribble] = "scribble",
            [ControlNetPreprocessor.Rembg] = "rembg",
            [ControlNetPreprocessor.None] = "none",
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
    }
}
