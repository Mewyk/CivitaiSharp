namespace CivitaiSharp.Sdk.Models.Jobs;

using System.Text.Json.Serialization;

/// <summary>
/// Priority configuration for job scheduling.
/// </summary>
[method: JsonConstructor]
public readonly struct Priority(decimal modifier)
{
    /// <summary>
    /// Gets the priority modifier. Higher values increase priority.
    /// </summary>
    [JsonPropertyName("modifier")]
    public decimal Modifier { get; init; } = modifier;

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
    /// Gets the default priority configuration.
    /// </summary>
    public static Priority Default => new(DefaultModifier);
}
