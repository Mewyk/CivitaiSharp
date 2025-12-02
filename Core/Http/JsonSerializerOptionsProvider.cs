namespace CivitaiSharp.Core.Http;

using System.Text.Json;
using System.Text.Json.Serialization;
using CivitaiSharp.Core.Json;

/// <summary>
/// Provides configured <see cref="JsonSerializerOptions"/> for Civitai API responses, ensuring consistent
/// JSON handling across Core and SDK assemblies. Instances are cached and should not be modified.
/// </summary>
/// <remarks>
/// Uses source-generated <see cref="CivitaiJsonContext"/> for AOT-compatible serialization.
/// All options are configured at compile time via <see cref="JsonSourceGenerationOptionsAttribute"/>.
/// </remarks>
internal static class JsonSerializerOptionsProvider
{
    /// <summary>
    /// Gets the default <see cref="JsonSerializerOptions"/> for API responses.
    /// This instance is cached and should not be modified.
    /// </summary>
    /// <remarks>
    /// Uses the source-generated <see cref="CivitaiJsonContext"/> which provides AOT-compatible
    /// serialization with all converters pre-configured at compile time.
    /// </remarks>
    public static JsonSerializerOptions Default => CivitaiJsonContext.Default.Options;
}
