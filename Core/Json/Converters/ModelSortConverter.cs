namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;

/// <summary>
/// AOT-compatible JSON converter for <see cref="ModelSort"/>.
/// </summary>
internal sealed class ModelSortConverter : JsonConverter<ModelSort>
{
    /// <inheritdoc />
    public override ModelSort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(ModelSort)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "Highest Rated" => ModelSort.HighestRated,
            "Most Downloaded" => ModelSort.MostDownloaded,
            "Newest" => ModelSort.Newest,
            _ => throw new JsonException($"Unknown {nameof(ModelSort)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ModelSort value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ModelSort.HighestRated => "Highest Rated",
            ModelSort.MostDownloaded => "Most Downloaded",
            ModelSort.Newest => "Newest",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(ModelSort)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
