namespace CivitaiSharp.Tools.Extensions;

using System;
using System.Net.Http.Headers;
using CivitaiSharp.Tools.Downloads;
using CivitaiSharp.Tools.Downloads.Options;
using CivitaiSharp.Tools.Downloads.Validation;
using CivitaiSharp.Tools.Hashing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
/// Extension methods for registering Civitai download and hashing services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Civitai download and file hashing services with default configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method registers the following services:
    /// <list type="bullet">
    /// <item><description><see cref="IFileHashingService"/> - For computing file hashes (SHA256, SHA512, BLAKE3, CRC32)</description></item>
    /// <item><description><see cref="IDownloadService"/> - For downloading images and model files</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Default options use the system temporary directory for downloads.
    /// Use one of the configuration overloads to customize behavior.
    /// </para>
    /// </remarks>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddCivitaiDownloads(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddCivitaiDownloads(static _ => { });
    }

    /// <summary>
    /// Adds Civitai download and file hashing services with configuration from an <see cref="IConfiguration"/> section.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Binds options from the configuration section. Expected structure:
    /// <code>
    /// {
    ///   "Images": {
    ///     "BaseDirectory": "C:\\Downloads\\Images",
    ///     "PathPattern": "{BaseModel}/{Username}/{Id}.{Extension}",
    ///     "OverwriteExisting": false
    ///   },
    ///   "Models": {
    ///     "BaseDirectory": "C:\\Models",
    ///     "PathPattern": "{ModelType}/{FileName}",
    ///     "OverwriteExisting": true,
    ///     "VerifyHash": true,
    ///     "HashAlgorithm": "Sha256"
    ///   }
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration section containing download options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    public static IServiceCollection AddCivitaiDownloads(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Bind options from configuration
        services.AddOptions<ImageDownloadOptions>()
            .Bind(configuration.GetSection("Images"))
            .ValidateOnStart();

        services.AddOptions<ModelDownloadOptions>()
            .Bind(configuration.GetSection("Models"))
            .ValidateOnStart();

        return services.AddCivitaiDownloadsCore();
    }

    /// <summary>
    /// Adds Civitai download and file hashing services with programmatic configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">An action to configure the download options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configure"/> is null.</exception>
    public static IServiceCollection AddCivitaiDownloads(this IServiceCollection services, Action<DownloadOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        // Create options instance and configure
        var options = new DownloadOptions();
        configure(options);

        // Register the configured options
        services.AddOptions<ImageDownloadOptions>()
            .Configure(imageOptions =>
            {
                imageOptions.BaseDirectory = options.Images.BaseDirectory;
                imageOptions.PathPattern = options.Images.PathPattern;
                imageOptions.OverwriteExisting = options.Images.OverwriteExisting;
            })
            .ValidateOnStart();

        services.AddOptions<ModelDownloadOptions>()
            .Configure(modelOptions =>
            {
                modelOptions.BaseDirectory = options.Models.BaseDirectory;
                modelOptions.PathPattern = options.Models.PathPattern;
                modelOptions.OverwriteExisting = options.Models.OverwriteExisting;
                modelOptions.VerifyHash = options.Models.VerifyHash;
                modelOptions.HashAlgorithm = options.Models.HashAlgorithm;
            })
            .ValidateOnStart();

        return services.AddCivitaiDownloadsCore();
    }

    private static IServiceCollection AddCivitaiDownloadsCore(this IServiceCollection services)
    {
        // Register validators
        services.AddSingleton<IValidateOptions<ImageDownloadOptions>, ImageOptionsValidator>();
        services.AddSingleton<IValidateOptions<ModelDownloadOptions>, ModelOptionsValidator>();

        // Configure a named HTTP client for download operations
        services.AddHttpClient(HttpClientNames.CivitaiPublicApi, static client =>
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("CivitaiSharp/1.0");
        });

        // Register services
        services.AddSingleton<IFileHashingService, FileHashingService>();
        services.AddScoped<IDownloadService, DownloadService>();

        return services;
    }
}
