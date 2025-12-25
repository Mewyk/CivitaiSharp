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
using CivitaiSharp.Sdk.Http;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Models.Results;

/// <summary>
/// Immutable, thread-safe builder for constructing and submitting text-to-image generation jobs.
/// Each fluent method returns a new builder instance, allowing safe reuse and caching of base configurations.
/// </summary>
public sealed record TextToImageBuilder
{
    private readonly SdkHttpClient _httpClient;
    private readonly SdkClientOptions _options;
    private readonly AirIdentifier? _model;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="TextToImageBuilder"/> class.
    /// This constructor is internal to enforce creation through JobsBuilder.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to execute requests.</param>
    /// <param name="options">The SDK client options.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient or options is null.</exception>
    internal TextToImageBuilder(
        SdkHttpClient httpClient,
        SdkClientOptions options)
        : this(
            httpClient ?? throw new ArgumentNullException(nameof(httpClient)),
            options ?? throw new ArgumentNullException(nameof(options)),
            model: null,
            paramsBuilder: null,
            additionalNetworks: null,
            controlNets: null,
            quantity: null,
            priority: null,
            properties: null,
            callbackUrl: null,
            retries: null,
            timeout: null,
            clipSkip: null)
    {
    }

    private TextToImageBuilder(
        SdkHttpClient httpClient,
        SdkClientOptions options,
        AirIdentifier? model,
        ImageJobParamsBuilder? paramsBuilder,
        ImmutableDictionary<AirIdentifier, ImageJobNetworkParams>? additionalNetworks,
        ImmutableList<ImageJobControlNet>? controlNets,
        int? quantity,
        Priority? priority,
        ImmutableDictionary<string, JsonElement>? properties,
        string? callbackUrl,
        int? retries,
        string? timeout,
        int? clipSkip)
    {
        _httpClient = httpClient;
        _options = options;
        _model = model;
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
    }

    /// <summary>
    /// Sets the base model to use for generation.
    /// </summary>
    /// <param name="model">The AIR identifier for the model. Required.</param>
    /// <returns>A new builder instance with the updated model.</returns>
    /// <example>urn:air:sdxl:checkpoint:civitai:4201@130072</example>
    public TextToImageBuilder WithModel(AirIdentifier model)
        => new(_httpClient, _options, model, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);

