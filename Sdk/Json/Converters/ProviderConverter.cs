namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// AOT-compatible JSON converter for <see cref="Provider"/>.
/// This converter is strict and will throw if the API returns an unknown value.
/// </summary>
internal sealed class ProviderConverter : JsonConverter<Provider>
{
    /// <inheritdoc />
    public override Provider Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException($"Null value is not valid for {nameof(Provider)}.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(Provider)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "Civitai" => Provider.Civitai,
            "OctoML" => Provider.OctoML,
            "SaladML" => Provider.SaladML,
            "PicFinder" => Provider.PicFinder,
            "RunPods" => Provider.RunPods,
            "ValdiAI" => Provider.ValdiAI,
            "OctoMLNext" => Provider.OctoMLNext,
            "RunDiffusion" => Provider.RunDiffusion,
            "SaladShared" => Provider.SaladShared,
            _ => throw new JsonException($"Unknown {nameof(Provider)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Provider value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            Provider.Civitai => "Civitai",
            Provider.OctoML => "OctoML",
            Provider.SaladML => "SaladML",
            Provider.PicFinder => "PicFinder",
            Provider.RunPods => "RunPods",
            Provider.ValdiAI => "ValdiAI",
            Provider.OctoMLNext => "OctoMLNext",
            Provider.RunDiffusion => "RunDiffusion",
            Provider.SaladShared => "SaladShared",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(Provider)} value.")
        };

        writer.WriteStringValue(stringValue);
    }
}
