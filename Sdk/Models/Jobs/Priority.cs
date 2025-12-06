namespace CivitaiSharp.Sdk.Models.Jobs;

using System;
using System.Text.Json.Serialization;

/// <summary>
/// Priority configuration for job scheduling.
/// </summary>
/// <remarks>
/// This type is a struct for better memory efficiency as it contains only a single decimal value.
/// </remarks>
public readonly struct Priority
{
    /// <summary>
    /// The default priority modifier value.
    /// </summary>
    public const decimal DefaultModifier = 1.0m;

    /// <summary>
    /// The minimum allowed priority modifier value.
    /// </summary>
    public const decimal MinModifier = 0.1m;

    /// <summary>
    /// The maximum allowed priority modifier value.
    /// </summary>
    public const decimal MaxModifier = 10.0m;

    /// <summary>
    /// Initializes a new instance of the <see cref="Priority"/> struct with the default modifier.
    /// </summary>
    public Priority() => Modifier = DefaultModifier;

    /// <summary>
    /// Initializes a new instance of the <see cref="Priority"/> struct with the specified modifier.
    /// </summary>
    /// <param name="modifier">The priority modifier value. Must be between <see cref="MinModifier"/> and <see cref="MaxModifier"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="modifier"/> is less than <see cref="MinModifier"/> or greater than <see cref="MaxModifier"/>.
    /// </exception>
    public Priority(decimal modifier)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(modifier, MinModifier);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(modifier, MaxModifier);
        Modifier = modifier;
    }

    /// <summary>
    /// Gets the priority modifier. Higher values increase priority.
    /// Valid range: <see cref="MinModifier"/> to <see cref="MaxModifier"/>.
    /// </summary>
    [JsonPropertyName("modifier")]
    public decimal Modifier { get; init; }

    /// <summary>
    /// Gets the default priority configuration.
    /// </summary>
    public static Priority Default => new(DefaultModifier);
}
