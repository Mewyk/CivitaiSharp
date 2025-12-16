namespace CivitaiSharp.Sdk.Models.Jobs;

using System.Text.Json.Serialization;

/// <summary>
/// Priority configuration for job scheduling.
/// </summary>
/// <param name="Modifier">
/// The priority modifier value. Higher values increase priority. Maps to JSON property "modifier".
/// Range: 0.1-10.0, default: 1.0.
/// </param>
public sealed record Priority(
    [property: JsonPropertyName("modifier")] decimal Modifier)
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
    /// Gets the default priority configuration.
    /// </summary>
    public static Priority Default => new(DefaultModifier);
}
