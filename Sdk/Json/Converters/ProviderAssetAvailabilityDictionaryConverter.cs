namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Models.Coverage;

/// <summary>
/// AOT-compatible JSON converter for <see cref="IReadOnlyDictionary{TKey, TValue}"/> where TKey is <see cref="AirIdentifier"/> and TValue is <see cref="ProviderAssetAvailability"/>.
/// </summary>
internal sealed class ProviderAssetAvailabilityDictionaryConverter : JsonConverter<IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability>>
{
    /// <inheritdoc />
    public override IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object for dictionary.");
        }

        var dictionary = new Dictionary<AirIdentifier, ProviderAssetAvailability>();

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

            var keyString = reader.GetString()!;
            if (!AirIdentifier.TryParse(keyString, out var key))
            {
                throw new JsonException($"Invalid AIR identifier key: '{keyString}'");
            }

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
        IReadOnlyDictionary<AirIdentifier, ProviderAssetAvailability> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key.ToString());
            JsonSerializer.Serialize(writer, kvp.Value, SdkJsonContext.Default.ProviderAssetAvailability);
        }

        writer.WriteEndObject();
    }
}
