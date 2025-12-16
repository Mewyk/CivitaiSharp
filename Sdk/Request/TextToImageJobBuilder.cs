namespace CivitaiSharp.Sdk.Request;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Models.Results;
using CivitaiSharp.Sdk.Services;

/// <summary>
/// Fluent builder for constructing and submitting text-to-image generation jobs.
/// </summary>
/// <remarks>
/// This builder follows an immutable design pattern. Each method returns a new instance
/// with the updated configuration, making it thread-safe and cacheable.
/// </remarks>
public sealed record TextToImageJobBuilder(
    IJobsService? JobsService = null,
    AirIdentifier? Model = null,
    ImageJobParamsBuilder? ParamsBuilder = null,
    Dictionary<AirIdentifier, ImageJobNetworkParams>? AdditionalNetworks = null,
    List<ImageJobControlNet>? ControlNets = null,
    int? Quantity = null,
    Priority? Priority = null,
    Dictionary<string, JsonElement>? Properties = null,
    string? CallbackUrl = null,
    int? Retries = null,
    string? Timeout = null,
    int? ClipSkip = null)
{

    /// <summary>
    /// Creates a new <see cref="TextToImageJobBuilder"/> instance with the jobs service for submission.
    /// </summary>
    /// <param name="jobsService">The jobs service for submitting jobs.</param>
    /// <returns>A new builder instance.</returns>
    public static TextToImageJobBuilder Create(IJobsService jobsService)
    {
        ArgumentNullException.ThrowIfNull(jobsService);
        return new TextToImageJobBuilder(JobsService: jobsService);
    }

    /// <summary>
    /// Sets the base model to use for generation.
    /// </summary>
    /// <param name="model">The AIR identifier for the model. Required.</param>
    /// <returns>A new builder instance with the updated model.</returns>
    /// <example>urn:air:sdxl:checkpoint:civitai:4201@130072</example>
    public TextToImageJobBuilder WithModel(AirIdentifier model)
        => this with { Model = model };

    /// <summary>
    /// Sets the positive prompt for image generation.
    /// </summary>
    /// <param name="prompt">The prompt text describing what to generate. Required.</param>
    /// <returns>A new builder instance with the updated prompt.</returns>
    public TextToImageJobBuilder WithPrompt(string prompt)
    {
        var builder = ParamsBuilder ?? ImageJobParamsBuilder.Create();
        return this with { ParamsBuilder = builder.WithPrompt(prompt) };
    }

    /// <summary>
    /// Sets the negative prompt describing what to avoid.
    /// </summary>
    /// <param name="negativePrompt">The negative prompt text.</param>
    /// <returns>A new builder instance with the updated negative prompt.</returns>
    public TextToImageJobBuilder WithNegativePrompt(string negativePrompt)
    {
        var builder = ParamsBuilder ?? ImageJobParamsBuilder.Create();
        return this with { ParamsBuilder = builder.WithNegativePrompt(negativePrompt) };
    }

    /// <summary>
    /// Sets the image dimensions.
    /// </summary>
    /// <param name="width">The width in pixels. Must be a multiple of 8. Range: 64-2048.</param>
    /// <param name="height">The height in pixels. Must be a multiple of 8. Range: 64-2048.</param>
    /// <returns>A new builder instance with the updated dimensions.</returns>
    public TextToImageJobBuilder WithSize(int width, int height)
    {
        var builder = ParamsBuilder ?? ImageJobParamsBuilder.Create();
        return this with { ParamsBuilder = builder.WithSize(width, height) };
    }

    /// <summary>
    /// Sets the number of sampling steps.
    /// </summary>
    /// <param name="steps">The step count. Range: 1-100, default: 20.</param>
    /// <returns>A new builder instance with the updated steps.</returns>
    public TextToImageJobBuilder WithSteps(int steps)
    {
        var builder = ParamsBuilder ?? ImageJobParamsBuilder.Create();
        return this with { ParamsBuilder = builder.WithSteps(steps) };
    }

    /// <summary>
    /// Sets the classifier-free guidance scale.
    /// </summary>
    /// <param name="cfgScale">The CFG scale. Range: 1-30, default: 7.0.</param>
    /// <returns>A new builder instance with the updated CFG scale.</returns>
    public TextToImageJobBuilder WithCfgScale(decimal cfgScale)
    {
        var builder = ParamsBuilder ?? ImageJobParamsBuilder.Create();
        return this with { ParamsBuilder = builder.WithCfgScale(cfgScale) };
    }

    /// <summary>
    /// Sets the random seed for reproducible generation.
    /// </summary>
    /// <param name="seed">The seed value.</param>
    /// <returns>A new builder instance with the updated seed.</returns>
    public TextToImageJobBuilder WithSeed(long seed)
    {
        var builder = ParamsBuilder ?? ImageJobParamsBuilder.Create();
        return this with { ParamsBuilder = builder.WithSeed(seed) };
    }

    /// <summary>
    /// Configures generation parameters using a custom builder.
    /// </summary>
    /// <param name="paramsBuilder">The configured parameters builder.</param>
    /// <returns>A new builder instance with the updated parameters.</returns>
    public TextToImageJobBuilder WithParams(ImageJobParamsBuilder paramsBuilder)
        => this with { ParamsBuilder = paramsBuilder };

    /// <summary>
    /// Configures generation parameters using a configuration action.
    /// </summary>
    /// <param name="configure">Action to configure the parameters builder.</param>
    /// <returns>A new builder instance with the updated parameters.</returns>
    public TextToImageJobBuilder WithParams(Func<ImageJobParamsBuilder, ImageJobParamsBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = ParamsBuilder ?? ImageJobParamsBuilder.Create();
        return this with { ParamsBuilder = configure(builder) };
    }

    /// <summary>
    /// Adds an additional network (LoRA, embedding, etc.) to the generation.
    /// </summary>
    /// <param name="network">The AIR identifier for the network.</param>
    /// <param name="networkParams">The network configuration.</param>
    /// <returns>A new builder instance with the added network.</returns>
    public TextToImageJobBuilder AddAdditionalNetwork(AirIdentifier network, ImageJobNetworkParams networkParams)
    {
        ArgumentNullException.ThrowIfNull(networkParams);
        var networks = AdditionalNetworks != null
            ? new Dictionary<AirIdentifier, ImageJobNetworkParams>(AdditionalNetworks)
            : new Dictionary<AirIdentifier, ImageJobNetworkParams>();
        networks[network] = networkParams;
        return this with { AdditionalNetworks = networks };
    }

    /// <summary>
    /// Adds an additional network (LoRA, embedding, etc.) using a builder.
    /// </summary>
    /// <param name="network">The AIR identifier for the network.</param>
    /// <param name="networkBuilder">The configured network parameters builder.</param>
    /// <returns>A new builder instance with the added network.</returns>
    public TextToImageJobBuilder AddAdditionalNetwork(AirIdentifier network, ImageJobNetworkParamsBuilder networkBuilder)
    {
        ArgumentNullException.ThrowIfNull(networkBuilder);
        return AddAdditionalNetwork(network, networkBuilder.Build());
    }

    /// <summary>
    /// Adds an additional network (LoRA, embedding, etc.) using a configuration action.
    /// </summary>
    /// <param name="network">The AIR identifier for the network.</param>
    /// <param name="configure">Action to configure the network parameters builder.</param>
    /// <returns>A new builder instance with the added network.</returns>
    public TextToImageJobBuilder AddAdditionalNetwork(AirIdentifier network, Func<ImageJobNetworkParamsBuilder, ImageJobNetworkParamsBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(ImageJobNetworkParamsBuilder.Create());
        return AddAdditionalNetwork(network, builder.Build());
    }

    /// <summary>
    /// Adds a ControlNet configuration for guided generation.
    /// </summary>
    /// <param name="controlNet">The ControlNet configuration.</param>
    /// <returns>A new builder instance with the added ControlNet.</returns>
    public TextToImageJobBuilder AddControlNet(ImageJobControlNet controlNet)
    {
        ArgumentNullException.ThrowIfNull(controlNet);
        var controlNets = ControlNets != null
            ? new List<ImageJobControlNet>(ControlNets)
            : new List<ImageJobControlNet>();
        controlNets.Add(controlNet);
        return this with { ControlNets = controlNets };
    }

    /// <summary>
    /// Adds a ControlNet configuration using a builder.
    /// </summary>
    /// <param name="controlNetBuilder">The configured ControlNet builder.</param>
    /// <returns>A new builder instance with the added ControlNet.</returns>
    public TextToImageJobBuilder AddControlNet(ImageJobControlNetBuilder controlNetBuilder)
    {
        ArgumentNullException.ThrowIfNull(controlNetBuilder);
        return AddControlNet(controlNetBuilder.Build());
    }

    /// <summary>
    /// Adds a ControlNet configuration using a configuration action.
    /// </summary>
    /// <param name="configure">Action to configure the ControlNet builder.</param>
    /// <returns>A new builder instance with the added ControlNet.</returns>
    public TextToImageJobBuilder AddControlNet(Func<ImageJobControlNetBuilder, ImageJobControlNetBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = configure(ImageJobControlNetBuilder.Create());
        return AddControlNet(builder.Build());
    }

    /// <summary>
    /// Sets the number of images to generate.
    /// </summary>
    /// <param name="quantity">The quantity. Range: 1-10, default: 1.</param>
    /// <returns>A new builder instance with the updated quantity.</returns>
    public TextToImageJobBuilder WithQuantity(int quantity)
        => this with { Quantity = quantity };

    /// <summary>
    /// Sets the priority configuration for job scheduling.
    /// </summary>
    /// <param name="priority">The priority configuration.</param>
    /// <returns>A new builder instance with the updated priority.</returns>
    public TextToImageJobBuilder WithPriority(Priority priority)
        => this with { Priority = priority };

    /// <summary>
    /// Adds a custom property for job tracking and querying.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The property value (must be JSON-serializable).</param>
    /// <returns>A new builder instance with the added property.</returns>
    public TextToImageJobBuilder AddProperty(string key, JsonElement value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var properties = Properties != null
            ? new Dictionary<string, JsonElement>(Properties)
            : new Dictionary<string, JsonElement>();
        properties[key] = value;
        return this with { Properties = properties };
    }

    /// <summary>
    /// Sets the webhook URL to call when the job completes.
    /// </summary>
    /// <param name="callbackUrl">The webhook URL.</param>
    /// <returns>A new builder instance with the updated callback URL.</returns>
    public TextToImageJobBuilder WithCallbackUrl(string callbackUrl)
        => this with { CallbackUrl = callbackUrl };

    /// <summary>
    /// Sets the number of automatic retries on failure.
    /// </summary>
    /// <param name="retries">The number of retry attempts. Default: 0.</param>
    /// <returns>A new builder instance with the updated retry attempts.</returns>
    public TextToImageJobBuilder WithRetries(int retries)
        => this with { Retries = retries };

    /// <summary>
    /// Sets the job timeout.
    /// </summary>
    /// <param name="timeout">The timeout duration. Format: "HH:mm:ss". Default: "00:10:00".</param>
    /// <returns>A new builder instance with the updated timeout.</returns>
    public TextToImageJobBuilder WithTimeout(string timeout)
        => this with { Timeout = timeout };

    /// <summary>
    /// Sets the job timeout.
    /// </summary>
    /// <param name="timeout">The timeout duration. Default: 10 minutes.</param>
    /// <returns>A new builder instance with the updated timeout.</returns>
    public TextToImageJobBuilder WithTimeout(TimeSpan timeout)
        => this with { Timeout = timeout.ToString(@"hh\:mm\:ss") };

    /// <summary>
    /// Sets the number of CLIP layers to skip.
    /// </summary>
    /// <param name="clipSkip">The number of layers to skip. Range: 1-12.</param>
    /// <returns>A new builder instance with the updated CLIP skip.</returns>
    /// <remarks>
    /// A value of 2 is commonly used for anime/Pony models.
    /// This can also be set via <see cref="ImageJobParamsBuilder.WithClipSkip"/>.
    /// </remarks>
    public TextToImageJobBuilder WithClipSkip(int clipSkip)
        => this with { ClipSkip = clipSkip };

    /// <summary>
    /// Builds the <see cref="TextToImageJobRequest"/> instance.
    /// </summary>
    /// <returns>The configured <see cref="TextToImageJobRequest"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required properties are missing.</exception>
    public TextToImageJobRequest Build()
    {
        if (Model == null)
        {
            throw new InvalidOperationException("Model is required. Use WithModel() to set it.");
        }

        if (ParamsBuilder == null)
        {
            throw new InvalidOperationException("Parameters are required. Use WithPrompt() or WithParams() to set them.");
        }

        return new TextToImageJobRequest
        {
            Model = Model.Value,
            Params = ParamsBuilder.Build(),
            AdditionalNetworks = AdditionalNetworks?.Count > 0
                ? AdditionalNetworks
                : null,
            ControlNets = ControlNets?.Count > 0
                ? ControlNets.AsReadOnly()
                : null,
            Quantity = Quantity,
            Priority = Priority,
            Properties = Properties?.Count > 0
                ? Properties
                : null,
            CallbackUrl = CallbackUrl,
            Retries = Retries,
            Timeout = Timeout,
            ClipSkip = ClipSkip
        };
    }

    /// <summary>
    /// Submits the job to the Civitai API.
    /// </summary>
    /// <param name="wait">If true, blocks until the job completes (up to ~10 minutes).</param>
    /// <param name="detailed">If true, includes the original job specification in the response.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the job status collection with a token for polling.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the builder was not created via the jobs service factory method.
    /// </exception>
    public Task<Result<JobStatusCollection>> SubmitAsync(
        bool wait = false,
        bool detailed = false,
        CancellationToken cancellationToken = default)
    {
        if (JobsService == null)
        {
            throw new InvalidOperationException(
                "Cannot submit job. This builder was not created via IJobsService.CreateTextToImage().");
        }

        var request = Build();
        return JobsService.SubmitAsync(request, wait, detailed, cancellationToken);
    }
}
