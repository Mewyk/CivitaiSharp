namespace CivitaiSharp.Sdk.Air;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Immutable, thread-safe builder for constructing <see cref="AirIdentifier"/> instances with validation.
/// Each fluent method returns a new builder instance, allowing safe reuse and caching of base configurations.
/// </summary>
/// <remarks>
/// <para>
/// This builder simplifies the construction of AIR (Artificial Intelligence Resource) identifiers
/// by providing a clear, step-by-step fluent interface. Use this when you want to construct
/// AIR identifiers programmatically with compile-time guidance and runtime validation.
/// </para>
/// <para>
/// This builder follows the same immutable pattern as <c>RequestBuilder</c> in CivitaiSharp.Core.
/// Each method returns a new instance, making it inherently thread-safe.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var air = new AirBuilder()
///     .WithEcosystem(AirEcosystem.StableDiffusionXl)
///     .WithAssetType(AirAssetType.Lora)
///     .WithModelId(328553)
///     .WithVersionId(368189)
///     .Build();
/// 
/// Console.WriteLine(air.ToString());
/// // Output: urn:air:sdxl:lora:civitai:328553@368189
/// </code>
/// </example>
public sealed record AirBuilder
{
    private readonly AirEcosystem? _ecosystem;
    private readonly AirAssetType? _assetType;
    private readonly AirSource _source;
    private readonly long? _modelId;
    private readonly long? _versionId;

    /// <summary>
    /// Initializes a new instance of the <see cref="AirBuilder"/> record with default values.
    /// </summary>
    public AirBuilder()
        : this(ecosystem: null, assetType: null, source: AirIdentifier.DefaultSource, modelId: null, versionId: null)
    {
    }

    private AirBuilder(
        AirEcosystem? ecosystem,
        AirAssetType? assetType,
        AirSource source,
        long? modelId,
        long? versionId)
    {
        _ecosystem = ecosystem;
        _assetType = assetType;
        _source = source;
        _modelId = modelId;
        _versionId = versionId;
    }

    /// <summary>
    /// Sets the ecosystem (e.g., Stable Diffusion XL, FLUX.1).
    /// </summary>
    /// <param name="ecosystem">The model ecosystem.</param>
    /// <returns>A new builder instance with the ecosystem set.</returns>
    public AirBuilder WithEcosystem(AirEcosystem ecosystem) =>
        new(ecosystem, _assetType, _source, _modelId, _versionId);

    /// <summary>
    /// Sets the asset type (e.g., Checkpoint, LoRA, Embedding).
    /// </summary>
    /// <param name="assetType">The asset type.</param>
    /// <returns>A new builder instance with the asset type set.</returns>
    public AirBuilder WithAssetType(AirAssetType assetType) =>
        new(_ecosystem, assetType, _source, _modelId, _versionId);

    /// <summary>
    /// Sets the source platform for the resource.
    /// </summary>
    /// <param name="source">The source platform. Defaults to <see cref="AirSource.Civitai"/> if not specified.</param>
    /// <returns>A new builder instance with the source set.</returns>
    public AirBuilder WithSource(AirSource source) =>
        new(_ecosystem, _assetType, source, _modelId, _versionId);

    /// <summary>
    /// Sets the model ID.
    /// </summary>
    /// <param name="modelId">The model ID. Must be greater than 0.</param>
    /// <returns>A new builder instance with the model ID set.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when modelId is less than 1.</exception>
    public AirBuilder WithModelId(long modelId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(modelId, 1);
        return new(_ecosystem, _assetType, _source, modelId, _versionId);
    }

    /// <summary>
    /// Sets the version ID.
    /// </summary>
    /// <param name="versionId">The version ID. Must be greater than 0.</param>
    /// <returns>A new builder instance with the version ID set.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when versionId is less than 1.</exception>
    public AirBuilder WithVersionId(long versionId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(versionId, 1);
        return new(_ecosystem, _assetType, _source, _modelId, versionId);
    }

    /// <summary>
    /// Builds the <see cref="AirIdentifier"/> with the configured values.
    /// </summary>
    /// <returns>A new <see cref="AirIdentifier"/> instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required properties (<see cref="AirIdentifier.Ecosystem"/>, <see cref="AirIdentifier.AssetType"/>,
    /// <see cref="AirIdentifier.ModelId"/>, <see cref="AirIdentifier.VersionId"/>) have not been set.
    /// </exception>
    public AirIdentifier Build()
    {
        if (!_ecosystem.HasValue)
        {
            throw new InvalidOperationException(
                $"{nameof(AirIdentifier.Ecosystem)} must be set before building. Use {nameof(WithEcosystem)}() to specify the model ecosystem.");
        }

        if (!_assetType.HasValue)
        {
            throw new InvalidOperationException(
                $"{nameof(AirIdentifier.AssetType)} must be set before building. Use {nameof(WithAssetType)}() to specify the asset type.");
        }

        if (!_modelId.HasValue)
        {
            throw new InvalidOperationException(
                $"{nameof(AirIdentifier.ModelId)} must be set before building. Use {nameof(WithModelId)}() to specify the model ID.");
        }

        if (!_versionId.HasValue)
        {
            throw new InvalidOperationException(
                $"{nameof(AirIdentifier.VersionId)} must be set before building. Use {nameof(WithVersionId)}() to specify the version ID.");
        }

        return new AirIdentifier(
            _ecosystem.Value,
            _assetType.Value,
            _source,
            _modelId.Value,
            _versionId.Value);
    }

    /// <summary>
    /// Attempts to build the <see cref="AirIdentifier"/> with the configured values.
    /// </summary>
    /// <param name="result">When this method returns <see langword="true"/>, contains the built <see cref="AirIdentifier"/>;
    /// otherwise, contains the default value.</param>
    /// <returns><see langword="true"/> if all required properties were set and the identifier was built successfully; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// Use this method when you prefer to handle missing required properties without exceptions.
    /// All properties (<see cref="AirIdentifier.Ecosystem"/>, <see cref="AirIdentifier.AssetType"/>,
    /// <see cref="AirIdentifier.ModelId"/>, <see cref="AirIdentifier.VersionId"/>) must be set for the build to succeed.
    /// </remarks>
    public bool TryBuild([MaybeNullWhen(false)] out AirIdentifier result)
    {
        if (!_ecosystem.HasValue || !_assetType.HasValue || !_modelId.HasValue || !_versionId.HasValue)
        {
            result = default;
            return false;
        }

        result = new AirIdentifier(
            _ecosystem.Value,
            _assetType.Value,
            _source,
            _modelId.Value,
            _versionId.Value);
        return true;
    }
}
