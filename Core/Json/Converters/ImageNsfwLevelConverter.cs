namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;

/// <summary>
/// AOT-compatible JSON converter for <see cref="ImageNsfwLevel"/>.
/// </summary>
internal sealed class ImageNsfwLevelConverter : JsonConverter<ImageNsfwLevel>
{
    /// <inheritdoc />
    public override ImageNsfwLevel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(ImageNsfwLevel)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "None" => ImageNsfwLevel.None,
            "Soft" => ImageNsfwLevel.Soft,
            "Mature" => ImageNsfwLevel.Mature,
            "X" => ImageNsfwLevel.Explicit,
            _ => throw new JsonException($"Unknown {nameof(ImageNsfwLevel)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ImageNsfwLevel value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ImageNsfwLevel.None => "None",
            ImageNsfwLevel.Soft => "Soft",
            ImageNsfwLevel.Mature => "Mature",
            ImageNsfwLevel.Explicit => "X",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(ImageNsfwLevel)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
