namespace CivitaiSharp.Core.Models;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a model version. When embedded in a Model response, some fields may be null.
/// When fetched directly via /model-versions/:id, additional fields like Model and ModelId are populated.
/// </summary>
/// <param name="Id">The unique identifier for the model version. Maps to JSON property "id".</param>
/// <param name="Index">The index/order of this version among all versions of the parent model. Maps to JSON property "index".</param>
/// <param name="ModelId">The unique identifier of the parent model. Maps to JSON property "modelId".</param>
/// <param name="Name">The name of the model version. Maps to JSON property "name".</param>
/// <param name="BaseModel">The base model this version was trained on (e.g., "SDXL 1.0", "SD 1.5", "Pony"). Maps to JSON property "baseModel".</param>
/// <param name="BaseModelType">The base model type classification. Maps to JSON property "baseModelType".</param>
/// <param name="Description">The description of the version, usually a changelog of updates. Maps to JSON property "description".</param>
/// <param name="CreatedAt">The date and time when this version was created. Maps to JSON property "createdAt".</param>
/// <param name="UpdatedAt">The date and time when this version was last updated. Maps to JSON property "updatedAt".</param>
/// <param name="PublishedAt">The date and time when this version was published. Maps to JSON property "publishedAt".</param>
/// <param name="Status">The publication status of the version (e.g., "Published", "Draft"). Maps to JSON property "status".</param>
/// <param name="Availability">The availability status of the version. Maps to JSON property "availability".</param>
/// <param name="NsfwLevel">The NSFW content level of the version on a numeric scale. Maps to JSON property "nsfwLevel".</param>
/// <param name="DownloadUrl">Direct download URL for this specific version. Maps to JSON property "downloadUrl".</param>
/// <param name="SupportsGeneration">Indicates whether this version supports generation capabilities. Maps to JSON property "supportsGeneration".</param>
/// <param name="TrainedWords">Array of trigger words or phrases used to activate this model in generation. Maps to JSON property "trainedWords".</param>
/// <param name="TrainingStatus">The status of any ongoing training job for this version. Maps to JSON property "trainingStatus".</param>
/// <param name="TrainingDetails">
/// Detailed metadata about the training process if applicable. Maps to JSON property "trainingDetails".
/// <para>
/// This property uses <see cref="JsonElement"/> because the Civitai API returns highly variable structures
/// for training details depending on the training workflow. The JSON structure is not documented by the API,
/// nor is the data consistent. Use <see cref="JsonElement.TryGetProperty(string, out JsonElement)"/> to safely access nested values.
/// </para>
/// </param>
/// <param name="EarlyAccessEndsAt">The date and time when early access restrictions end for this version. Maps to JSON property "earlyAccessEndsAt".</param>
/// <param name="EarlyAccessConfig">
/// Configuration settings for early access restrictions. Maps to JSON property "earlyAccessConfig".
/// <para>
/// This property uses <see cref="JsonElement"/> because the Civitai API returns highly variable structures
/// for early access configuration. The JSON structure is not documented by the API, nor is the data consistent.
/// Use <see cref="JsonElement.TryGetProperty(string, out JsonElement)"/> to safely access nested values.
/// </para>
/// </param>
/// <param name="UploadType">The method by which this version was uploaded (e.g., "Created", "Imported"). Maps to JSON property "uploadType".</param>
/// <param name="UsageControl">The usage control type for this version (e.g., "Download"). Maps to JSON property "usageControl".</param>
/// <param name="AirIdentifier">The AIR artifact identifier for this version. Maps to JSON property "air".</param>
/// <param name="Model">Abbreviated information about the parent model. Maps to JSON property "model".</param>
/// <param name="Files">List of files included in this model version. Maps to JSON property "files".</param>
/// <param name="Images">Gallery images showcasing example outputs from this model version. Maps to JSON property "images".</param>
/// <param name="Stats">Statistical information about user interactions with this model version. Maps to JSON property "stats".</param>
public sealed record ModelVersion(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("index")] int? Index,
    [property: JsonPropertyName("modelId")] long? ModelId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("baseModel")] string BaseModel,
    [property: JsonPropertyName("baseModelType")] string? BaseModelType,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")] DateTime? UpdatedAt,
    [property: JsonPropertyName("publishedAt")] DateTime? PublishedAt,
    [property: JsonPropertyName("status")] string? Status,
    [property: JsonPropertyName("availability")] Availability? Availability,
    [property: JsonPropertyName("nsfwLevel")] int NsfwLevel,
    [property: JsonPropertyName("downloadUrl")] string? DownloadUrl,
    [property: JsonPropertyName("supportsGeneration")] bool SupportsGeneration,
    [property: JsonPropertyName("trainedWords")] IReadOnlyList<string>? TrainedWords,
    [property: JsonPropertyName("trainingStatus")] string? TrainingStatus,
    [property: JsonPropertyName("trainingDetails")] JsonElement? TrainingDetails,
    [property: JsonPropertyName("earlyAccessEndsAt")] DateTime? EarlyAccessEndsAt,
    [property: JsonPropertyName("earlyAccessConfig")] JsonElement? EarlyAccessConfig,
    [property: JsonPropertyName("uploadType")] string? UploadType,
    [property: JsonPropertyName("usageControl")] string? UsageControl,
    [property: JsonPropertyName("air")] string? AirIdentifier,
    [property: JsonPropertyName("model")] ModelVersionModel? Model,
    [property: JsonPropertyName("files")] IReadOnlyList<ModelFile>? Files,
    [property: JsonPropertyName("images")] IReadOnlyList<ModelVersionImage>? Images,
    [property: JsonPropertyName("stats")] ModelVersionStats? Stats);
