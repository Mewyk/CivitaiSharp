namespace CivitaiSharp.Sdk.Models.Results;

using System.Text.Json.Serialization;

/// <summary>
/// A JSON representation of a .NET <see cref="System.TimeSpan"/> returned by the Civitai API.
/// </summary>
/// <remarks>
/// The API returns a structured object (ticks plus derived components), not the ISO-8601 duration string.
/// This avoids ambiguity and is easy to consume in many languages.
/// </remarks>
public sealed record TimeSpanDetails(
    [property: JsonPropertyName("ticks")] long? Ticks,
    [property: JsonPropertyName("days")] int? Days,
    [property: JsonPropertyName("hours")] int? Hours,
    [property: JsonPropertyName("milliseconds")] int? Milliseconds,
    [property: JsonPropertyName("microseconds")] int? Microseconds,
    [property: JsonPropertyName("nanoseconds")] int? Nanoseconds,
    [property: JsonPropertyName("minutes")] int? Minutes,
    [property: JsonPropertyName("seconds")] int? Seconds,
    [property: JsonPropertyName("totalDays")] double? TotalDays,
    [property: JsonPropertyName("totalHours")] double? TotalHours,
    [property: JsonPropertyName("totalMilliseconds")] double? TotalMilliseconds,
    [property: JsonPropertyName("totalMicroseconds")] double? TotalMicroseconds,
    [property: JsonPropertyName("totalNanoseconds")] double? TotalNanoseconds,
    [property: JsonPropertyName("totalMinutes")] double? TotalMinutes,
    [property: JsonPropertyName("totalSeconds")] double? TotalSeconds);
