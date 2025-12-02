namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;

/// <summary>
/// AOT-compatible JSON converter for <see cref="ModelType"/>.
/// </summary>
internal sealed class ModelTypeConverter : JsonConverter<ModelType>
{
    /// <inheritdoc />
    public override ModelType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(ModelType)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "Checkpoint" => ModelType.Checkpoint,
            "TextualInversion" => ModelType.TextualInversion,
            "Hypernetwork" => ModelType.Hypernetwork,
            "AestheticGradient" => ModelType.AestheticGradient,
            "LORA" => ModelType.Lora,
            "LoCon" => ModelType.LoCon,
            "DoRA" => ModelType.DoRa,
            "Controlnet" => ModelType.Controlnet,
            "Poses" => ModelType.Poses,
            "Upscaler" => ModelType.Upscaler,
            "MotionModule" => ModelType.MotionModule,
            "VAE" => ModelType.Vae,
            "Wildcards" => ModelType.Wildcards,
            "Workflows" => ModelType.Workflows,
            "Other" => ModelType.Other,
            _ => throw new JsonException($"Unknown {nameof(ModelType)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ModelType value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ModelType.Checkpoint => "Checkpoint",
            ModelType.TextualInversion => "TextualInversion",
            ModelType.Hypernetwork => "Hypernetwork",
            ModelType.AestheticGradient => "AestheticGradient",
            ModelType.Lora => "LORA",
            ModelType.LoCon => "LoCon",
            ModelType.DoRa => "DoRA",
            ModelType.Controlnet => "Controlnet",
            ModelType.Poses => "Poses",
            ModelType.Upscaler => "Upscaler",
            ModelType.MotionModule => "MotionModule",
            ModelType.Vae => "VAE",
            ModelType.Wildcards => "Wildcards",
            ModelType.Workflows => "Workflows",
            ModelType.Other => "Other",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(ModelType)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
