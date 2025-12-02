namespace CivitaiSharp.Sdk.Extensions;

using System.Net.Http.Headers;
using CivitaiSharp.Sdk.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register Civitai SDK services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The default configuration section name for SDK options.
    /// </summary>
    public const string DefaultConfigurationSectionName = "CivitaiSdk";

    private const string UserAgentValue = $"CivitaiSharp.Sdk/{Sdk.VersionInfo.Version}";

    /// <summary>
    /// Registers the Civitai SDK client and related services with the specified configuration action.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <param name="configure">Action to configure <see cref="CivitaiSdkClientOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configure"/> is null.</exception>
    /// <example>
    /// <code>
    /// services.AddCivitaiSdk(options =>
    /// {
    ///     options.ApiToken = "your-api-token";
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddCivitaiSdk(
        this IServiceCollection services,
        Action<CivitaiSdkClientOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.Configure(configure);
        RegisterSdkServices(services);
        return services;
    }

    /// <summary>
    /// Registers the Civitai SDK client and related services using configuration from the provided <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <param name="configuration">The configuration source containing SDK settings.</param>
    /// <param name="sectionName">Optional section name to read from configuration. Defaults to "CivitaiSdk".</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="sectionName"/> is null or whitespace.</exception>
    /// <example>
    /// <code>
    /// // appsettings.json:
    /// // {
    /// //   "CivitaiSdk": {
    /// //     "ApiToken": "your-api-token"
    /// //   }
    /// // }
    /// 
    /// services.AddCivitaiSdk(configuration);
    /// </code>
    /// </example>
    public static IServiceCollection AddCivitaiSdk(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DefaultConfigurationSectionName)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        services.Configure<CivitaiSdkClientOptions>(configuration.GetSection(sectionName));
        RegisterSdkServices(services);
        return services;
    }

    /// <summary>
    /// Registers SDK services and configures HTTP client with resilience policies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Registering <see cref="SdkHttpClient"/> and <see cref="ICivitaiSdkClient"/> as singletons is correct
    /// for this library. The HTTP client factory pattern (<see cref="IHttpClientFactory"/>) is used,
    /// which properly manages <see cref="HttpMessageHandler"/> lifetimes and avoids socket exhaustion.
    /// </para>
    /// <para>
    /// The singleton services hold a reference to an <see cref="HttpClient"/> instance created from
    /// the factory, which internally recycles handlers appropriately.
    /// </para>
    /// </remarks>
    private static void RegisterSdkServices(IServiceCollection services)
    {
        // Ensure SDK enum mappings are registered before any SDK type is used
        SdkApiStringRegistry.EnsureInitialized();

        services.AddHttpClient(nameof(SdkHttpClient), (serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<CivitaiSdkClientOptions>>().Value;
            ConfigureHttpClient(client, options);
        })
        .AddStandardResilienceHandler();

        services.AddSingleton(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(SdkHttpClient));
            var logger = serviceProvider.GetService<ILogger<SdkHttpClient>>();
            return new SdkHttpClient(httpClient, logger);
        });

        services.AddSingleton<ICivitaiSdkClient>(serviceProvider =>
        {
            var httpClient = serviceProvider.GetRequiredService<SdkHttpClient>();
            var options = serviceProvider.GetRequiredService<IOptions<CivitaiSdkClientOptions>>().Value;
            return new CivitaiSdkClient(httpClient, options);
        });
    }

    /// <summary>
    /// Configures the HTTP client base address, headers, and authentication.
    /// </summary>
    private static void ConfigureHttpClient(HttpClient client, CivitaiSdkClientOptions options)
    {
        options.Validate();

        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = options.Timeout;
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgentValue);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiToken);
    }
}
