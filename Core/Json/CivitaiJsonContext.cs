namespace CivitaiSharp.Core.Json;

using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Json.Converters;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Models.Common;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Core.Services;

/// <summary>
/// Source-generated JSON serialization context for CivitaiSharp.Core.
/// Provides AOT-compatible serialization without runtime reflection.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    Converters = [
        typeof(AvailabilityConverter),
        typeof(CommercialUsePermissionConverter),
        typeof(CommercialUsePermissionListConverter),
        typeof(ImageNsfwLevelConverter),
        typeof(ImageSortConverter),
        typeof(MediaTypeConverter),
        typeof(ModelModeConverter),
        typeof(ModelSortConverter),
        typeof(ModelTypeConverter),
        typeof(TimePeriodConverter),
        // Paged response converters (AOT-compatible)
        typeof(PagedModelResponseConverter),
        typeof(PagedImageResponseConverter),
        typeof(PagedCreatorResponseConverter),
        typeof(PagedTagResponseConverter)
    ])]
// Core model types
[JsonSerializable(typeof(Model))]
[JsonSerializable(typeof(ModelVersion))]
[JsonSerializable(typeof(ModelVersionImage))]
[JsonSerializable(typeof(ModelVersionImageFile))]
[JsonSerializable(typeof(ModelVersionModel))]
[JsonSerializable(typeof(ModelVersionStats))]
[JsonSerializable(typeof(ModelFile))]
[JsonSerializable(typeof(ModelStats))]
[JsonSerializable(typeof(Image))]
[JsonSerializable(typeof(ImageMeta))]
[JsonSerializable(typeof(ImageMetaExtra))]
[JsonSerializable(typeof(ImageMetaResource))]
[JsonSerializable(typeof(ImageStats))]
[JsonSerializable(typeof(Creator))]
[JsonSerializable(typeof(Tag))]
[JsonSerializable(typeof(Hashes))]
[JsonSerializable(typeof(CivitaiResource))]
// Common types
[JsonSerializable(typeof(FileMetadata))]
[JsonSerializable(typeof(PaginationMetadata))]
// Internal response types
[JsonSerializable(typeof(ApiErrorResponse))]
// Paged response types for each entity
[JsonSerializable(typeof(PagedApiResponse<Model>))]
[JsonSerializable(typeof(PagedApiResponse<Image>))]
[JsonSerializable(typeof(PagedApiResponse<Creator>))]
[JsonSerializable(typeof(PagedApiResponse<Tag>))]
// Collection types needed for deserialization and serialization
[JsonSerializable(typeof(System.Collections.Generic.List<Model>))]
[JsonSerializable(typeof(System.Collections.Generic.List<Image>))]
[JsonSerializable(typeof(System.Collections.Generic.List<Creator>))]
[JsonSerializable(typeof(System.Collections.Generic.List<Tag>))]
[JsonSerializable(typeof(System.Collections.Generic.IReadOnlyList<Model>))]
[JsonSerializable(typeof(System.Collections.Generic.IReadOnlyList<Image>))]
[JsonSerializable(typeof(System.Collections.Generic.IReadOnlyList<Creator>))]
[JsonSerializable(typeof(System.Collections.Generic.IReadOnlyList<Tag>))]
// Request body types for API calls (Dictionary values can be string, bool, int, or string[])
[JsonSerializable(typeof(System.Collections.Generic.Dictionary<string, object?>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(string[]))]
internal partial class CivitaiJsonContext : JsonSerializerContext;
