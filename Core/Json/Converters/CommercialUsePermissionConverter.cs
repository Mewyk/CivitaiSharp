namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;

/// <summary>
/// AOT-compatible JSON converter for <see cref="CommercialUsePermission"/>.
/// </summary>
internal sealed class CommercialUsePermissionConverter : JsonConverter<CommercialUsePermission>
{
    /// <inheritdoc />
    public override CommercialUsePermission Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {nameof(CommercialUsePermission)}, got {reader.TokenType}.");
        }

        var value = reader.GetString();
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

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, CommercialUsePermission value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            CommercialUsePermission.None => "None",
            CommercialUsePermission.Image => "Image",
            CommercialUsePermission.Rent => "Rent",
            CommercialUsePermission.RentCivit => "RentCivit",
            CommercialUsePermission.Sell => "Sell",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(CommercialUsePermission)} value.")
        };
        writer.WriteStringValue(stringValue);
    }
}
