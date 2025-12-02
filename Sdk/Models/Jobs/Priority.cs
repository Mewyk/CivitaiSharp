namespace CivitaiSharp.Sdk.Models.Jobs;

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
    /// Initializes a new instance of the <see cref="Priority"/> struct with the default modifier.
    /// </summary>
    public Priority() => Modifier = DefaultModifier;

    /// <summary>
    /// Initializes a new instance of the <see cref="Priority"/> struct with the specified modifier.
    /// </summary>
    /// <param name="modifier">The priority modifier value.</param>
    public Priority(decimal modifier) => Modifier = modifier;

    /// <summary>
    /// Gets the priority modifier. Higher values increase priority.
    /// </summary>
    [JsonPropertyName("modifier")]
    public decimal Modifier { get; init; }

    /// <summary>
    /// Gets the default priority configuration.
    /// </summary>
    public static Priority Default => new(DefaultModifier);
}
