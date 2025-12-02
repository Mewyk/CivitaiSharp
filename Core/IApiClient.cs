namespace CivitaiSharp.Core;

using CivitaiSharp.Core.Request;

/// <summary>
/// Client interface for the Civitai public API. Properties return cached, immutable builder instances
/// that are safe for concurrent access. Each fluent method on the builders returns a new instance,
/// so multiple threads can safely derive new configurations from the same base builder.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Gets a cached, immutable, thread-safe request builder for models.
    /// </summary>
    ModelBuilder Models { get; }

    /// <summary>
    /// Gets a cached, immutable, thread-safe request builder for images.
    /// </summary>
    ImageBuilder Images { get; }

    /// <summary>
    /// Gets a cached, immutable, thread-safe request builder for tags.
    /// </summary>
    TagBuilder Tags { get; }

    /// <summary>
    /// Gets a cached, immutable, thread-safe request builder for creators.
    /// </summary>
    CreatorBuilder Creators { get; }
}
