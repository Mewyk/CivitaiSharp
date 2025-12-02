using System.Text.RegularExpressions;

namespace CivitaiSharp.Core.Validation;

/// <summary>
/// Provides validation methods for Civitai-specific input constraints.
/// </summary>
public static partial class CivitaiValidation
{
    /// <summary>
    /// Regular expression pattern for valid Civitai usernames.
    /// Usernames can only contain letters, numbers, and underscores.
    /// </summary>
    private const string UsernamePattern = @"^[a-zA-Z0-9_]+$";

    [GeneratedRegex(UsernamePattern)]
    private static partial Regex UsernameRegex();

    /// <summary>
    /// Validates that a username conforms to Civitai's username requirements.
    /// Usernames can only contain letters (a-z, A-Z), numbers (0-9), and underscores (_).
    /// </summary>
    /// <param name="username">The username to validate.</param>
    /// <returns><c>true</c> if the username is valid; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This validation is performed client-side to provide early feedback.
    /// The server may have additional validation rules.
    /// </remarks>
    public static bool IsValidUsername(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }

        return UsernameRegex().IsMatch(username);
    }

    /// <summary>
    /// Validates that a username conforms to Civitai's username requirements and throws if invalid.
    /// </summary>
    /// <param name="username">The username to validate.</param>
    /// <param name="parameterName">The name of the parameter (for the exception).</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the username is null, whitespace, or contains invalid characters.
    /// </exception>
    public static void ThrowIfInvalidUsername(string? username, string? parameterName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username, parameterName);

        if (!UsernameRegex().IsMatch(username))
        {
            throw new ArgumentException(
                "Username can only contain letters, numbers, and underscores.",
                parameterName);
        }
    }
}
