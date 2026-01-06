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
            "Depth" => ControlNetPreprocessor.Depth,
            "DepthLeres" => ControlNetPreprocessor.DepthLeres,
            "DepthMidas" => ControlNetPreprocessor.DepthMidas,
            "DepthZoe" => ControlNetPreprocessor.DepthZoe,
            "SoftedgeHed" => ControlNetPreprocessor.SoftEdgeHed,
            "SoftedgePidinet" => ControlNetPreprocessor.SoftEdgePidinet,
            "Lineart" => ControlNetPreprocessor.Lineart,
            "LineartAnime" => ControlNetPreprocessor.LineartAnime,
            "Openpose" => ControlNetPreprocessor.Openpose,
            "OpenposeFace" => ControlNetPreprocessor.OpenposeFace,
            "OpenposeFull" => ControlNetPreprocessor.OpenposeFull,
            "MediapipeFace" => ControlNetPreprocessor.MediapipeFace,
            "NormalBae" => ControlNetPreprocessor.NormalBae,
            "Segmentation" => ControlNetPreprocessor.Segmentation,
            "Shuffle" => ControlNetPreprocessor.Shuffle,
            "Tile" => ControlNetPreprocessor.Tile,
            "Inpaint" => ControlNetPreprocessor.Inpaint,
            "Mlsd" => ControlNetPreprocessor.Mlsd,
            "Scribble" => ControlNetPreprocessor.Scribble,
            "Rembg" => ControlNetPreprocessor.Rembg,
            "None" => ControlNetPreprocessor.None,
            _ => throw new JsonException($"Unknown {nameof(ControlNetPreprocessor)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ControlNetPreprocessor value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ControlNetPreprocessor.Canny => "Canny",
            ControlNetPreprocessor.Depth => "Depth",
            ControlNetPreprocessor.DepthLeres => "DepthLeres",
            ControlNetPreprocessor.DepthMidas => "DepthMidas",
            ControlNetPreprocessor.DepthZoe => "DepthZoe",
            ControlNetPreprocessor.SoftEdgeHed => "SoftedgeHed",
            ControlNetPreprocessor.SoftEdgePidinet => "SoftedgePidinet",
            ControlNetPreprocessor.Lineart => "Lineart",
            ControlNetPreprocessor.LineartAnime => "LineartAnime",
            ControlNetPreprocessor.Openpose => "Openpose",
            ControlNetPreprocessor.OpenposeFace => "OpenposeFace",
            ControlNetPreprocessor.OpenposeFull => "OpenposeFull",
            ControlNetPreprocessor.MediapipeFace => "MediapipeFace",
            ControlNetPreprocessor.NormalBae => "NormalBae",
            ControlNetPreprocessor.Segmentation => "Segmentation",
            ControlNetPreprocessor.Shuffle => "Shuffle",
            ControlNetPreprocessor.Tile => "Tile",
            ControlNetPreprocessor.Inpaint => "Inpaint",
            ControlNetPreprocessor.Mlsd => "Mlsd",
            ControlNetPreprocessor.Scribble => "Scribble",
            ControlNetPreprocessor.Rembg => "Rembg",
            ControlNetPreprocessor.None => "None",
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
