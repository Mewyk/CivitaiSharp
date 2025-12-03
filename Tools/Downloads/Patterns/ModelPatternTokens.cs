namespace CivitaiSharp.Tools.Downloads.Patterns;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;

/// <summary>
/// Defines valid tokens for model file path patterns and extracts token values from model files.
/// </summary>
public static class ModelPatternTokens
{
    /// <summary>
    /// The set of valid token names when only a <see cref="ModelFile"/> is available.
    /// </summary>
    public static FrozenSet<string> FileOnlyTokens { get; } = new[]
    {
        "FileId",
        "FileName",
        "FileType",
        "Format",
        "Size",
        "Precision"
    }.ToFrozenSet(StringComparer.Ordinal);

    /// <summary>
    /// The set of all valid token names when both <see cref="ModelFile"/> and <see cref="ModelVersion"/> are available.
    /// </summary>
    public static FrozenSet<string> AllTokens { get; } = new[]
    {
        // File tokens
        "FileId",
        "FileName",
        "FileType",
        "Format",
        "Size",
        "Precision",
        // Version tokens
        "VersionId",
        "VersionName",
        "BaseModel",
        // Model tokens (from ModelVersion.Model)
        "ModelId",
        "ModelName",
        "ModelType"
    }.ToFrozenSet(StringComparer.Ordinal);

    /// <summary>
    /// Extracts token values from a model file for use in path pattern processing.
    /// </summary>
    /// <param name="file">The model file to extract values from.</param>
    /// <returns>A dictionary mapping token names to their values.</returns>
    public static Dictionary<string, string> ExtractTokenValues(ModelFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        // Pre-allocate with exact capacity for all 6 file-only tokens
        return new Dictionary<string, string>(capacity: 6, StringComparer.Ordinal)
        {
            ["FileId"] = file.Id.ToString(CultureInfo.InvariantCulture),
            ["FileName"] = file.Name,
            ["FileType"] = file.Type ?? "Model",
            ["Format"] = file.Metadata?.Format ?? "unknown",
            ["Size"] = file.Metadata?.Size ?? "unknown",
            ["Precision"] = file.Metadata?.Precision ?? "unknown"
        };
    }

    /// <summary>
    /// Extracts token values from a model file and model version for use in path pattern processing.
    /// </summary>
    /// <param name="file">The model file to extract values from.</param>
    /// <param name="version">The model version providing additional context.</param>
    /// <returns>A dictionary mapping token names to their values.</returns>
    public static Dictionary<string, string> ExtractTokenValues(ModelFile file, ModelVersion version)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(version);

        // Pre-allocate with exact capacity for all 12 tokens (6 file + 6 version/model)
        var tokens = new Dictionary<string, string>(capacity: 12, StringComparer.Ordinal)
        {
            // File tokens
            ["FileId"] = file.Id.ToString(CultureInfo.InvariantCulture),
            ["FileName"] = file.Name,
            ["FileType"] = file.Type ?? "Model",
            ["Format"] = file.Metadata?.Format ?? "unknown",
            ["Size"] = file.Metadata?.Size ?? "unknown",
            ["Precision"] = file.Metadata?.Precision ?? "unknown",

            // Version tokens
            ["VersionId"] = version.Id.ToString(CultureInfo.InvariantCulture),
            ["VersionName"] = version.Name ?? "unknown",
            ["BaseModel"] = version.BaseModel ?? "unknown"
        };

        // Add model tokens (if Model is available)
        // Note: ModelVersionModel only contains Name and Type, not Id or Creator
        if (version.Model is not null)
        {
            tokens["ModelId"] = version.ModelId?.ToString(CultureInfo.InvariantCulture) ?? "unknown";
            tokens["ModelName"] = version.Model.Name ?? "unknown";
            tokens["ModelType"] = version.Model.Type.ToApiString() ?? "unknown";
        }
        else
        {
            // Provide fallbacks when Model is not available
            tokens["ModelId"] = version.ModelId?.ToString(CultureInfo.InvariantCulture) ?? "unknown";
            tokens["ModelName"] = "unknown";
            tokens["ModelType"] = "unknown";
        }

        return tokens;
    }

    /// <summary>
    /// Determines the appropriate set of valid tokens based on whether a version is provided.
    /// </summary>
    /// <param name="hasVersion">Whether a <see cref="ModelVersion"/> is available.</param>
    /// <returns>The appropriate set of valid tokens.</returns>
    public static FrozenSet<string> GetValidTokens(bool hasVersion)
        => hasVersion ? AllTokens : FileOnlyTokens;
}
