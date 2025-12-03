namespace CivitaiSharp.Core.Extensions;

using System;
using System.Net.Http.Headers;
using CivitaiSharp.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register Civitai API services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The default configuration section name for API options.
    /// </summary>
    public const string DefaultConfigurationSectionName = "CivitaiApi";

    private const string UserAgentValue = $"CivitaiSharp.Core/{Core.VersionInfo.Version}";

    /// <summary>
    /// Registers the API client and related services using default configuration.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if services is null.</exception>
    public static IServiceCollection AddCivitaiApi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure<ApiClientOptions>(_ => { });
        RegisterApiServices(services);
        return services;
    }

    /// <summary>
    /// Registers the API client and related services with the specified configuration action.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <param name="configure">Action to configure <see cref="ApiClientOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if services or configure is null.</exception>
    public static IServiceCollection AddCivitaiApi(
        this IServiceCollection services,
        Action<ApiClientOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.Configure(configure);
        RegisterApiServices(services);
        return services;
    }

    /// <summary>
    /// Registers the API client and related services using configuration from the provided <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
    /// Reads configuration from the "CivitaiApi" section by default.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <param name="configuration">The configuration source containing API settings.</param>
    /// <param name="sectionName">Optional section name to read from configuration. Defaults to "CivitaiApi".</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if services or configuration is null.</exception>
    /// <exception cref="ArgumentException">Thrown if sectionName is null or whitespace.</exception>
    public static IServiceCollection AddCivitaiApi(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration,
        string sectionName = DefaultConfigurationSectionName)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        services.Configure<ApiClientOptions>(configuration.GetSection(sectionName));
        RegisterApiServices(services);
        return services;
    }

    /// <summary>
    /// Registers core API services and configures HTTP client resilience policies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Registering <see cref="ApiHttpClient"/> and <see cref="ApiClient"/> as singletons is correct 
    /// for this library. The HTTP client factory pattern (<see cref="IHttpClientFactory"/>) is used,
    /// which properly manages <see cref="HttpMessageHandler"/> lifetimes and avoids socket exhaustion.
    /// </para>
    /// <para>
    /// The singleton services hold a reference to an <see cref="HttpClient"/> instance created from 
    /// the factory, which internally recycles handlers appropriately.
    /// </para>
    /// </remarks>
    private static void RegisterApiServices(IServiceCollection services)
    {
        services.AddSingleton(_ => new ApiResponseHandler());

        services.AddHttpClient(nameof(ApiHttpClient), (serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ApiClientOptions>>().Value;
            ConfigureHttpClient(client, options);
        })
        .AddStandardResilienceHandler();

        services.AddSingleton(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(ApiHttpClient));
            var responseHandler = serviceProvider.GetRequiredService<ApiResponseHandler>();
            var options = serviceProvider.GetRequiredService<IOptions<ApiClientOptions>>().Value;
            var logger = serviceProvider.GetService<ILogger<ApiHttpClient>>();
            return new ApiHttpClient(httpClient, responseHandler, options, logger);
        });

        services.AddSingleton<IApiClient>(serviceProvider =>
        {
            var httpClient = serviceProvider.GetRequiredService<ApiHttpClient>();
            return new ApiClient(httpClient);
        });
    }

    /// <summary>
    /// Configures the HTTP client base address, headers, and authentication.
    /// </summary>
    /// <param name="client">The HTTP client to configure.</param>
    /// <param name="options">The API client options containing configuration values.</param>
    private static void ConfigureHttpClient(HttpClient client, ApiClientOptions options)
    {
        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = options.Timeout;
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgentValue);

        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
        }
    }
}
