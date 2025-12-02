namespace CivitaiSharp.Sdk.Models.Coverage;

using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Json.Converters;

/// <summary>
/// Availability information for a model or asset on the generation infrastructure.
/// </summary>
/// <param name="Availability">The availability status of the asset. Maps to JSON property "availability".</param>
/// <param name="Workers">
/// The number of workers that have this model loaded. Maps to JSON property "workers".
/// Higher worker counts indicate better availability and faster job processing.
/// A value of 0 means the model is unavailable.
/// </param>
public sealed record ProviderAssetAvailability(
    [property: JsonPropertyName("availability")]
    [property: JsonConverter(typeof(AvailabilityStatusConverter))]
    AvailabilityStatus Availability,

    [property: JsonPropertyName("workers")]
    int Workers);
