namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;

/// <summary>
/// AOT-compatible JSON converter for <see cref="Availability"/>.
/// </summary>
internal sealed class AvailabilityConverter : JsonConverter<Availability>
{
    /// <inheritdoc />
    public override Availability Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(Availability)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "Public" => Availability.Public,
            "Private" => Availability.Private,
            "Archived" => Availability.Archived,
            "Unsearchable" => Availability.Unsearchable,
            _ => throw new JsonException($"Unknown {nameof(Availability)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Availability value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            Availability.Public => "Public",
            Availability.Private => "Private",
            Availability.Archived => "Archived",
            Availability.Unsearchable => "Unsearchable",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(Availability)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
