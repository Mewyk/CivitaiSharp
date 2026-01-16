namespace CivitaiSharp.Sdk.Json.Converters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Models.Results;

/// <summary>
/// AOT-compatible JSON converter for <see cref="JobResult"/>.
/// The API's job result can be an arbitrary JSON value depending on job type.
/// We preserve the raw JSON and expose common blob properties as conveniences.
/// </summary>
internal sealed class JobResultConverter : JsonConverter<JobResult>
{
    /// <inheritdoc />
    public override JobResult? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        using var document = JsonDocument.ParseValue(ref reader);
        return new JobResult(document.RootElement.Clone());
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, JobResult value, JsonSerializerOptions options)
    {
        if (value.Raw.ValueKind == JsonValueKind.Undefined)
        {
            writer.WriteNullValue();
            return;
        }

        value.Raw.WriteTo(writer);
    }
}
