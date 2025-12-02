namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// AOT-compatible JSON converter for <see cref="NetworkType"/>.
/// </summary>
internal sealed class NetworkTypeConverter : JsonConverter<NetworkType>
{
    /// <inheritdoc />
    public override NetworkType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(NetworkType)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "lora" => NetworkType.Lora,
            "lycoris" => NetworkType.Lycoris,
            "dora" => NetworkType.Dora,
            "embedding" => NetworkType.Embedding,
            "vae" => NetworkType.Vae,
            _ => throw new JsonException($"Unknown {nameof(NetworkType)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, NetworkType value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            NetworkType.Lora => "lora",
            NetworkType.Lycoris => "lycoris",
            NetworkType.Dora => "dora",
            NetworkType.Embedding => "embedding",
            NetworkType.Vae => "vae",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(NetworkType)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
