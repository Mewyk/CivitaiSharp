namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;

/// <summary>
/// AOT-compatible JSON converter for <see cref="ModelMode"/>.
/// </summary>
internal sealed class ModelModeConverter : JsonConverter<ModelMode>
{
    /// <inheritdoc />
    public override ModelMode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(ModelMode)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "Archived" => ModelMode.Archived,
            "TakenDown" => ModelMode.TakenDown,
            _ => throw new JsonException($"Unknown {nameof(ModelMode)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ModelMode value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ModelMode.Archived => "Archived",
            ModelMode.TakenDown => "TakenDown",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(ModelMode)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
