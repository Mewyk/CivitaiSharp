namespace CivitaiSharp.Tools.Downloads.Patterns;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using CivitaiSharp.Core.Models;

/// <summary>
/// Defines valid tokens for image path patterns and extracts token values from images.
/// </summary>
public static class ImagePatternTokens
{
    /// <summary>
    /// The set of all valid token names for image path patterns.
    /// </summary>
    public static FrozenSet<string> ValidTokens { get; } = new[]
    {
        "Id",
        "PostId",
        "Username",
        "Width",
        "Height",
        "BaseModel",
        "NsfwLevel",
        "Date",
        "Extension"
    }.ToFrozenSet(StringComparer.Ordinal);

    /// <summary>
    /// Extracts token values from an image for use in path pattern processing.
    /// </summary>
    /// <param name="image">The image to extract values from.</param>
    /// <param name="extension">The file extension to use (without leading dot).</param>
    /// <returns>A dictionary mapping token names to their values.</returns>
    public static Dictionary<string, string> ExtractTokenValues(Image image, string extension)
    {
        ArgumentNullException.ThrowIfNull(image);

        // Pre-allocate with exact capacity for all 9 known tokens
        var tokens = new Dictionary<string, string>(capacity: 9, StringComparer.Ordinal)
        {
            ["Id"] = image.Id.ToString(CultureInfo.InvariantCulture),
            ["PostId"] = image.PostId?.ToString(CultureInfo.InvariantCulture) ?? "unknown",
            ["Username"] = image.Username ?? "unknown",
            ["Width"] = image.Width.ToString(CultureInfo.InvariantCulture),
            ["Height"] = image.Height.ToString(CultureInfo.InvariantCulture),
            ["BaseModel"] = image.BaseModel ?? "unknown",
            ["NsfwLevel"] = image.NsfwLevel?.ToString() ?? "None",
            ["Date"] = (image.CreatedAt ?? DateTime.UtcNow).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["Extension"] = string.IsNullOrWhiteSpace(extension) ? "png" : extension.TrimStart('.')
        };

        return tokens;
    }

    /// <summary>
    /// Infers the file extension from an image URL or media type.
    /// </summary>
    /// <param name="image">The image to infer the extension from.</param>
    /// <returns>The inferred extension without a leading dot, or "png" as the default.</returns>
    public static string InferExtension(Image image)
    {
        ArgumentNullException.ThrowIfNull(image);

        // Try to get extension from URL
        if (!string.IsNullOrWhiteSpace(image.Url))
        {
            var urlExtension = GetExtensionFromUrl(image.Url);
            if (!string.IsNullOrEmpty(urlExtension))
                return urlExtension;
        }

        // Fall back to media type
        return image.Type switch
        {
            MediaType.Video => "mp4",
            MediaType.Image => "png",
            _ => "png"
        };
    }

    /// <summary>
    /// Attempts to extract a file extension from a URL.
    /// </summary>
    private static string? GetExtensionFromUrl(string url)
    {
        // Handle URLs with query strings
        var queryIndex = url.IndexOf('?');
        var pathPart = queryIndex >= 0 ? url[..queryIndex] : url;

        // Find last path segment
        var lastSlashIndex = pathPart.LastIndexOf('/');
        if (lastSlashIndex < 0 || lastSlashIndex >= pathPart.Length - 1)
        {
            return null;
        }

        var lastSegment = pathPart[(lastSlashIndex + 1)..];
        var dotIndex = lastSegment.LastIndexOf('.');
        if (dotIndex <= 0 || dotIndex >= lastSegment.Length - 1)
        {
            return null;
        }

        var extension = lastSegment[(dotIndex + 1)..].ToLowerInvariant();

        // Only return known image/video extensions
        return extension switch
        {
            "png" or "jpg" or "jpeg" or "gif" or "webp" or "avif" => extension,
            "mp4" or "webm" or "mov" => extension,
            _ => null
        };
    }
}
