namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Models.Coverage;

/// <summary>
/// AOT-compatible JSON converter for <see cref="IReadOnlyDictionary{TKey, TValue}"/> where TValue is <see cref="ProviderAssetAvailability"/>.
/// </summary>
internal sealed class ProviderAssetAvailabilityDictionaryConverter : JsonConverter<IReadOnlyDictionary<string, ProviderAssetAvailability>>
{
    /// <inheritdoc />
    public override IReadOnlyDictionary<string, ProviderAssetAvailability> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object for dictionary.");
        }

        var dictionary = new Dictionary<string, ProviderAssetAvailability>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected property name.");
            }

            var key = reader.GetString()!;
            reader.Read();

            var availability = JsonSerializer.Deserialize(ref reader, SdkJsonContext.Default.ProviderAssetAvailability);
            if (availability is not null)
            {
                dictionary[key] = availability;
            }
        }

        return dictionary;
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        IReadOnlyDictionary<string, ProviderAssetAvailability> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key);
            JsonSerializer.Serialize(writer, kvp.Value, SdkJsonContext.Default.ProviderAssetAvailability);
        }

        writer.WriteEndObject();
    }
}
