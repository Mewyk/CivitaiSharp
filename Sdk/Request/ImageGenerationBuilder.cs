namespace CivitaiSharp.Sdk.Request;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Http;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Models.Results;

/// <summary>
/// Immutable, thread-safe builder for constructing and submitting image generation jobs.
/// Supports both text-to-image and image-to-image generation modes.
/// Each fluent method returns a new builder instance, allowing safe reuse and caching of base configurations.
/// </summary>
public sealed record ImageGenerationBuilder
{
    private readonly SdkHttpClient _httpClient;
    private readonly SdkClientOptions _options;
    private readonly AirIdentifier? _air;
    private readonly ImageJobParamsBuilder? _paramsBuilder;
    private readonly ImmutableDictionary<AirIdentifier, ImageJobNetworkParams>? _additionalNetworks;
    private readonly ImmutableList<ImageJobControlNet>? _controlNets;
    private readonly int? _quantity;
    private readonly Priority? _priority;
    private readonly ImmutableDictionary<string, JsonElement>? _properties;
    private readonly string? _callbackUrl;
    private readonly int? _retries;
    private readonly string? _timeout;
    private readonly int? _clipSkip;
    private readonly string? _sourceImageUrl;
    private readonly decimal? _denoisingStrength;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageGenerationBuilder"/> class.
    /// This constructor is internal to enforce creation through JobsBuilder.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to execute requests.</param>
    /// <param name="options">The SDK client options.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient or options is null.</exception>
    internal ImageGenerationBuilder(
        SdkHttpClient httpClient,
        SdkClientOptions options)
        : this(
            httpClient ?? throw new ArgumentNullException(nameof(httpClient)),
            options ?? throw new ArgumentNullException(nameof(options)),
            air: null,
            paramsBuilder: null,
            additionalNetworks: null,
            controlNets: null,
            quantity: null,
            priority: null,
            properties: null,
            callbackUrl: null,
            retries: null,
            timeout: null,
            clipSkip: null,
            sourceImageUrl: null,
            denoisingStrength: null)
    {
    }

    private ImageGenerationBuilder(
        SdkHttpClient httpClient,
        SdkClientOptions options,
        AirIdentifier? air,
        ImageJobParamsBuilder? paramsBuilder,
        ImmutableDictionary<AirIdentifier, ImageJobNetworkParams>? additionalNetworks,
        ImmutableList<ImageJobControlNet>? controlNets,
        int? quantity,
        Priority? priority,
        ImmutableDictionary<string, JsonElement>? properties,
        string? callbackUrl,
        int? retries,
        string? timeout,
        int? clipSkip,
        string? sourceImageUrl,
        decimal? denoisingStrength)
    {
        _httpClient = httpClient;
        _options = options;
        _air = air;
        _paramsBuilder = paramsBuilder;
        _additionalNetworks = additionalNetworks;
        _controlNets = controlNets;
        _quantity = quantity;
        _priority = priority;
        _properties = properties;
        _callbackUrl = callbackUrl;
        _retries = retries;
        _timeout = timeout;
        _clipSkip = clipSkip;
        _sourceImageUrl = sourceImageUrl;
        _denoisingStrength = denoisingStrength;
    }

    /// <summary>
    /// Sets the base AIR identifier to use for generation.
    /// </summary>
    /// <param name="air">The AIR identifier. Required.</param>
    /// <returns>A new builder instance with the updated AIR identifier.</returns>
    /// <example>urn:air:sdxl:checkpoint:civitai:4201@130072</example>
    public ImageGenerationBuilder WithAir(AirIdentifier air)
        => new(_httpClient, _options, air, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);

