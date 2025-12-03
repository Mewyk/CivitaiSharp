namespace CivitaiSharp.Tools.Downloads.Validation;

using System;
using System.IO;
using CivitaiSharp.Tools.Downloads.Options;
using CivitaiSharp.Tools.Downloads.Patterns;
using CivitaiSharp.Tools.Hashing;
using Microsoft.Extensions.Options;

/// <summary>
/// Validates <see cref="ModelDownloadOptions"/> at startup and on change.
/// </summary>
public sealed class ModelOptionsValidator : IValidateOptions<ModelDownloadOptions>
{
    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, ModelDownloadOptions options)
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

        // Validate the pattern using AllTokens since we want to allow all tokens
        // (file-only patterns will still work, just with fallback values for missing version tokens)
        var validationResult = PathPatternProcessor.ValidatePattern(options.PathPattern, ModelPatternTokens.AllTokens);

        if (validationResult.IsFailure)
        {
            return ValidateOptionsResult.Fail(validationResult.ErrorInfo.Message);
        }

        // Validate HashAlgorithm is a valid enum value
        if (!Enum.IsDefined(options.HashAlgorithm))
        {
            return ValidateOptionsResult.Fail(
                $"HashAlgorithm '{options.HashAlgorithm}' is not a valid algorithm. " +
                $"Valid values are: {string.Join(", ", Enum.GetNames<HashAlgorithm>())}");
        }

        return ValidateOptionsResult.Success;
    }
}
