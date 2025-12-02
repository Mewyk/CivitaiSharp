namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;

/// <summary>
/// AOT-compatible JSON converter for <see cref="IReadOnlyList{CommercialUsePermission}"/> that handles
/// the Civitai API's set-like string format (e.g., "{Image,RentCivit,Rent}").
/// </summary>
/// <remarks>
/// The Civitai API returns commercial use permissions in a non-standard format that
/// looks like a set literal rather than a JSON array. This converter parses both
/// the set-like string format and standard JSON arrays for compatibility.
/// </remarks>
internal sealed class CommercialUsePermissionListConverter : JsonConverter<IReadOnlyList<CommercialUsePermission>?>
{
    /// <inheritdoc />
    public override IReadOnlyList<CommercialUsePermission>? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => ParseSetString(reader.GetString()),
            JsonTokenType.StartArray => ParseArray(ref reader),
            _ => throw new JsonException(
                $"Expected string, array, or null for CommercialUsePermission list, but found {reader.TokenType}.")
        };
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        IReadOnlyList<CommercialUsePermission>? value,
        JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (var permission in value)
        {
            var stringValue = permission switch
            {
                CommercialUsePermission.None => "None",
                CommercialUsePermission.Image => "Image",
                CommercialUsePermission.Rent => "Rent",
                CommercialUsePermission.RentCivit => "RentCivit",
                CommercialUsePermission.Sell => "Sell",
                _ => throw new ArgumentOutOfRangeException(nameof(value), permission, 
                    $"Unknown {nameof(CommercialUsePermission)} value.")
            };
            writer.WriteStringValue(stringValue);
        }
        writer.WriteEndArray();
    }

    /// <summary>
    /// Parses the set-like string format (e.g., "{Image,RentCivit,Rent}").
    /// </summary>
    private static IReadOnlyList<CommercialUsePermission>? ParseSetString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        // Remove curly braces if present: "{Image,Rent}" -> "Image,Rent"
        var trimmed = value.Trim();
        if (trimmed.StartsWith('{') && trimmed.EndsWith('}'))
        {
            trimmed = trimmed[1..^1];
        }

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return [];
        }

        var parts = trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new List<CommercialUsePermission>(parts.Length);

        foreach (var part in parts)
        {
            result.Add(ParsePermission(part));
        }

        return result;
    }

    /// <summary>
    /// Parses a standard JSON array of permission values.
    /// </summary>
    private static IReadOnlyList<CommercialUsePermission> ParseArray(ref Utf8JsonReader reader)
    {
        var result = new List<CommercialUsePermission>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                if (value is not null)
                {
                    result.Add(ParsePermission(value));
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Parses a string value to a <see cref="CommercialUsePermission"/>.
    /// </summary>
    private static CommercialUsePermission ParsePermission(string value)
    {
        return value switch
        {
            "None" => CommercialUsePermission.None,
            "Image" => CommercialUsePermission.Image,
            "Rent" => CommercialUsePermission.Rent,
            "RentCivit" => CommercialUsePermission.RentCivit,
            "Sell" => CommercialUsePermission.Sell,
            _ => throw new JsonException($"Unknown {nameof(CommercialUsePermission)} value: '{value}'.")
        };
    }
}
