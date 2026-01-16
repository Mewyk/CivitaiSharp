namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// AOT-compatible JSON converter for <see cref="JobSupport"/>.
/// This converter is strict and will throw if the API returns an unknown value.
/// </summary>
internal sealed class JobSupportConverter : JsonConverter<JobSupport>
{
    /// <inheritdoc />
    public override JobSupport Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException($"Null value is not valid for {nameof(JobSupport)}.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(JobSupport)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "Unsupported" => JobSupport.Unsupported,
            "Unavailable" => JobSupport.Unavailable,
            "Available" => JobSupport.Available,
            _ => throw new JsonException($"Unknown {nameof(JobSupport)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, JobSupport value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            JobSupport.Unsupported => "Unsupported",
            JobSupport.Unavailable => "Unavailable",
            JobSupport.Available => "Available",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(JobSupport)} value.")
        };

        writer.WriteStringValue(stringValue);
    }
}
