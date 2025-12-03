namespace CivitaiSharp.Tools.Downloads.Validation;

using System;
using System.IO;
using CivitaiSharp.Tools.Downloads.Options;
using CivitaiSharp.Tools.Downloads.Patterns;
using Microsoft.Extensions.Options;

/// <summary>
/// Validates <see cref="ImageDownloadOptions"/> at startup and on change.
/// </summary>
public sealed class ImageOptionsValidator : IValidateOptions<ImageDownloadOptions>
{
    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, ImageDownloadOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Validate BaseDirectory
        if (string.IsNullOrWhiteSpace(options.BaseDirectory))
        {
            return ValidateOptionsResult.Fail("BaseDirectory cannot be null or empty.");
        }

        // Check if BaseDirectory is a valid path format
        try
        {
            // This will throw if the path format is invalid
            _ = Path.GetFullPath(options.BaseDirectory);
        }
        catch (Exception exception)
        {
            return ValidateOptionsResult.Fail($"BaseDirectory is not a valid path: {exception.Message}");
        }

        // Validate PathPattern
        if (string.IsNullOrWhiteSpace(options.PathPattern))
        {
            return ValidateOptionsResult.Fail("PathPattern cannot be null or empty.");
        }

        // Validate that the pattern contains only valid tokens
        var validationResult = PathPatternProcessor.ValidatePattern(options.PathPattern, ImagePatternTokens.ValidTokens);

        if (validationResult.IsFailure)
        {
            return ValidateOptionsResult.Fail(validationResult.ErrorInfo.Message);
        }

        return ValidateOptionsResult.Success;
    }
}
