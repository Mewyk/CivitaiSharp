namespace CivitaiSharp.Core.Extensions;

using System;
using System.Runtime.CompilerServices;

/// <summary>
/// Extension methods for enum types to support API string conversion and validation.
/// </summary>
/// <remarks>
/// This implementation is AOT-compatible and does not use reflection.
/// All enum-to-string mappings are defined in <see cref="ApiStringRegistry"/>.
/// </remarks>
public static class EnumExtensions
{
    /// <summary>
    /// Converts an enum value to its API request parameter string representation.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The enum value to convert.</param>
    /// <returns>The API string representation of the enum value.</returns>
    /// <remarks>
    /// Uses pre-defined frozen dictionary mappings from <see cref="ApiStringRegistry"/>
    /// for optimal performance. Falls back to the enum member name if no mapping exists.
    /// </remarks>
    public static string ToApiString<TEnum>(this TEnum value)
        where TEnum : struct, Enum
    {
        return ApiStringRegistry.ToApiString(value);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the enum value is not defined.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The enum value to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated (provided automatically by the compiler).</param>
    /// <returns>The validated enum value for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is not a defined enum member.</exception>
    public static TEnum ThrowIfUndefined<TEnum>(
        this TEnum value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                $"Value is not a valid {typeof(TEnum).Name}.");
        }
        return value;
    }

    /// <summary>
    /// Attempts to parse an API string value to the corresponding enum value.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to parse to.</typeparam>
    /// <param name="value">The API string value to parse.</param>
    /// <param name="result">When this method returns, contains the parsed enum value if successful.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    /// <remarks>
    /// Matches against the API string mappings in <see cref="ApiStringRegistry"/> first (case-insensitive),
    /// then falls back to standard enum parsing by member name.
    /// </remarks>
    public static bool TryParseFromApiString<TEnum>(string? value, out TEnum result)
        where TEnum : struct, Enum
    {
        return ApiStringRegistry.TryParseFromApiString(value, out result);
    }
}
