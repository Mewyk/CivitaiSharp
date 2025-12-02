namespace CivitaiSharp.Core.Json.Converters;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Models.Common;
using CivitaiSharp.Core.Services;

/// <summary>
/// AOT-compatible JSON converter for <see cref="PagedApiResponse{Model}"/>.
/// Handles both wrapped responses and plain array responses.
/// </summary>
internal sealed class PagedModelResponseConverter : JsonConverter<PagedApiResponse<Model>>
{
    /// <inheritdoc />
    public override PagedApiResponse<Model>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        return root.ValueKind switch
        {
            JsonValueKind.Array => new PagedApiResponse<Model>(Items: DeserializeItems(root) ?? []),
            JsonValueKind.Object => DeserializeObject(root),
            _ => throw new JsonException($"Expected JSON array or object for PagedApiResponse<Model>, but found {root.ValueKind}.")
        };
    }

    private static List<Model>? DeserializeItems(JsonElement element) =>
        JsonSerializer.Deserialize(element.GetRawText(), CivitaiJsonContext.Default.ListModel);

    private static PaginationMetadata? DeserializeMetadata(JsonElement element) =>
        JsonSerializer.Deserialize(element.GetRawText(), CivitaiJsonContext.Default.PaginationMetadata);

    private static PagedApiResponse<Model> DeserializeObject(JsonElement root)
    {
        IReadOnlyList<Model>? items = null;
        PaginationMetadata? metadata = null;

        foreach (var property in root.EnumerateObject())
        {
            switch (property.Name)
            {
                case "items":
                    items = DeserializeItems(property.Value);
                    break;
                case "metadata":
                    metadata = DeserializeMetadata(property.Value);
                    break;
            }
        }

        return new PagedApiResponse<Model>(items ?? [], metadata);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, PagedApiResponse<Model> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("items");
        JsonSerializer.Serialize(writer, value.Items, CivitaiJsonContext.Default.IReadOnlyListModel);
        if (value.Metadata is not null)
        {
            writer.WritePropertyName("metadata");
            JsonSerializer.Serialize(writer, value.Metadata, CivitaiJsonContext.Default.PaginationMetadata);
        }
        writer.WriteEndObject();
    }
}

/// <summary>
/// AOT-compatible JSON converter for <see cref="PagedApiResponse{Image}"/>.
/// Handles both wrapped responses and plain array responses.
/// </summary>
internal sealed class PagedImageResponseConverter : JsonConverter<PagedApiResponse<Image>>
{
    /// <inheritdoc />
    public override PagedApiResponse<Image>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        return root.ValueKind switch
        {
            JsonValueKind.Array => new PagedApiResponse<Image>(Items: DeserializeItems(root) ?? []),
            JsonValueKind.Object => DeserializeObject(root),
            _ => throw new JsonException($"Expected JSON array or object for PagedApiResponse<Image>, but found {root.ValueKind}.")
        };
    }

    private static List<Image>? DeserializeItems(JsonElement element) =>
        JsonSerializer.Deserialize(element.GetRawText(), CivitaiJsonContext.Default.ListImage);

    private static PaginationMetadata? DeserializeMetadata(JsonElement element) =>
        JsonSerializer.Deserialize(element.GetRawText(), CivitaiJsonContext.Default.PaginationMetadata);

    private static PagedApiResponse<Image> DeserializeObject(JsonElement root)
    {
        IReadOnlyList<Image>? items = null;
        PaginationMetadata? metadata = null;

        foreach (var property in root.EnumerateObject())
        {
            switch (property.Name)
            {
                case "items":
                    items = DeserializeItems(property.Value);
                    break;
                case "metadata":
                    metadata = DeserializeMetadata(property.Value);
                    break;
            }
        }

        return new PagedApiResponse<Image>(items ?? [], metadata);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, PagedApiResponse<Image> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("items");
        JsonSerializer.Serialize(writer, value.Items, CivitaiJsonContext.Default.IReadOnlyListImage);
        if (value.Metadata is not null)
        {
            writer.WritePropertyName("metadata");
            JsonSerializer.Serialize(writer, value.Metadata, CivitaiJsonContext.Default.PaginationMetadata);
        }
        writer.WriteEndObject();
    }
}

