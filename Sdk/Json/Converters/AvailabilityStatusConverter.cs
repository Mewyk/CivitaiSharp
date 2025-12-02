namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// AOT-compatible JSON converter for <see cref="AvailabilityStatus"/>.
/// </summary>
internal sealed class AvailabilityStatusConverter : JsonConverter<AvailabilityStatus>
{
    /// <inheritdoc />
    public override AvailabilityStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(AvailabilityStatus)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "Available" => AvailabilityStatus.Available,
            "Unavailable" => AvailabilityStatus.Unavailable,
            "Degraded" => AvailabilityStatus.Degraded,
            _ => throw new JsonException($"Unknown {nameof(AvailabilityStatus)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, AvailabilityStatus value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            AvailabilityStatus.Available => "Available",
            AvailabilityStatus.Unavailable => "Unavailable",
            AvailabilityStatus.Degraded => "Degraded",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(AvailabilityStatus)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
