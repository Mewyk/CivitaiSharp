namespace CivitaiSharp.Core;

using CivitaiSharp.Core.Request;
using CivitaiSharp.Core.Services;

/// <summary>
/// Primary client facade for the Civitai public API. All builder properties return cached, immutable,
/// thread-safe instances that can be safely shared across threads. Each fluent method on a builder
/// returns a new instance with the updated configuration, leaving the original builder unchanged.
/// Obtain an instance through dependency injection using
/// <see cref="Extensions.ServiceCollectionExtensions.AddCivitaiApi(Microsoft.Extensions.DependencyInjection.IServiceCollection, Action{ApiClientOptions})"/>.
/// </summary>
public sealed class ApiClient : IApiClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClient"/> class.
    /// Internal to enforce dependency injection usage.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to make API requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> is null.</exception>
    internal ApiClient(ApiHttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        Models = new ModelBuilder(httpClient);
        Images = new ImageBuilder(httpClient);
        Tags = new TagBuilder(httpClient);
        Creators = new CreatorBuilder(httpClient);
    }

    /// <inheritdoc />
    public ModelBuilder Models { get; }

    /// <inheritdoc />
    public ImageBuilder Images { get; }

    /// <inheritdoc />
    public TagBuilder Tags { get; }

    /// <inheritdoc />
    public CreatorBuilder Creators { get; }
}
