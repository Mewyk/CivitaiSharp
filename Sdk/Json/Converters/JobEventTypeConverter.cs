namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Enums;

/// <summary>
/// AOT-compatible JSON converter for <see cref="JobEventType"/>.
/// This converter is strict and will throw if the API returns an unknown value.
/// </summary>
internal sealed class JobEventTypeConverter : JsonConverter<JobEventType>
{
    /// <inheritdoc />
    public override JobEventType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException($"Null value is not valid for {nameof(JobEventType)}.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(JobEventType)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "Initialized" => JobEventType.Initialized,
            "Claimed" => JobEventType.Claimed,
            "Rejected" => JobEventType.Rejected,
            "LateRejected" => JobEventType.LateRejected,
            "ClaimExpired" => JobEventType.ClaimExpired,
            "Updated" => JobEventType.Updated,
            "Failed" => JobEventType.Failed,
            "Succeeded" => JobEventType.Succeeded,
            "Expired" => JobEventType.Expired,
            "Deleted" => JobEventType.Deleted,
            _ => throw new JsonException($"Unknown {nameof(JobEventType)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, JobEventType value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            JobEventType.Initialized => "Initialized",
            JobEventType.Claimed => "Claimed",
            JobEventType.Rejected => "Rejected",
            JobEventType.LateRejected => "LateRejected",
            JobEventType.ClaimExpired => "ClaimExpired",
            JobEventType.Updated => "Updated",
            JobEventType.Failed => "Failed",
            JobEventType.Succeeded => "Succeeded",
            JobEventType.Expired => "Expired",
            JobEventType.Deleted => "Deleted",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(JobEventType)} value.")
        };

        writer.WriteStringValue(stringValue);
    }
}
