namespace CivitaiSharp.Sdk.Json;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Json.Converters;
using CivitaiSharp.Sdk.Models.Coverage;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Models.Results;
using CivitaiSharp.Sdk.Models.Usage;

/// <summary>
/// Source-generated JSON serialization context for CivitaiSharp.Sdk.
/// Provides AOT-compatible serialization without runtime reflection.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    Converters = [
        typeof(AirIdentifierConverter),
        typeof(SchedulerConverter),
        typeof(NullableSchedulerConverter),
        typeof(NetworkTypeConverter),
        typeof(ControlNetPreprocessorConverter),
        typeof(NullableControlNetPreprocessorConverter),
        typeof(AvailabilityStatusConverter),
        typeof(ProviderAssetAvailabilityDictionaryConverter)
    ])]
// Job request types
[JsonSerializable(typeof(TextToImageJobRequest))]
[JsonSerializable(typeof(BatchJobRequest))]
[JsonSerializable(typeof(QueryJobsRequest))]
[JsonSerializable(typeof(ImageJobParams))]
[JsonSerializable(typeof(ImageJobNetworkParams))]
[JsonSerializable(typeof(ImageJobControlNet))]
[JsonSerializable(typeof(Priority))]
// Job response types
[JsonSerializable(typeof(JobStatus))]
[JsonSerializable(typeof(JobStatusCollection))]
[JsonSerializable(typeof(JobResult))]
// Coverage types
[JsonSerializable(typeof(ProviderAssetAvailability))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, ProviderAssetAvailability>))]
[JsonSerializable(typeof(Dictionary<string, ProviderAssetAvailability>))]
// Usage types
[JsonSerializable(typeof(ConsumptionDetails))]
// AIR types
[JsonSerializable(typeof(AirIdentifier))]
// Collection types
[JsonSerializable(typeof(IReadOnlyList<JobStatus>))]
[JsonSerializable(typeof(List<JobStatus>))]
[JsonSerializable(typeof(IReadOnlyList<TextToImageJobRequest>))]
[JsonSerializable(typeof(List<TextToImageJobRequest>))]
[JsonSerializable(typeof(IReadOnlyList<ImageJobControlNet>))]
[JsonSerializable(typeof(List<ImageJobControlNet>))]
[JsonSerializable(typeof(IReadOnlyDictionary<AirIdentifier, ImageJobNetworkParams>))]
[JsonSerializable(typeof(Dictionary<AirIdentifier, ImageJobNetworkParams>))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, JsonElement>))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
// Primitive types for properties
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(JsonElement))]
internal partial class SdkJsonContext : JsonSerializerContext;
