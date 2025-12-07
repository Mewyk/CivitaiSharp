namespace CivitaiSharp.Sdk.Air;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CivitaiSharp.Core.Extensions;

/// <summary>
/// Represents an AIR (Artificial Intelligence Resource) identifier for Civitai models and assets.
/// Format: <c>urn:air:{ecosystem}:{type}:{source}:{modelId}@{versionId}</c>
/// (e.g., <c>urn:air:sdxl:checkpoint:civitai:4201@130072</c>)
/// </summary>
public readonly partial struct AirIdentifier : IEquatable<AirIdentifier>, IParsable<AirIdentifier>
{
    /// <summary>
    /// The default source for Civitai assets.
    /// </summary>
    public const AirSource DefaultSource = AirSource.Civitai;

    /// <summary>
    /// Compiled regex pattern for parsing AIR identifiers.
    /// </summary>
    [GeneratedRegex(@"^urn:air:([a-z0-9]+):([a-z]+):([a-z]+):(\d+)@(\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex AirPattern();

    /// <summary>
    /// Gets the model ecosystem (e.g., sd1, sdxl, flux1).
    /// </summary>
    public AirEcosystem Ecosystem { get; }

    /// <summary>
    /// Gets the asset type (e.g., checkpoint, lora).
    /// </summary>
    public AirAssetType AssetType { get; }

    /// <summary>
    /// Gets the source platform of the asset.
    /// </summary>
    public AirSource Source { get; }

    /// <summary>
    /// Gets the model ID on Civitai.
    /// </summary>
    public long ModelId { get; }

    /// <summary>
    /// Gets the model version ID on Civitai.
    /// </summary>
    public long VersionId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AirIdentifier"/> struct.
    /// </summary>
    /// <param name="ecosystem">The model ecosystem.</param>
    /// <param name="assetType">The asset type.</param>
    /// <param name="source">The source platform.</param>
    /// <param name="modelId">The model ID.</param>
    /// <param name="versionId">The version ID.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when modelId or versionId is less than 1.</exception>
    public AirIdentifier(
        AirEcosystem ecosystem,
        AirAssetType assetType,
        AirSource source,
        long modelId,
        long versionId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(modelId, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(versionId, 1);

        Ecosystem = ecosystem;
        AssetType = assetType;
        Source = source;
        ModelId = modelId;
        VersionId = versionId;
    }

    /// <summary>
    /// Creates an AIR identifier for a Civitai model.
    /// </summary>
    /// <param name="ecosystem">The model ecosystem.</param>
    /// <param name="assetType">The asset type.</param>
    /// <param name="modelId">The Civitai model ID.</param>
    /// <param name="versionId">The Civitai version ID.</param>
    /// <returns>A new <see cref="AirIdentifier"/> instance.</returns>
    public static AirIdentifier Create(
        AirEcosystem ecosystem,
        AirAssetType assetType,
        long modelId,
        long versionId) =>
        new(ecosystem, assetType, DefaultSource, modelId, versionId);

    /// <summary>
    /// Attempts to parse a string as an AIR identifier.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed identifier if successful.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string? value, out AirIdentifier result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var match = AirPattern().Match(value);
        if (!match.Success)
        {
            return false;
        }

        var ecosystemStr = match.Groups[1].Value;
        var typeStr = match.Groups[2].Value;
        var sourceStr = match.Groups[3].Value;
        var modelIdStr = match.Groups[4].Value;
        var versionIdStr = match.Groups[5].Value;

        if (!EnumExtensions.TryParseFromApiString<AirEcosystem>(ecosystemStr, out var ecosystem))
        {
            return false;
        }

        if (!EnumExtensions.TryParseFromApiString<AirAssetType>(typeStr, out var assetType))
        {
            return false;
        }

        if (!EnumExtensions.TryParseFromApiString<AirSource>(sourceStr, out var source))
        {
            return false;
        }

        if (!long.TryParse(modelIdStr, out var modelId) || modelId < 1)
        {
            return false;
        }

        if (!long.TryParse(versionIdStr, out var versionId) || versionId < 1)
        {
            return false;
        }

        result = new AirIdentifier(ecosystem, assetType, source, modelId, versionId);
        return true;
    }

    /// <summary>
    /// Parses a string as an AIR identifier.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <returns>The parsed <see cref="AirIdentifier"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    /// <exception cref="ArgumentException">Thrown when value is empty or whitespace.</exception>
    /// <exception cref="FormatException">Thrown when value is not a valid AIR identifier.</exception>
    public static AirIdentifier Parse(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!TryParse(value, out var result))
        {
            throw new FormatException($"'{value}' is not a valid AIR identifier. Expected format: urn:air:{{ecosystem}}:{{type}}:{{source}}:{{modelId}}@{{versionId}}");
        }

        return result;
    }

    /// <inheritdoc />
    static AirIdentifier IParsable<AirIdentifier>.Parse(string s, IFormatProvider? provider) =>
        Parse(s);

    /// <inheritdoc />
    static bool IParsable<AirIdentifier>.TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out AirIdentifier result) =>
        TryParse(s, out result);

    /// <summary>
    /// Returns the string representation of this AIR identifier.
    /// </summary>
    /// <returns>The AIR identifier string in the format <c>urn:air:{ecosystem}:{type}:{source}:{modelId}@{versionId}</c>.</returns>
    public override string ToString() =>
        $"urn:air:{Ecosystem.ToApiString()}:{AssetType.ToApiString()}:{Source.ToApiString()}:{ModelId}@{VersionId}";

    /// <inheritdoc />
    public bool Equals(AirIdentifier other) =>
        Ecosystem == other.Ecosystem &&
        AssetType == other.AssetType &&
        Source == other.Source &&
        ModelId == other.ModelId &&
        VersionId == other.VersionId;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is AirIdentifier other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Ecosystem, AssetType, Source, ModelId, VersionId);

    /// <summary>
    /// Determines whether two AIR identifiers are equal.
    /// </summary>
    public static bool operator ==(AirIdentifier left, AirIdentifier right) => left.Equals(right);

    /// <summary>
    /// Determines whether two AIR identifiers are not equal.
    /// </summary>
    public static bool operator !=(AirIdentifier left, AirIdentifier right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts an AIR identifier to its string representation.
    /// </summary>
    public static implicit operator string(AirIdentifier air) => air.ToString();
}
