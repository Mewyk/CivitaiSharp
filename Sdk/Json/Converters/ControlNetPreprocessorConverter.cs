namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// AOT-compatible JSON converter for <see cref="ControlNetPreprocessor"/>.
/// </summary>
internal sealed class ControlNetPreprocessorConverter : JsonConverter<ControlNetPreprocessor>
{
    /// <inheritdoc />
    public override ControlNetPreprocessor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(ControlNetPreprocessor)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "Canny" => ControlNetPreprocessor.Canny,
            "DepthZoe" => ControlNetPreprocessor.DepthZoe,
            "SoftedgePidinet" => ControlNetPreprocessor.SoftEdgePidinet,
            "Rembg" => ControlNetPreprocessor.Rembg,
            _ => throw new JsonException($"Unknown {nameof(ControlNetPreprocessor)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ControlNetPreprocessor value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ControlNetPreprocessor.Canny => "Canny",
            ControlNetPreprocessor.DepthZoe => "DepthZoe",
            ControlNetPreprocessor.SoftEdgePidinet => "SoftedgePidinet",
            ControlNetPreprocessor.Rembg => "Rembg",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(ControlNetPreprocessor)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}

/// <summary>
/// AOT-compatible JSON converter for nullable <see cref="ControlNetPreprocessor"/>.
/// Handles null values by writing JSON null and reading null tokens appropriately.
/// </summary>
internal sealed class NullableControlNetPreprocessorConverter : JsonConverter<ControlNetPreprocessor?>
{
    private static readonly ControlNetPreprocessorConverter InnerConverter = new();

    /// <inheritdoc />
    public override ControlNetPreprocessor? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return InnerConverter.Read(ref reader, typeof(ControlNetPreprocessor), options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ControlNetPreprocessor? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            InnerConverter.Write(writer, value.Value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
