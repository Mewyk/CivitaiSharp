namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;

/// <summary>
/// AOT-compatible JSON converter for <see cref="TimePeriod"/>.
/// </summary>
internal sealed class TimePeriodConverter : JsonConverter<TimePeriod>
{
    /// <inheritdoc />
    public override TimePeriod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(TimePeriod)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
        return value switch
        {
            "AllTime" => TimePeriod.AllTime,
            "Year" => TimePeriod.Year,
            "Month" => TimePeriod.Month,
            "Week" => TimePeriod.Week,
            "Day" => TimePeriod.Day,
            _ => throw new JsonException($"Unknown {nameof(TimePeriod)} value: '{value}'.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TimePeriod value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            TimePeriod.AllTime => "AllTime",
            TimePeriod.Year => "Year",
            TimePeriod.Month => "Month",
            TimePeriod.Week => "Week",
            TimePeriod.Day => "Day",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(TimePeriod)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