    /// <summary>
    /// Sets the positive prompt for image generation.
    /// </summary>
    /// <param name="positivePrompt">The prompt text describing what to generate. Required.</param>
    /// <returns>A new builder instance with the updated positive prompt.</returns>
    public ImageGenerationBuilder WithPositivePrompt(string positivePrompt)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _air, builder.WithPositivePrompt(positivePrompt), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Sets the negative prompt describing what to avoid.
    /// </summary>
    /// <param name="negativePrompt">The negative prompt text.</param>
    /// <returns>A new builder instance with the updated negative prompt.</returns>
    public ImageGenerationBuilder WithNegativePrompt(string negativePrompt)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _air, builder.WithNegativePrompt(negativePrompt), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Sets the image dimensions.
    /// </summary>
    /// <param name="width">The width in pixels. Must be a multiple of 8. Range: 64-2048.</param>
    /// <param name="height">The height in pixels. Must be a multiple of 8. Range: 64-2048.</param>
    /// <returns>A new builder instance with the updated dimensions.</returns>
    public ImageGenerationBuilder WithDimensions(int width, int height)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _air, builder.WithDimensions(width, height), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Sets the number of sampling steps.
    /// </summary>
    /// <param name="steps">The step count. Range: 1-100, default: 20.</param>
    /// <returns>A new builder instance with the updated steps.</returns>
    public ImageGenerationBuilder WithSteps(int steps)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _air, builder.WithSteps(steps), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Sets the classifier-free guidance scale.
    /// </summary>
    /// <param name="configurationScale">The configuration scale. Range: 1-30, default: 7.0.</param>
    /// <returns>A new builder instance with the updated configuration scale.</returns>
    public ImageGenerationBuilder WithConfigurationScale(decimal configurationScale)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _air, builder.WithConfigurationScale(configurationScale), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Sets the random seed for reproducible generation.
    /// </summary>
    /// <param name="seed">The seed value.</param>
    /// <returns>A new builder instance with the updated seed.</returns>
    public ImageGenerationBuilder WithSeed(long seed)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _air, builder.WithSeed(seed), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Sets the sampling algorithm/scheduler to use for generation.
    /// </summary>
    /// <param name="scheduler">The scheduler/sampler algorithm.</param>
    /// <returns>A new builder instance with the updated scheduler.</returns>
    /// <remarks>
    /// Common schedulers include Euler, EulerAncestral, DpmPlusPlus2M, and DpmPlusPlus2MKarras.
    /// The choice of scheduler affects both generation quality and speed.
    /// </remarks>
    public ImageGenerationBuilder WithScheduler(Scheduler scheduler)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _air, builder.WithScheduler(scheduler), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Configures generation parameters using a custom builder.
    /// </summary>
    /// <param name="paramsBuilder">The configured parameters builder.</param>
    /// <returns>A new builder instance with the updated parameters.</returns>
    public ImageGenerationBuilder WithParams(ImageJobParamsBuilder paramsBuilder)
        => new(_httpClient, _options, _air, paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);

    /// <summary>
    /// Configures generation parameters using a configuration action.
    /// </summary>
    /// <param name="configure">Action to configure the parameters builder.</param>
    /// <returns>A new builder instance with the updated parameters.</returns>
    public ImageGenerationBuilder WithParams(Func<ImageJobParamsBuilder, ImageJobParamsBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _air, configure(builder), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Sets the source image URL for image-to-image generation.
    /// </summary>
    /// <param name="sourceImageUrl">The URL of the source image. When provided, enables image-to-image mode.</param>
    /// <returns>A new builder instance with the updated source image URL.</returns>
    /// <remarks>
    /// When a source image is provided, the generation becomes image-to-image mode.
    /// Use <see cref="WithDenoisingStrength"/> to control how much the source image is transformed.
    /// </remarks>
    public ImageGenerationBuilder WithSourceImageUrl(string sourceImageUrl)
        => new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, sourceImageUrl, _denoisingStrength);

