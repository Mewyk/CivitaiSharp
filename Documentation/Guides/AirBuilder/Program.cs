namespace Guides.AirBuilder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CivitaiSharp.Core;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Air;

public static class AirBuilderSamples
{
    public static void Main()
    {
    }

    public static void BasicUsage()
    {
        #region basic-usage
        // Build an AIR identifier using the fluent API
        var builder = new AirBuilder();

        var airId = builder
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(328553)
            .WithVersionId(368189)
            .Build();

        Console.WriteLine(airId.ToString());
        // Output: urn:air:sdxl:lora:civitai:328553@368189
        #endregion basic-usage
    }

    public static void WithEcosystemExamples()
    {
        var builder = new AirBuilder();

        #region with-ecosystem
        builder.WithEcosystem(AirEcosystem.StableDiffusionXl);
        builder.WithEcosystem(AirEcosystem.Flux1);
        builder.WithEcosystem(AirEcosystem.Pony);
        #endregion with-ecosystem
    }

    public static void WithAssetTypeExamples()
    {
        var builder = new AirBuilder();

        #region with-asset-type
        builder.WithAssetType(AirAssetType.Lora);
        builder.WithAssetType(AirAssetType.Checkpoint);
        builder.WithAssetType(AirAssetType.Vae);
        #endregion with-asset-type
    }

    public static void WithSourceExample()
    {
        var builder = new AirBuilder();

        #region with-source
        // Explicitly set source (usually not needed)
        builder.WithSource(AirSource.Civitai);
        #endregion with-source
    }

    public static void WithModelIdExample()
    {
        var builder = new AirBuilder();

        #region with-model-id
        builder.WithModelId(328553);

        // Must be greater than 0
        #endregion with-model-id
    }

    public static void WithVersionIdExample()
    {
        var builder = new AirBuilder();

        #region with-version-id
        builder.WithVersionId(368189);

        // Must be greater than 0
        #endregion with-version-id
    }

