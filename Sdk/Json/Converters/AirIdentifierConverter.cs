namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Air;

/// <summary>
/// JSON converter for <see cref="AirIdentifier"/> that serializes to/from AIR strings.
/// </summary>
public sealed class AirIdentifierConverter : JsonConverter<AirIdentifier>
{
    /// <inheritdoc/>
    public override AirIdentifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("AIR identifier cannot be null or empty.");
        }

        if (!AirIdentifier.TryParse(value, out var result))
        {
            throw new JsonException($"Invalid AIR identifier format: '{value}'. Expected format: urn:air:{{ecosystem}}:{{type}}:{{source}}:{{modelId}}@{{versionId}}");
        }

        return result;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, AirIdentifier value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
