namespace CivitaiSharp.Sdk.Tests.Request;

using System.Text.Json;
using CivitaiSharp.Sdk.Air;
using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Models.Jobs;
using CivitaiSharp.Sdk.Request;
using CivitaiSharp.Sdk.Services;
using NSubstitute;
using Xunit;

public sealed class TextToImageJobBuilderTests : IClassFixture<SdkTestFixture>
{
    private readonly IJobsService _mockJobsService;

    public TextToImageJobBuilderTests()
    {
        _mockJobsService = Substitute.For<IJobsService>();
    }

    [Fact]
    public void Create_WithJobsService_ReturnsBuilderWithService()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService);

        Assert.NotNull(builder);
        Assert.NotNull(builder.JobsService);
    }

    [Fact]
    public void WithModel_SetsModelValue()
    {
        var model = AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072");
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(model)
            .WithPrompt("test");

        var result = builder.Build();

        Assert.Equal(model, result.Model);
    }

    [Fact]
    public void WithPrompt_SetsPromptInParams()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("A beautiful landscape");

        var result = builder.Build();

        Assert.Equal("A beautiful landscape", result.Params.Prompt);
    }

    [Fact]
    public void WithNegativePrompt_SetsNegativePromptInParams()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("positive")
            .WithNegativePrompt("negative");

        var result = builder.Build();

        Assert.Equal("negative", result.Params.NegativePrompt);
    }

    [Fact]
    public void WithSize_SetsSizeInParams()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithSize(1024, 768);

        var result = builder.Build();

        Assert.Equal(1024, result.Params.Width);
        Assert.Equal(768, result.Params.Height);
    }

    [Fact]
    public void WithSteps_SetsStepsInParams()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithSteps(30);

        var result = builder.Build();

        Assert.Equal(30, result.Params.Steps);
    }

    [Fact]
    public void WithCfgScale_SetsCfgScaleInParams()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithCfgScale(7.5m);

        var result = builder.Build();

        Assert.Equal(7.5m, result.Params.CfgScale);
    }

    [Fact]
    public void WithSeed_SetsSeedInParams()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithSeed(42);

        var result = builder.Build();

        Assert.Equal(42, result.Params.Seed);
    }

    [Fact]
    public void WithParams_UsesProvidedBuilder()
    {
        var paramsBuilder = ImageJobParamsBuilder.Create()
            .WithPrompt("custom prompt")
            .WithSteps(25);

        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithParams(paramsBuilder);

        var result = builder.Build();

        Assert.Equal("custom prompt", result.Params.Prompt);
        Assert.Equal(25, result.Params.Steps);
    }

    [Fact]
    public void WithParams_WithConfigureAction_ConfiguresParams()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithParams(p => p
                .WithPrompt("configured")
                .WithSteps(20)
                .WithCfgScale(7.0m));

        var result = builder.Build();

        Assert.Equal("configured", result.Params.Prompt);
        Assert.Equal(20, result.Params.Steps);
        Assert.Equal(7.0m, result.Params.CfgScale);
    }

    [Fact]
    public void AddAdditionalNetwork_WithParams_AddsNetworkToDictionary()
    {
        var networkId = AirIdentifier.Parse("urn:air:sdxl:lora:civitai:1234@5678");
        var networkParams = new ImageJobNetworkParams { Type = NetworkType.Lora };

        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .AddAdditionalNetwork(networkId, networkParams);

        var result = builder.Build();

        Assert.NotNull(result.AdditionalNetworks);
        Assert.Single(result.AdditionalNetworks);
        Assert.Equal(NetworkType.Lora, result.AdditionalNetworks[networkId].Type);
    }

    [Fact]
    public void AddAdditionalNetwork_WithBuilder_AddsNetworkUsingBuilder()
    {
        var networkId = AirIdentifier.Parse("urn:air:sdxl:lora:civitai:1234@5678");
        var networkBuilder = ImageJobNetworkParamsBuilder.Create()
            .WithType(NetworkType.Lora)
            .WithStrength(0.8m);

        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .AddAdditionalNetwork(networkId, networkBuilder);

        var result = builder.Build();

        Assert.NotNull(result.AdditionalNetworks);
        Assert.Equal(0.8m, result.AdditionalNetworks[networkId].Strength);
    }

    [Fact]
    public void AddAdditionalNetwork_WithConfigureAction_AddsNetworkUsingAction()
    {
        var networkId = AirIdentifier.Parse("urn:air:sdxl:lora:civitai:1234@5678");

        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .AddAdditionalNetwork(networkId, n => n
                .WithType(NetworkType.Lora)
                .WithTriggerWord("anime"));

        var result = builder.Build();

        Assert.NotNull(result.AdditionalNetworks);
        Assert.Equal("anime", result.AdditionalNetworks[networkId].TriggerWord);
    }

    [Fact]
    public void AddControlNet_WithControlNet_AddsToList()
    {
        var controlNet = new ImageJobControlNet { ImageUrl = "https://example.com/control.png" };

        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .AddControlNet(controlNet);

        var result = builder.Build();

        Assert.NotNull(result.ControlNets);
        Assert.Single(result.ControlNets);
        Assert.Equal("https://example.com/control.png", result.ControlNets[0].ImageUrl);
    }

    [Fact]
    public void AddControlNet_WithBuilder_AddsUsingBuilder()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .AddControlNet(ImageJobControlNetBuilder.Create()
                .WithImageUrl("https://example.com/control.png")
                .WithWeight(0.9m));

        var result = builder.Build();

        Assert.NotNull(result.ControlNets);
        Assert.Equal(0.9m, result.ControlNets[0].Weight);
    }

    [Fact]
    public void AddControlNet_WithConfigureAction_AddsUsingAction()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .AddControlNet(c => c
                .WithImageUrl("https://example.com/control.png")
                .WithPreprocessor(ControlNetPreprocessor.Canny));

        var result = builder.Build();

        Assert.NotNull(result.ControlNets);
        Assert.Equal(ControlNetPreprocessor.Canny, result.ControlNets[0].Preprocessor);
    }

    [Fact]
    public void WithQuantity_SetsQuantityValue()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithQuantity(4);

        var result = builder.Build();

        Assert.Equal(4, result.Quantity);
    }

    [Fact]
    public void WithPriority_SetsPriorityValue()
    {
        var priority = new Priority(5.0m);
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithPriority(priority);

        var result = builder.Build();

        Assert.Equal(priority, result.Priority);
    }

    [Fact]
    public void AddProperty_AddsPropertyToDictionary()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .AddProperty("key1", JsonDocument.Parse("\"value1\"").RootElement);

        var result = builder.Build();

        Assert.NotNull(result.Properties);
        Assert.Single(result.Properties);
        Assert.Equal("value1", result.Properties["key1"].GetString());
    }

    [Fact]
    public void WithCallbackUrl_SetsCallbackUrlValue()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithCallbackUrl("https://webhook.example.com");

        var result = builder.Build();

        Assert.Equal("https://webhook.example.com", result.CallbackUrl);
    }

    [Fact]
    public void WithRetries_SetsRetriesValue()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithRetries(3);

        var result = builder.Build();

        Assert.Equal(3, result.Retries);
    }

    [Fact]
    public void WithTimeout_String_SetsTimeoutValue()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithTimeout("00:15:00");

        var result = builder.Build();

        Assert.Equal("00:15:00", result.Timeout);
    }

    [Fact]
    public void WithTimeout_TimeSpan_FormatsAndSetsTimeout()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithTimeout(TimeSpan.FromMinutes(20));

        var result = builder.Build();

        Assert.Equal("00:20:00", result.Timeout);
    }

    [Fact]
    public void WithClipSkip_SetsClipSkipValue()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"))
            .WithPrompt("test")
            .WithClipSkip(2);

        var result = builder.Build();

        Assert.Equal(2, result.ClipSkip);
    }

    [Fact]
    public void Build_WithoutModel_ThrowsInvalidOperationException()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithPrompt("test");

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Model is required", exception.Message);
    }

    [Fact]
    public void Build_WithoutPrompt_ThrowsInvalidOperationException()
    {
        var builder = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"));

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Parameters are required", exception.Message);
    }

    [Fact]
    public void BuilderIsImmutable_ReturnsNewInstanceOnEachMethod()
    {
        var builder1 = TextToImageJobBuilder.Create(_mockJobsService);
        var builder2 = builder1.WithModel(AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072"));
        var builder3 = builder2.WithPrompt("test");

        Assert.NotSame(builder1, builder2);
        Assert.NotSame(builder2, builder3);
    }

    [Fact]
    public void CompleteFluentAPI_BuildsCorrectRequest()
    {
        var model = AirIdentifier.Parse("urn:air:sdxl:checkpoint:civitai:4201@130072");
        var networkId = AirIdentifier.Parse("urn:air:sdxl:lora:civitai:1234@5678");

        var result = TextToImageJobBuilder.Create(_mockJobsService)
            .WithModel(model)
            .WithPrompt("A beautiful sunset over mountains")
            .WithNegativePrompt("blurry, low quality")
            .WithSize(1024, 1024)
            .WithSteps(30)
            .WithCfgScale(7.5m)
            .WithSeed(42)
            .AddAdditionalNetwork(networkId, n => n
                .WithType(NetworkType.Lora)
                .WithStrength(0.8m))
            .AddControlNet(c => c
                .WithImageUrl("https://example.com/control.png")
                .WithWeight(0.9m))
            .WithQuantity(2)
            .WithCallbackUrl("https://webhook.example.com")
            .Build();

        Assert.Equal(model, result.Model);
        Assert.Equal("A beautiful sunset over mountains", result.Params.Prompt);
        Assert.Equal("blurry, low quality", result.Params.NegativePrompt);
        Assert.Equal(1024, result.Params.Width);
        Assert.Equal(1024, result.Params.Height);
        Assert.Equal(30, result.Params.Steps);
        Assert.Equal(7.5m, result.Params.CfgScale);
        Assert.Equal(42, result.Params.Seed);
        Assert.NotNull(result.AdditionalNetworks);
        Assert.Single(result.AdditionalNetworks);
        Assert.NotNull(result.ControlNets);
        Assert.Single(result.ControlNets);
        Assert.Equal(2, result.Quantity);
        Assert.Equal("https://webhook.example.com", result.CallbackUrl);
    }
}