    /// <summary>
    /// Sets the denoising strength for image-to-image generation.
    /// </summary>
    /// <param name="denoisingStrength">The strength value. Range: 0.0-1.0. Lower values preserve more of the source image.</param>
    /// <returns>A new builder instance with the updated denoising strength.</returns>
    /// <remarks>
    /// Only applicable when a source image is provided via <see cref="WithSourceImageUrl"/>.
    /// Lower values (e.g., 0.3) preserve more of the source image, higher values (e.g., 0.8) allow more transformation.
    /// </remarks>
    public ImageGenerationBuilder WithDenoisingStrength(decimal denoisingStrength)
        => new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, denoisingStrength);

    /// <summary>
    /// Adds an additional network (LoRA, embedding, etc.) to the generation.
    /// </summary>
    /// <param name="network">The AIR identifier for the network.</param>
    /// <param name="networkParams">The network configuration.</param>
    /// <returns>A new builder instance with the added network.</returns>
    public ImageGenerationBuilder WithAdditionalNetwork(AirIdentifier network, ImageJobNetworkParams networkParams)
    {
        ArgumentNullException.ThrowIfNull(networkParams);
        var networks = _additionalNetworks ?? [];
        return new(_httpClient, _options, _air, _paramsBuilder, networks.SetItem(network, networkParams), _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Adds an additional network (LoRA, embedding, etc.) using a builder.
    /// </summary>
    /// <param name="network">The AIR identifier for the network.</param>
    /// <param name="networkBuilder">The configured network parameters builder.</param>
    /// <returns>A new builder instance with the added network.</returns>
    public ImageGenerationBuilder WithAdditionalNetwork(AirIdentifier network, NetworkBuilder networkBuilder)
    {
        ArgumentNullException.ThrowIfNull(networkBuilder);
        return WithAdditionalNetwork(network, networkBuilder.Build());
    }

    /// <summary>
    /// Adds an additional network (LoRA, embedding, etc.) using a configuration action.
    /// </summary>
    /// <param name="network">The AIR identifier for the network.</param>
    /// <param name="configure">Action to configure the network parameters builder.</param>
    /// <returns>A new builder instance with the added network.</returns>
    public ImageGenerationBuilder WithAdditionalNetwork(AirIdentifier network, Func<NetworkBuilder, NetworkBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(NetworkBuilder.Create());
        return WithAdditionalNetwork(network, builder.Build());
    }

    /// <summary>
    /// Adds a ControlNet configuration for guided generation.
    /// </summary>
    /// <param name="controlNet">The ControlNet configuration.</param>
    /// <returns>A new builder instance with the added ControlNet.</returns>
    public ImageGenerationBuilder WithControlNet(ImageJobControlNet controlNet)
    {
        ArgumentNullException.ThrowIfNull(controlNet);
        var controlNets = _controlNets ?? [];
        return new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, controlNets.Add(controlNet), _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Adds a ControlNet configuration using a builder.
    /// </summary>
    /// <param name="controlNetBuilder">The configured ControlNet builder.</param>
    /// <returns>A new builder instance with the added ControlNet.</returns>
    public ImageGenerationBuilder WithControlNet(ControlNetBuilder controlNetBuilder)
    {
        ArgumentNullException.ThrowIfNull(controlNetBuilder);
        return WithControlNet(controlNetBuilder.Build());
    }

    /// <summary>
    /// Adds a ControlNet configuration using a configuration action.
    /// </summary>
    /// <param name="configure">Action to configure the ControlNet builder.</param>
    /// <returns>A new builder instance with the added ControlNet.</returns>
    public ImageGenerationBuilder WithControlNet(Func<ControlNetBuilder, ControlNetBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(ControlNetBuilder.Create());
        return WithControlNet(builder.Build());
    }

    /// <summary>
    /// Sets the number of images to generate.
    /// </summary>
    /// <param name="quantity">The quantity. Range: 1-10, default: 1.</param>
    /// <returns>A new builder instance with the updated quantity.</returns>
    public ImageGenerationBuilder WithQuantity(int quantity)
        => new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, _controlNets, quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);

    /// <summary>
    /// Sets the priority configuration for job scheduling.
    /// </summary>
    /// <param name="priority">The priority configuration.</param>
    /// <returns>A new builder instance with the updated priority.</returns>
    public ImageGenerationBuilder WithPriority(Priority priority)
        => new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);

    /// <summary>
    /// Adds a custom property for job tracking and querying.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The property value (must be JSON-serializable).</param>
    /// <returns>A new builder instance with the added property.</returns>
    public ImageGenerationBuilder WithProperty(string key, JsonElement value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var properties = _properties ?? [];
        return new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, properties.SetItem(key, value), _callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);
    }

    /// <summary>
    /// Sets the webhook URL to call when the job completes.
    /// </summary>
    /// <param name="callbackUrl">The webhook URL.</param>
    /// <returns>A new builder instance with the updated callback URL.</returns>
    public ImageGenerationBuilder WithCallbackUrl(string callbackUrl)
        => new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, callbackUrl, _retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);

    /// <summary>
    /// Sets the number of automatic retries on failure.
    /// </summary>
    /// <param name="retries">The number of retry attempts. Default: 0.</param>
    /// <returns>A new builder instance with the updated retry attempts.</returns>
    public ImageGenerationBuilder WithRetries(int retries)
        => new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, retries, _timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);

    /// <summary>
    /// Sets the job timeout.
    /// </summary>
    /// <param name="timeout">The timeout duration. Format: "HH:mm:ss". Default: "00:10:00".</param>
    /// <returns>A new builder instance with the updated timeout.</returns>
    public ImageGenerationBuilder WithTimeout(string timeout)
        => new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, timeout, _clipSkip, _sourceImageUrl, _denoisingStrength);

    /// <summary>
    /// Sets the job timeout.
    /// </summary>
    /// <param name="timeout">The timeout duration. Default: 10 minutes.</param>
    /// <returns>A new builder instance with the updated timeout.</returns>
    public ImageGenerationBuilder WithTimeout(TimeSpan timeout)
        => new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, timeout.ToString(@"hh\:mm\:ss"), _clipSkip, _sourceImageUrl, _denoisingStrength);

    /// <summary>
    /// Sets the number of CLIP layers to skip.
    /// </summary>
    /// <param name="clipSkip">The number of layers to skip. Range: 1-12.</param>
    /// <returns>A new builder instance with the updated CLIP skip.</returns>
    /// <remarks>
    /// A value of 2 is commonly used for anime/Pony models.
    /// This can also be set via <see cref="ImageJobParamsBuilder.WithClipSkip"/>.
    /// </remarks>
    public ImageGenerationBuilder WithClipSkip(int clipSkip)
        => new(_httpClient, _options, _air, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, clipSkip, _sourceImageUrl, _denoisingStrength);

    /// <summary>
    /// Executes the job submission to the Civitai Generator API.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status collection with a token for polling.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required properties are missing.</exception>
    public Task<Result<JobStatusCollection>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_air == null)
        {
            throw new InvalidOperationException("AIR identifier is required. Use WithAir() to set it.");
        }

        if (_paramsBuilder == null)
        {
            throw new InvalidOperationException("Parameters are required. Use WithPositivePrompt() or WithParams() to set them.");
        }

        var request = new ImageGenerationJobRequest
        {
            Air = _air.Value,
            Params = _paramsBuilder.Build(),
            AdditionalNetworks = _additionalNetworks?.Count > 0 ? _additionalNetworks : null,
            ControlNets = _controlNets?.Count > 0 ? _controlNets : null,
            Quantity = _quantity,
            Priority = _priority,
            Properties = _properties?.Count > 0 ? _properties : null,
            CallbackUrl = _callbackUrl,
            Retries = _retries,
            Timeout = _timeout,
            ClipSkip = _clipSkip,
            SourceImageUrl = _sourceImageUrl,
            DenoisingStrength = _denoisingStrength
        };

        // Validate ControlNet configurations if present
        if (request.ControlNets is not null)
        {
            foreach (var controlNet in request.ControlNets)
            {
                controlNet.Validate();
            }
        }

        var uri = _options.GetApiPath("jobs");
        return _httpClient.PostAsync<ImageGenerationJobRequest, JobStatusCollection>(uri, request, cancellationToken);
    }

    /// <summary>
    /// Executes multiple job submissions as a batch to the Civitai Generator API.
    /// </summary>
    /// <param name="additionalJobs">Additional configured job builders to submit in the same batch.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status collection with a token for polling all jobs.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required properties are missing.</exception>
    public Task<Result<JobStatusCollection>> ExecuteBatchAsync(
        IEnumerable<ImageGenerationBuilder> additionalJobs,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(additionalJobs);

        // Build all requests including this one
        var allBuilders = new[] { this }.Concat(additionalJobs).ToList();
        var requests = new List<ImageGenerationJobRequest>();

        foreach (var builder in allBuilders)
        {
            if (builder._air == null)
            {
                throw new InvalidOperationException("All jobs must have an AIR identifier set. Use WithAir() on all builders.");
            }

            if (builder._paramsBuilder == null)
            {
                throw new InvalidOperationException("All jobs must have parameters set. Use WithPositivePrompt() or WithParams() on all builders.");
            }

            var request = new ImageGenerationJobRequest
            {
                Air = builder._air.Value,
                Params = builder._paramsBuilder.Build(),
                AdditionalNetworks = builder._additionalNetworks?.Count > 0 ? builder._additionalNetworks : null,
                ControlNets = builder._controlNets?.Count > 0 ? builder._controlNets : null,
                Quantity = builder._quantity,
                Priority = builder._priority,
                Properties = builder._properties?.Count > 0 ? builder._properties : null,
                CallbackUrl = builder._callbackUrl,
                Retries = builder._retries,
                Timeout = builder._timeout,
                ClipSkip = builder._clipSkip,
                SourceImageUrl = builder._sourceImageUrl,
                DenoisingStrength = builder._denoisingStrength
            };

            // Validate ControlNet configurations
            if (request.ControlNets is not null)
            {
                foreach (var controlNet in request.ControlNets)
                {
                    controlNet.Validate();
                }
            }

            requests.Add(request);
        }

        var batchRequest = new BatchJobRequest { Jobs = requests };
        var uri = _options.GetApiPath("jobs");
        return _httpClient.PostAsync<BatchJobRequest, JobStatusCollection>(uri, batchRequest, cancellationToken);
    }
}
