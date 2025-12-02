namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;

/// <summary>
/// AOT-compatible JSON converter for <see cref="MediaType"/>.
/// </summary>
internal sealed class MediaTypeConverter : JsonConverter<MediaType>
{
    /// <inheritdoc />
    public override MediaType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(MediaType)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "image" => MediaType.Image,
            "video" => MediaType.Video,
            _ => throw new JsonException($"Unknown {nameof(MediaType)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, MediaType value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            MediaType.Image => "image",
            MediaType.Video => "video",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(MediaType)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
