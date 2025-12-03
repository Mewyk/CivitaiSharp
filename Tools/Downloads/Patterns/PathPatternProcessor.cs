namespace CivitaiSharp.Tools.Downloads.Patterns;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CivitaiSharp.Core.Response;

/// <summary>
/// Processes path patterns by replacing tokens with actual values.
/// </summary>
/// <remarks>
/// <para>
/// This processor handles token replacement and path sanitization to ensure
/// generated file paths are valid across all supported platforms.
/// </para>
/// <para>
/// Invalid characters in token values are replaced with underscores to ensure
/// file system compatibility. Directory separators in patterns are normalized
/// for the current operating system.
/// </para>
/// </remarks>
public static class PathPatternProcessor
{
    private static readonly FrozenSet<char> InvalidPathChars = Path.GetInvalidPathChars()
        .Concat(Path.GetInvalidFileNameChars())
        .Where(c => c != Path.DirectorySeparatorChar && c != Path.AltDirectorySeparatorChar)
        .Distinct()
        .ToFrozenSet();

    /// <summary>
    /// Validates that a path pattern contains only valid tokens.
    /// </summary>
    /// <param name="pattern">The pattern to validate.</param>
    /// <param name="validTokens">The set of valid token names (without braces).</param>
    /// <returns>A result indicating success or containing validation error details.</returns>
    public static Result<Unit> ValidatePattern(string pattern, FrozenSet<string> validTokens)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        ArgumentNullException.ThrowIfNull(validTokens);

        var invalidTokens = new List<string>();
        var index = 0;

        while (index < pattern.Length)
        {
            var openBrace = pattern.IndexOf('{', index);
            if (openBrace == -1)
                break;

            var closeBrace = pattern.IndexOf('}', openBrace);
            if (closeBrace == -1)
            {
                return new Result<Unit>.Failure(
                    Error.Create(ErrorCode.PatternValidationFailed,
                        $"Unclosed token at position {openBrace}. Missing closing brace '}}' for token starting at '{pattern[openBrace..]}'."));
            }

            var tokenName = pattern[(openBrace + 1)..closeBrace];

            if (string.IsNullOrWhiteSpace(tokenName))
            {
                return new Result<Unit>.Failure(
                    Error.Create(ErrorCode.PatternValidationFailed,
                        $"Empty token '{{}}' at position {openBrace}. Token names cannot be empty."));
            }

            if (!validTokens.Contains(tokenName))
            {
                invalidTokens.Add(tokenName);
            }

            index = closeBrace + 1;
        }

        if (invalidTokens.Count > 0)
        {
            var validList = string.Join(", ", validTokens.Select(t => $"{{{t}}}"));
            var invalidList = string.Join(", ", invalidTokens.Select(t => $"{{{t}}}"));

            return new Result<Unit>.Failure(
                Error.Create(ErrorCode.PatternValidationFailed,
                    $"Invalid tokens in pattern: {invalidList}. Valid tokens are: {validList}"));
        }

        return new Result<Unit>.Success(Unit.Value);
    }

    /// <summary>
    /// Processes a path pattern by replacing tokens with values from the provided dictionary.
    /// </summary>
    /// <param name="pattern">The pattern containing tokens to replace.</param>
    /// <param name="tokenValues">Dictionary mapping token names (without braces) to their values.</param>
    /// <returns>The processed path with tokens replaced and invalid characters sanitized.</returns>
    public static string Process(string pattern, IReadOnlyDictionary<string, string> tokenValues)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        ArgumentNullException.ThrowIfNull(tokenValues);

        var result = new StringBuilder(pattern.Length * 2);
        var index = 0;

        while (index < pattern.Length)
        {
            var openBrace = pattern.IndexOf('{', index);
            if (openBrace == -1)
            {
                // No more tokens, append the rest
                result.Append(pattern, index, pattern.Length - index);
                break;
            }

            // Append text before the token
            if (openBrace > index)
            {
                result.Append(pattern, index, openBrace - index);
            }

            var closeBrace = pattern.IndexOf('}', openBrace);
            if (closeBrace == -1)
            {
                // Unclosed brace - append as literal (validation should have caught this)
                result.Append(pattern, openBrace, pattern.Length - openBrace);
                break;
            }

            var tokenName = pattern[(openBrace + 1)..closeBrace];

            if (tokenValues.TryGetValue(tokenName, out var value))
            {
                // Sanitize the value for file system use
                result.Append(SanitizePathSegment(value));
            }
            else
            {
                // Token not found - keep as literal (validation should have caught this)
                result.Append(pattern, openBrace, closeBrace - openBrace + 1);
            }

            index = closeBrace + 1;
        }

        return NormalizePath(result.ToString());
    }

    /// <summary>
    /// Sanitizes a path segment by replacing invalid characters with underscores.
    /// </summary>
    /// <param name="segment">The path segment to sanitize.</param>
    /// <returns>The sanitized path segment.</returns>
    private static string SanitizePathSegment(string segment)
    {
        if (string.IsNullOrEmpty(segment))
            return segment;

        var result = new StringBuilder(segment.Length);

        foreach (var character in segment)
        {
            if (InvalidPathChars.Contains(character))
            {
                result.Append('_');
            }
            else
            {
                result.Append(character);
            }
        }

        // Trim leading/trailing whitespace and dots (invalid for Windows)
        var sanitized = result.ToString().Trim().TrimEnd('.');

        // Handle reserved Windows names
        return SanitizeReservedNames(sanitized);
    }

    /// <summary>
    /// Windows reserved file names (case-insensitive).
    /// </summary>
    private static readonly FrozenSet<string> ReservedWindowsNames = new[]
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Sanitizes reserved Windows file names by appending an underscore.
    /// </summary>
    private static string SanitizeReservedNames(string name)
    {
        // Check for exact match
        if (ReservedWindowsNames.Contains(name))
        {
            return name + "_";
        }

        // Check for reserved name with extension (e.g., "CON.txt")
        var dotIndex = name.IndexOf('.');
        if (dotIndex > 0 && ReservedWindowsNames.Contains(name[..dotIndex]))
        {
            return name + "_";
        }

        return name;
    }

    /// <summary>
    /// Normalizes directory separators for the current operating system.
    /// </summary>
    private static string NormalizePath(string path)
    {
        // Normalize directory separators
        var normalized = path
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);

        // Remove duplicate separators
        var separator = Path.DirectorySeparatorChar.ToString();
        var doubleSeparator = separator + separator;

        while (normalized.Contains(doubleSeparator, StringComparison.Ordinal))
        {
            normalized = normalized.Replace(doubleSeparator, separator, StringComparison.Ordinal);
        }

        return normalized;
    }
}
