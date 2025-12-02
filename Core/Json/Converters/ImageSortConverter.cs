namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;

/// <summary>
/// AOT-compatible JSON converter for <see cref="ImageSort"/>.
/// </summary>
internal sealed class ImageSortConverter : JsonConverter<ImageSort>
{
    /// <inheritdoc />
    public override ImageSort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(ImageSort)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "Most Reactions" => ImageSort.MostReactions,
            "Most Comments" => ImageSort.MostComments,
            "Most Collected" => ImageSort.MostCollected,
            "Newest" => ImageSort.Newest,
            "Oldest" => ImageSort.Oldest,
            "Random" => ImageSort.Random,
            _ => throw new JsonException($"Unknown {nameof(ImageSort)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ImageSort value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ImageSort.MostReactions => "Most Reactions",
            ImageSort.MostComments => "Most Comments",
            ImageSort.MostCollected => "Most Collected",
            ImageSort.Newest => "Newest",
            ImageSort.Oldest => "Oldest",
            ImageSort.Random => "Random",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(ImageSort)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
