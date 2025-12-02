namespace CivitaiSharp.Sdk.Http;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Json;
using CivitaiSharp.Sdk.Models.Coverage;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Models.Results;
using CivitaiSharp.Sdk.Models.Usage;

/// <summary>
/// Provides AOT-compatible JSON type info resolution for SDK types.
/// Maps runtime types to their source-generated JsonTypeInfo.
/// </summary>
internal static class SdkJsonTypeResolver
{
    /// <summary>
    /// Gets the JsonTypeInfo for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to get JsonTypeInfo for.</typeparam>
    /// <returns>The JsonTypeInfo for serialization/deserialization.</returns>
    /// <exception cref="NotSupportedException">Thrown when the type is not supported.</exception>
    public static JsonTypeInfo<T> GetTypeInfo<T>()
    {
        var typeInfo = TryGetTypeInfo<T>();
        return typeInfo ?? throw new NotSupportedException(
            $"Type '{typeof(T).FullName}' is not supported for AOT-compatible JSON serialization. " +
            "Add it to SdkJsonContext.");
    }

    /// <summary>
    /// Tries to get the JsonTypeInfo for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to get JsonTypeInfo for.</typeparam>
    /// <returns>The JsonTypeInfo if found; otherwise, null.</returns>
    public static JsonTypeInfo<T>? TryGetTypeInfo<T>()
    {
        // Use pattern matching to map types to their JsonTypeInfo
        // This is fully AOT-compatible as all types are known at compile time
        object? typeInfo = typeof(T) switch
        {
            // Job request types
            _ when typeof(T) == typeof(TextToImageJobRequest) => SdkJsonContext.Default.TextToImageJobRequest,
            _ when typeof(T) == typeof(BatchJobRequest) => SdkJsonContext.Default.BatchJobRequest,
            _ when typeof(T) == typeof(QueryJobsRequest) => SdkJsonContext.Default.QueryJobsRequest,

            // Job response types
            _ when typeof(T) == typeof(JobStatus) => SdkJsonContext.Default.JobStatus,
            _ when typeof(T) == typeof(JobStatusCollection) => SdkJsonContext.Default.JobStatusCollection,
            _ when typeof(T) == typeof(JobResult) => SdkJsonContext.Default.JobResult,

            // Coverage types
            _ when typeof(T) == typeof(ProviderAssetAvailability) => SdkJsonContext.Default.ProviderAssetAvailability,
            _ when typeof(T) == typeof(IReadOnlyDictionary<string, ProviderAssetAvailability>) => SdkJsonContext.Default.IReadOnlyDictionaryStringProviderAssetAvailability,

            // Usage types
            _ when typeof(T) == typeof(ConsumptionDetails) => SdkJsonContext.Default.ConsumptionDetails,

            // Core types (Unit is handled specially)
            _ when typeof(T) == typeof(Unit) => null, // Unit is handled specially - no serialization needed

            _ => null
        };

        return typeInfo as JsonTypeInfo<T>;
    }

    /// <summary>
    /// Deserializes JSON from a stream using AOT-compatible type info.
    /// </summary>
    public static async ValueTask<T?> DeserializeAsync<T>(
        Stream stream,
        CancellationToken cancellationToken)
    {
        var typeInfo = GetTypeInfo<T>();
        return await JsonSerializer.DeserializeAsync(stream, typeInfo, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Serializes an object to JSON using AOT-compatible type info.
    /// </summary>
    public static string Serialize<T>(T value)
    {
        var typeInfo = GetTypeInfo<T>();
        return JsonSerializer.Serialize(value, typeInfo);
    }

    /// <summary>
    /// Creates JSON HTTP content from an object using AOT-compatible serialization.
    /// Uses <see cref="JsonContent"/> for efficient streaming serialization without intermediate string allocation.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to serialize as JSON content.</param>
    /// <returns>An <see cref="HttpContent"/> containing the serialized JSON.</returns>
    public static HttpContent CreateJsonContent<T>(T value)
    {
        var typeInfo = GetTypeInfo<T>();
        return JsonContent.Create(value, typeInfo);
    }
}
