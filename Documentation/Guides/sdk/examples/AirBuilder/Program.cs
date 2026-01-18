using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CivitaiSharp.Core;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Air;

AirBuilderSamples.BasicUsage();
AirBuilderSamples.MethodChainingExample();

static class AirBuilderSamples
{
    public static void BasicUsage()
    {
        #region BasicUsage
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
        #endregion
    }

    public static void WithEcosystemExamples()
    {
        var builder = new AirBuilder();

        #region WithEcosystem
        builder.WithEcosystem(AirEcosystem.StableDiffusionXl);
        builder.WithEcosystem(AirEcosystem.Flux1);
        builder.WithEcosystem(AirEcosystem.Pony);
        #endregion
    }

    public static void WithAssetTypeExamples()
    {
        var builder = new AirBuilder();

        #region WithAssetType
        builder.WithAssetType(AirAssetType.Lora);
        builder.WithAssetType(AirAssetType.Checkpoint);
        builder.WithAssetType(AirAssetType.Vae);
        #endregion
    }

    public static void WithSourceExample()
    {
        var builder = new AirBuilder();

        #region WithSource
        // Explicitly set source (usually not needed)
        builder.WithSource(AirSource.Civitai);
        #endregion
    }

    public static void WithModelIdExample()
    {
        var builder = new AirBuilder();

        #region WithModelId
        builder.WithModelId(328553);

        // Must be greater than 0
        #endregion
    }

    public static void WithVersionIdExample()
    {
        var builder = new AirBuilder();

        #region WithVersionId
        builder.WithVersionId(368189);

        // Must be greater than 0
        #endregion
    }

    public static void ResetReuseExample()
    {
        #region ResetReuse
        // Start from a base builder and derive per-item builders (recommended)
        var baseBuilder = new AirBuilder()
            .WithEcosystem(AirEcosystem.Flux1)
            .WithAssetType(AirAssetType.Lora);

        // Derive from base and set specific IDs
        var fluxLora = baseBuilder
            .WithModelId(123)
            .WithVersionId(456)
            .Build();

        // Create a completely different identifier
        var sdxlCheckpoint = new AirBuilder()
            .WithEcosystem(AirEcosystem.StableDiffusionXl)
            .WithAssetType(AirAssetType.Checkpoint)
            .WithModelId(789)
            .WithVersionId(101)
            .Build();
        #endregion
    }

    public static void BuildExample()
    {
        var builder = new AirBuilder();

        #region Build
        var airId = builder.Build();

        // Throws InvalidOperationException if:
        // - Ecosystem is not set
        // - AssetType is not set
        // - ModelId is not set
        // - VersionId is not set
        #endregion
    }

    public static void InputValidationExample()
    {
        var builder = new AirBuilder();

        #region InputValidation
        // ModelId must be > 0
        builder.WithModelId(0); // Throws ArgumentOutOfRangeException

        // VersionId must be > 0
        builder.WithVersionId(-1); // Throws ArgumentOutOfRangeException
        #endregion
    }

    public static void BuildValidationExample()
    {
        #region BuildValidation
        var builder = new AirBuilder()
            .WithEcosystem(AirEcosystem.Flux1)
            .WithModelId(123);

        // Missing AssetType and VersionId
        var airId = builder.Build(); // Throws InvalidOperationException
        #endregion
    }

    public static async Task<AirIdentifier?> BuildFromModelAsync(IApiClient apiClient, int modelId)
    {
        ArgumentNullException.ThrowIfNull(apiClient);
        #region BuildFromModel
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
        #endregion
    }

    public static List<AirIdentifier> BuildMultipleIdentifiers()
    {
        #region BatchBuilding
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
        #endregion
    }

    public static AirIdentifier? TryBuildAirId(
        AirEcosystem ecosystem,
        AirAssetType assetType,
        long modelId,
        long versionId)
    {
        #region BuilderErrorHandling
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
        #endregion
    }

    public static void ReuseBuildersExample(IEnumerable<(AirAssetType AssetType, long ModelId, long VersionId)> modelData)
    {
        ArgumentNullException.ThrowIfNull(modelData);
        #region ReuseBuilders
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
        #endregion
    }

    public static AirIdentifier BuildFromUserInput(long modelId, long versionId)
    {
        #region ValidateEarly
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
        #endregion
    }

    public static void MethodChainingExample()
    {
        #region MethodChaining
        // Fluent method chaining (recommended)
        var airIdentifier = new AirBuilder()
            .WithEcosystem(AirEcosystem.Flux1)
            .WithAssetType(AirAssetType.Lora)
            .WithModelId(123)
            .WithVersionId(456)
            .Build();
        #endregion
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