    public static void ResetReuseExample()
    {
        #region reset-reuse
        // Start from a base builder and derive per-item builders (recommended)
        var baseBuilder = new AirBuilder()
            .WithEcosystem(AirEcosystem.Flux1)
            .WithAssetType(AirAssetType.Lora);

        // For a new identifier, derive from the base and set IDs
        var air1 = baseBuilder
            .WithModelId(123)
            .WithVersionId(456)
            .Build();

        // To "reset", simply start from a fresh builder or reuse baseBuilder
        var air2 = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Checkpoint)
            .WithModelId(789)
            .WithVersionId(101)
            .Build();
        #endregion reset-reuse
    }

    public static void BuildExample()
    {
        var builder = new AirBuilder();

        #region build
        var airId = builder.Build();

        // Throws InvalidOperationException if:
        // - Ecosystem is not set
        // - AssetType is not set
        // - ModelId is not set
        // - VersionId is not set
        #endregion build
    }

    public static void InputValidationExample()
    {
        var builder = new AirBuilder();

        #region input-validation
        // ModelId must be > 0
        builder.WithModelId(0); // Throws ArgumentOutOfRangeException

        // VersionId must be > 0
        builder.WithVersionId(-1); // Throws ArgumentOutOfRangeException
        #endregion input-validation
    }

    public static void BuildValidationExample()
    {
        #region build-validation
        var builder = new AirBuilder()
            .WithEcosystem(AirEcosystem.Flux1)
            .WithModelId(123);

        // Missing AssetType and VersionId
        var airId = builder.Build(); // Throws InvalidOperationException
        #endregion build-validation
    }

    public static async Task<AirIdentifier?> BuildFromModelAsync(IApiClient apiClient, int modelId)
    {
        #region build-from-model
        // Fetch model from Civitai
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

        // Build AIR identifier
        var builder = new AirBuilder();
        return builder
            .WithEcosystem(GetEcosystem(version.BaseModel))
            .WithAssetType(GetAssetType(model.Type))
            .WithModelId(model.Id)
            .WithVersionId(version.Id)
            .Build();
        #endregion build-from-model
    }

    public static List<AirIdentifier> BuildMultipleIdentifiers()
    {
        #region batch-building
        var builder = new AirBuilder();
        var identifiers = new List<AirIdentifier>();

        // Build multiple identifiers efficiently
        foreach (var (ecosystem, assetType, modelId, versionId) in GetModelData())
        {
            var airId = builder
                .WithEcosystem(ecosystem)
                .WithAssetType(assetType)
                .WithModelId(modelId)
                .WithVersionId(versionId)
                .Build();

            identifiers.Add(airId);
        }

        return identifiers;
        #endregion batch-building
    }

    public static AirIdentifier? TryBuildAirId(
        AirEcosystem ecosystem,
        AirAssetType assetType,
        long modelId,
        long versionId)
    {
        #region builder-error-handling
        try
        {
            var builder = new AirBuilder();
            return builder
                .WithEcosystem(ecosystem)
                .WithAssetType(assetType)
                .WithModelId(modelId)
                .WithVersionId(versionId)
                .Build();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Invalid ID: {ex.Message}");
            return null;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Missing required property: {ex.Message}");
            return null;
        }
        #endregion builder-error-handling
    }

    public static void ReuseBuildersExample(IEnumerable<(AirAssetType AssetType, long ModelId, long VersionId)> modelData)
    {
        #region reuse-builders
        // Good - derive per-item builders from a reusable base configuration
        var baseBuilder = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl);

        foreach (var data in modelData)
        {
            var airId = baseBuilder
                .WithAssetType(data.AssetType)
                .WithModelId(data.ModelId)
                .WithVersionId(data.VersionId)
                .Build();

            ProcessAirId(airId);
        }
        #endregion reuse-builders
    }

    public static AirIdentifier BuildFromUserInput(long modelId, long versionId)
    {
        #region validate-early
        // Validate before building
        if (modelId <= 0)
        {
            throw new ArgumentException("Model ID must be positive", nameof(modelId));
        }

        if (versionId <= 0)
        {
            throw new ArgumentException("Version ID must be positive", nameof(versionId));
        }

        return new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(modelId)
            .WithVersionId(versionId)
            .Build();
        #endregion validate-early
    }

    public static void MethodChainingExample()
    {
        #region method-chaining
        // Preferred - fluent style
        var airId = new AirBuilder()
            .WithEcosystem(AirEcosystem.Flux1)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(123)
            .WithVersionId(456)
            .Build();

        // Avoid - verbose style
        var builder = new AirBuilder();
        builder.WithEcosystem(AirEcosystem.Flux1);
        builder.WithAssetType(AirAssetType.Lora);
        builder.WithModelId(123);
        builder.WithVersionId(456);
        var verboseAirId = builder.Build();
        #endregion method-chaining
    }

    private static AirEcosystem GetEcosystem(string baseModel) => baseModel switch
    {
        "SD 1.5" => AirEcosystem.StableDiffusion1,
        "SDXL 1.0" => AirEcosystem.StableDiffusionXl,
        "Flux.1" => AirEcosystem.Flux1,
        "Pony" => AirEcosystem.Pony,
        _ => AirEcosystem.StableDiffusion1,
    };

    private static AirAssetType GetAssetType(ModelType type) => type switch
    {
        ModelType.Checkpoint => AirAssetType.Checkpoint,
        ModelType.Lora => AirAssetType.Lora,
        ModelType.Vae => AirAssetType.Vae,
        ModelType.TextualInversion => AirAssetType.Embedding,
        ModelType.Hypernetwork => AirAssetType.Hypernetwork,
        _ => AirAssetType.Checkpoint,
    };

    private static IEnumerable<(AirEcosystem, AirAssetType, long, long)> GetModelData()
    {
        yield return (AirEcosystem.StableDiffusionXl, AirAssetType.Lora, 328553, 368189);
        yield return (AirEcosystem.Flux1, AirAssetType.Checkpoint, 123456, 789012);
        yield return (AirEcosystem.Pony, AirAssetType.Lora, 111111, 222222);
    }

    private static void ProcessAirId(AirIdentifier airId)
    {
        _ = airId;
    }
}
