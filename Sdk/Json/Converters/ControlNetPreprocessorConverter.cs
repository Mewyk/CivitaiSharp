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
            "canny" => ControlNetPreprocessor.Canny,
            "depth" => ControlNetPreprocessor.Depth,
            "depth_leres" => ControlNetPreprocessor.DepthLeres,
            "depth_midas" => ControlNetPreprocessor.DepthMidas,
            "depth_zoe" => ControlNetPreprocessor.DepthZoe,
            "softedge_hed" => ControlNetPreprocessor.SoftEdgeHed,
            "softedge_pidinet" => ControlNetPreprocessor.SoftEdgePidinet,
            "lineart" => ControlNetPreprocessor.Lineart,
            "lineart_anime" => ControlNetPreprocessor.LineartAnime,
            "openpose" => ControlNetPreprocessor.Openpose,
            "openpose_face" => ControlNetPreprocessor.OpenposeFace,
            "openpose_full" => ControlNetPreprocessor.OpenposeFull,
            "mediapipe_face" => ControlNetPreprocessor.MediapipeFace,
            "normal_bae" => ControlNetPreprocessor.NormalBae,
            "seg" => ControlNetPreprocessor.Segmentation,
            "shuffle" => ControlNetPreprocessor.Shuffle,
            "tile" => ControlNetPreprocessor.Tile,
            "inpaint" => ControlNetPreprocessor.Inpaint,
            "mlsd" => ControlNetPreprocessor.Mlsd,
            "scribble" => ControlNetPreprocessor.Scribble,
            "rembg" => ControlNetPreprocessor.Rembg,
            "none" => ControlNetPreprocessor.None,
            _ => throw new JsonException($"Unknown {nameof(ControlNetPreprocessor)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ControlNetPreprocessor value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ControlNetPreprocessor.Canny => "canny",
            ControlNetPreprocessor.Depth => "depth",
            ControlNetPreprocessor.DepthLeres => "depth_leres",
            ControlNetPreprocessor.DepthMidas => "depth_midas",
            ControlNetPreprocessor.DepthZoe => "depth_zoe",
            ControlNetPreprocessor.SoftEdgeHed => "softedge_hed",
            ControlNetPreprocessor.SoftEdgePidinet => "softedge_pidinet",
            ControlNetPreprocessor.Lineart => "lineart",
            ControlNetPreprocessor.LineartAnime => "lineart_anime",
            ControlNetPreprocessor.Openpose => "openpose",
            ControlNetPreprocessor.OpenposeFace => "openpose_face",
            ControlNetPreprocessor.OpenposeFull => "openpose_full",
            ControlNetPreprocessor.MediapipeFace => "mediapipe_face",
            ControlNetPreprocessor.NormalBae => "normal_bae",
            ControlNetPreprocessor.Segmentation => "seg",
            ControlNetPreprocessor.Shuffle => "shuffle",
            ControlNetPreprocessor.Tile => "tile",
            ControlNetPreprocessor.Inpaint => "inpaint",
            ControlNetPreprocessor.Mlsd => "mlsd",
            ControlNetPreprocessor.Scribble => "scribble",
            ControlNetPreprocessor.Rembg => "rembg",
            ControlNetPreprocessor.None => "none",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(ControlNetPreprocessor)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}

/// <summary>
/// AOT-compatible JSON converter for nullable <see cref="ControlNetPreprocessor"/>.
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