    /// <summary>
    /// Sets the positive prompt for image generation.
    /// </summary>
    /// <param name="prompt">The prompt text describing what to generate. Required.</param>
    /// <returns>A new builder instance with the updated prompt.</returns>
    public TextToImageBuilder WithPrompt(string prompt)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _model, builder.WithPrompt(prompt), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);
    }

    /// <summary>
    /// Sets the negative prompt describing what to avoid.
    /// </summary>
    /// <param name="negativePrompt">The negative prompt text.</param>
    /// <returns>A new builder instance with the updated negative prompt.</returns>
    public TextToImageBuilder WithNegativePrompt(string negativePrompt)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _model, builder.WithNegativePrompt(negativePrompt), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);
    }

    /// <summary>
    /// Sets the image dimensions.
    /// </summary>
    /// <param name="width">The width in pixels. Must be a multiple of 8. Range: 64-2048.</param>
    /// <param name="height">The height in pixels. Must be a multiple of 8. Range: 64-2048.</param>
    /// <returns>A new builder instance with the updated dimensions.</returns>
    public TextToImageBuilder WithSize(int width, int height)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _model, builder.WithSize(width, height), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);
    }

    /// <summary>
    /// Sets the number of sampling steps.
    /// </summary>
    /// <param name="steps">The step count. Range: 1-100, default: 20.</param>
    /// <returns>A new builder instance with the updated steps.</returns>
    public TextToImageBuilder WithSteps(int steps)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _model, builder.WithSteps(steps), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);
    }

    /// <summary>
    /// Sets the classifier-free guidance scale.
    /// </summary>
    /// <param name="cfgScale">The CFG scale. Range: 1-30, default: 7.0.</param>
    /// <returns>A new builder instance with the updated CFG scale.</returns>
    public TextToImageBuilder WithCfgScale(decimal cfgScale)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _model, builder.WithCfgScale(cfgScale), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);
    }

    /// <summary>
    /// Sets the random seed for reproducible generation.
    /// </summary>
    /// <param name="seed">The seed value.</param>
    /// <returns>A new builder instance with the updated seed.</returns>
    public TextToImageBuilder WithSeed(long seed)
    {
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _model, builder.WithSeed(seed), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);
    }

    /// <summary>
    /// Configures generation parameters using a custom builder.
    /// </summary>
    /// <param name="paramsBuilder">The configured parameters builder.</param>
    /// <returns>A new builder instance with the updated parameters.</returns>
    public TextToImageBuilder WithParams(ImageJobParamsBuilder paramsBuilder)
        => new(_httpClient, _options, _model, paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);

    /// <summary>
    /// Configures generation parameters using a configuration action.
    /// </summary>
    /// <param name="configure">Action to configure the parameters builder.</param>
    /// <returns>A new builder instance with the updated parameters.</returns>
    public TextToImageBuilder WithParams(Func<ImageJobParamsBuilder, ImageJobParamsBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = _paramsBuilder ?? ImageJobParamsBuilder.Create();
        return new(_httpClient, _options, _model, configure(builder), _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);
    }

    /// <summary>
    /// Adds an additional network (LoRA, embedding, etc.) to the generation.
    /// </summary>
    /// <param name="network">The AIR identifier for the network.</param>
    /// <param name="networkParams">The network configuration.</param>
    /// <returns>A new builder instance with the added network.</returns>
    public TextToImageBuilder WithAdditionalNetwork(AirIdentifier network, ImageJobNetworkParams networkParams)
    {
        ArgumentNullException.ThrowIfNull(networkParams);
        var networks = _additionalNetworks ?? ImmutableDictionary<AirIdentifier, ImageJobNetworkParams>.Empty;
        return new(_httpClient, _options, _model, _paramsBuilder, networks.SetItem(network, networkParams), _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);
    }

    /// <summary>
    /// Adds an additional network (LoRA, embedding, etc.) using a builder.
    /// </summary>
    /// <param name="network">The AIR identifier for the network.</param>
    /// <param name="networkBuilder">The configured network parameters builder.</param>
    /// <returns>A new builder instance with the added network.</returns>
    public TextToImageBuilder WithAdditionalNetwork(AirIdentifier network, ImageJobNetworkParamsBuilder networkBuilder)
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
    public TextToImageBuilder WithAdditionalNetwork(AirIdentifier network, Func<ImageJobNetworkParamsBuilder, ImageJobNetworkParamsBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(ImageJobNetworkParamsBuilder.Create());
        return WithAdditionalNetwork(network, builder.Build());
    }

    /// <summary>
    /// Adds a ControlNet configuration for guided generation.
    /// </summary>
    /// <param name="controlNet">The ControlNet configuration.</param>
    /// <returns>A new builder instance with the added ControlNet.</returns>
    public TextToImageBuilder WithControlNet(ImageJobControlNet controlNet)
    {
        ArgumentNullException.ThrowIfNull(controlNet);
        var controlNets = _controlNets ?? ImmutableList<ImageJobControlNet>.Empty;
        return new(_httpClient, _options, _model, _paramsBuilder, _additionalNetworks, controlNets.Add(controlNet), _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);
    }

    /// <summary>
    /// Adds a ControlNet configuration using a builder.
    /// </summary>
    /// <param name="controlNetBuilder">The configured ControlNet builder.</param>
    /// <returns>A new builder instance with the added ControlNet.</returns>
    public TextToImageBuilder WithControlNet(ImageJobControlNetBuilder controlNetBuilder)
    {
        ArgumentNullException.ThrowIfNull(controlNetBuilder);
        return WithControlNet(controlNetBuilder.Build());
    }

    /// <summary>
    /// Adds a ControlNet configuration using a configuration action.
    /// </summary>
    /// <param name="configure">Action to configure the ControlNet builder.</param>
    /// <returns>A new builder instance with the added ControlNet.</returns>
    public TextToImageBuilder WithControlNet(Func<ImageJobControlNetBuilder, ImageJobControlNetBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(ImageJobControlNetBuilder.Create());
        return WithControlNet(builder.Build());
    }

    /// <summary>
    /// Sets the number of images to generate.
    /// </summary>
    /// <param name="quantity">The quantity. Range: 1-10, default: 1.</param>
    /// <returns>A new builder instance with the updated quantity.</returns>
    public TextToImageBuilder WithQuantity(int quantity)
        => new(_httpClient, _options, _model, _paramsBuilder, _additionalNetworks, _controlNets, quantity, _priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);

    /// <summary>
    /// Sets the priority configuration for job scheduling.
    /// </summary>
    /// <param name="priority">The priority configuration.</param>
    /// <returns>A new builder instance with the updated priority.</returns>
    public TextToImageBuilder WithPriority(Priority priority)
        => new(_httpClient, _options, _model, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, priority, _properties, _callbackUrl, _retries, _timeout, _clipSkip);

    /// <summary>
    /// Adds a custom property for job tracking and querying.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The property value (must be JSON-serializable).</param>
    /// <returns>A new builder instance with the added property.</returns>
    public TextToImageBuilder WithProperty(string key, JsonElement value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var properties = _properties ?? ImmutableDictionary<string, JsonElement>.Empty;
        return new(_httpClient, _options, _model, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, properties.SetItem(key, value), _callbackUrl, _retries, _timeout, _clipSkip);
    }

    /// <summary>
    /// Sets the webhook URL to call when the job completes.
    /// </summary>
    /// <param name="callbackUrl">The webhook URL.</param>
    /// <returns>A new builder instance with the updated callback URL.</returns>
    public TextToImageBuilder WithCallbackUrl(string callbackUrl)
        => new(_httpClient, _options, _model, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, callbackUrl, _retries, _timeout, _clipSkip);

    /// <summary>
    /// Sets the number of automatic retries on failure.
    /// </summary>
    /// <param name="retries">The number of retry attempts. Default: 0.</param>
    /// <returns>A new builder instance with the updated retry attempts.</returns>
    public TextToImageBuilder WithRetries(int retries)
        => new(_httpClient, _options, _model, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, retries, _timeout, _clipSkip);

    /// <summary>
    /// Sets the job timeout.
    /// </summary>
    /// <param name="timeout">The timeout duration. Format: "HH:mm:ss". Default: "00:10:00".</param>
    /// <returns>A new builder instance with the updated timeout.</returns>
    public TextToImageBuilder WithTimeout(string timeout)
        => new(_httpClient, _options, _model, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, timeout, _clipSkip);

    /// <summary>
    /// Sets the job timeout.
    /// </summary>
    /// <param name="timeout">The timeout duration. Default: 10 minutes.</param>
    /// <returns>A new builder instance with the updated timeout.</returns>
    public TextToImageBuilder WithTimeout(TimeSpan timeout)
        => new(_httpClient, _options, _model, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, timeout.ToString(@"hh\:mm\:ss"), _clipSkip);

    /// <summary>
    /// Sets the number of CLIP layers to skip.
    /// </summary>
    /// <param name="clipSkip">The number of layers to skip. Range: 1-12.</param>
    /// <returns>A new builder instance with the updated CLIP skip.</returns>
    /// <remarks>
    /// A value of 2 is commonly used for anime/Pony models.
    /// This can also be set via <see cref="ImageJobParamsBuilder.WithClipSkip"/>.
    /// </remarks>
    public TextToImageBuilder WithClipSkip(int clipSkip)
        => new(_httpClient, _options, _model, _paramsBuilder, _additionalNetworks, _controlNets, _quantity, _priority, _properties, _callbackUrl, _retries, _timeout, clipSkip);

    /// <summary>
    /// Executes the job submission to the Civitai Generator API.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status collection with a token for polling.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required properties are missing.</exception>
    public Task<Result<JobStatusCollection>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_model == null)
        {
            throw new InvalidOperationException("Model is required. Use WithModel() to set it.");
        }

        if (_paramsBuilder == null)
        {
            throw new InvalidOperationException("Parameters are required. Use WithPrompt() or WithParams() to set them.");
        }

        var request = new TextToImageJobRequest
        {
            Model = _model.Value,
            Params = _paramsBuilder.Build(),
            AdditionalNetworks = _additionalNetworks?.Count > 0 ? _additionalNetworks : null,
            ControlNets = _controlNets?.Count > 0 ? _controlNets : null,
            Quantity = _quantity,
            Priority = _priority,
            Properties = _properties?.Count > 0 ? _properties : null,
            CallbackUrl = _callbackUrl,
            Retries = _retries,
            Timeout = _timeout,
            ClipSkip = _clipSkip
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
        return _httpClient.PostAsync<TextToImageJobRequest, JobStatusCollection>(uri, request, cancellationToken);
    }

    /// <summary>
    /// Executes multiple job submissions as a batch to the Civitai Generator API.
    /// </summary>
    /// <param name="additionalJobs">Additional configured job builders to submit in the same batch.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status collection with a token for polling all jobs.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required properties are missing.</exception>
    public Task<Result<JobStatusCollection>> ExecuteBatchAsync(
        IEnumerable<TextToImageBuilder> additionalJobs,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(additionalJobs);

        // Build all requests including this one
        var allBuilders = new[] { this }.Concat(additionalJobs).ToList();
        var requests = new List<TextToImageJobRequest>();

        foreach (var builder in allBuilders)
        {
            if (builder._model == null)
            {
                throw new InvalidOperationException("All jobs must have a model set. Use WithModel() on all builders.");
            }

            if (builder._paramsBuilder == null)
            {
                throw new InvalidOperationException("All jobs must have parameters set. Use WithPrompt() or WithParams() on all builders.");
            }

            var request = new TextToImageJobRequest
            {
                Model = builder._model.Value,
                Params = builder._paramsBuilder.Build(),
                AdditionalNetworks = builder._additionalNetworks?.Count > 0 ? builder._additionalNetworks : null,
                ControlNets = builder._controlNets?.Count > 0 ? builder._controlNets : null,
                Quantity = builder._quantity,
                Priority = builder._priority,
                Properties = builder._properties?.Count > 0 ? builder._properties : null,
                CallbackUrl = builder._callbackUrl,
                Retries = builder._retries,
                Timeout = builder._timeout,
                ClipSkip = builder._clipSkip
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