/// <summary>
/// AOT-compatible JSON converter for <see cref="PagedApiResponse{Creator}"/>.
/// Handles both wrapped responses and plain array responses.
/// </summary>
internal sealed class PagedCreatorResponseConverter : JsonConverter<PagedApiResponse<Creator>>
{
    /// <inheritdoc />
    public override PagedApiResponse<Creator>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        return root.ValueKind switch
        {
            JsonValueKind.Array => new PagedApiResponse<Creator>(Items: DeserializeItems(root) ?? []),
            JsonValueKind.Object => DeserializeObject(root),
            _ => throw new JsonException($"Expected JSON array or object for PagedApiResponse<Creator>, but found {root.ValueKind}.")
        };
    }

    private static List<Creator>? DeserializeItems(JsonElement element) =>
        JsonSerializer.Deserialize(element.GetRawText(), CivitaiJsonContext.Default.ListCreator);

    private static PaginationMetadata? DeserializeMetadata(JsonElement element) =>
        JsonSerializer.Deserialize(element.GetRawText(), CivitaiJsonContext.Default.PaginationMetadata);

    private static PagedApiResponse<Creator> DeserializeObject(JsonElement root)
    {
        IReadOnlyList<Creator>? items = null;
        PaginationMetadata? metadata = null;

        foreach (var property in root.EnumerateObject())
        {
            switch (property.Name)
            {
                case "items":
                    items = DeserializeItems(property.Value);
                    break;
                case "metadata":
                    metadata = DeserializeMetadata(property.Value);
                    break;
            }
        }

        return new PagedApiResponse<Creator>(items ?? [], metadata);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, PagedApiResponse<Creator> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("items");
        JsonSerializer.Serialize(writer, value.Items, CivitaiJsonContext.Default.IReadOnlyListCreator);
        if (value.Metadata is not null)
        {
            writer.WritePropertyName("metadata");
            JsonSerializer.Serialize(writer, value.Metadata, CivitaiJsonContext.Default.PaginationMetadata);
        }
        writer.WriteEndObject();
    }
}

/// <summary>
/// AOT-compatible JSON converter for <see cref="PagedApiResponse{Tag}"/>.
/// Handles both wrapped responses and plain array responses.
/// </summary>
internal sealed class PagedTagResponseConverter : JsonConverter<PagedApiResponse<Tag>>
{
    /// <inheritdoc />
    public override PagedApiResponse<Tag>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        return root.ValueKind switch
        {
            JsonValueKind.Array => new PagedApiResponse<Tag>(Items: DeserializeItems(root) ?? []),
            JsonValueKind.Object => DeserializeObject(root),
            _ => throw new JsonException($"Expected JSON array or object for PagedApiResponse<Tag>, but found {root.ValueKind}.")
        };
    }

    private static List<Tag>? DeserializeItems(JsonElement element) =>
        JsonSerializer.Deserialize(element.GetRawText(), CivitaiJsonContext.Default.ListTag);

    private static PaginationMetadata? DeserializeMetadata(JsonElement element) =>
        JsonSerializer.Deserialize(element.GetRawText(), CivitaiJsonContext.Default.PaginationMetadata);

    private static PagedApiResponse<Tag> DeserializeObject(JsonElement root)
    {
        IReadOnlyList<Tag>? items = null;
        PaginationMetadata? metadata = null;

        foreach (var property in root.EnumerateObject())
        {
            switch (property.Name)
            {
                case "items":
                    items = DeserializeItems(property.Value);
                    break;
                case "metadata":
                    metadata = DeserializeMetadata(property.Value);
                    break;
            }
        }

        return new PagedApiResponse<Tag>(items ?? [], metadata);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, PagedApiResponse<Tag> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("items");
        JsonSerializer.Serialize(writer, value.Items, CivitaiJsonContext.Default.IReadOnlyListTag);
        if (value.Metadata is not null)
        {
            writer.WritePropertyName("metadata");
            JsonSerializer.Serialize(writer, value.Metadata, CivitaiJsonContext.Default.PaginationMetadata);
        }
        writer.WriteEndObject();
    }
}
