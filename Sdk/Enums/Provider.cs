namespace CivitaiSharp.Sdk.Enums;

using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Json.Converters;

/// <summary>
/// Known infrastructure providers used by Civitai's orchestration system.
/// </summary>
[JsonConverter(typeof(ProviderConverter))]
public enum Provider
{
    /// <summary>Civitai's first-party infrastructure.</summary>
    Civitai = 0,

    /// <summary>OctoML provider.</summary>
    OctoML,

    /// <summary>SaladML provider.</summary>
    SaladML,

    /// <summary>PicFinder provider.</summary>
    PicFinder,

    /// <summary>RunPods provider.</summary>
    RunPods,

    /// <summary>ValdiAI provider.</summary>
    ValdiAI,

    /// <summary>Next-generation OctoML provider.</summary>
    OctoMLNext,

    /// <summary>RunDiffusion provider.</summary>
    RunDiffusion,

    /// <summary>SaladShared provider.</summary>
    SaladShared
}
