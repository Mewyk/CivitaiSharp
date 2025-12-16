namespace CivitaiSharp.Sdk.Tests.Request;

using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Request;
using Xunit;

public sealed class ImageJobNetworkParamsBuilderTests
{
    [Fact]
    public void Create_ReturnsNewBuilderInstance()
    {
        var builder = ImageJobNetworkParamsBuilder.Create();

        Assert.NotNull(builder);
    }

    [Fact]
    public void WithType_SetsTypeValue()
    {
        var builder = ImageJobNetworkParamsBuilder.Create()
            .WithType(NetworkType.Lora);

        var result = builder.Build();

        Assert.Equal(NetworkType.Lora, result.Type);
    }

    [Fact]
    public void WithStrength_SetsStrengthValue()
    {
        var builder = ImageJobNetworkParamsBuilder.Create()
            .WithType(NetworkType.Lora)
            .WithStrength(0.8m);

        var result = builder.Build();

        Assert.Equal(0.8m, result.Strength);
    }

    [Fact]
    public void WithTriggerWord_SetsTriggerWordValue()
    {
        var builder = ImageJobNetworkParamsBuilder.Create()
            .WithType(NetworkType.Lora)
            .WithTriggerWord("anime style");

        var result = builder.Build();

        Assert.Equal("anime style", result.TriggerWord);
    }

    [Fact]
    public void WithClipStrength_SetsClipStrengthValue()
    {
        var builder = ImageJobNetworkParamsBuilder.Create()
            .WithType(NetworkType.Lora)
            .WithClipStrength(1.2m);

        var result = builder.Build();

        Assert.Equal(1.2m, result.ClipStrength);
    }

    [Fact]
    public void Build_WithoutType_ThrowsInvalidOperationException()
    {
        var builder = ImageJobNetworkParamsBuilder.Create();

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Type is required", exception.Message);
    }

    [Fact]
    public void BuilderIsImmutable_ReturnsNewInstanceOnEachMethod()
    {
        var builder1 = ImageJobNetworkParamsBuilder.Create();
        var builder2 = builder1.WithType(NetworkType.Lora);
        var builder3 = builder2.WithStrength(0.5m);

        Assert.NotSame(builder1, builder2);
        Assert.NotSame(builder2, builder3);
    }

    [Fact]
    public void CompleteFluentAPI_BuildsCorrectNetworkParams()
    {
        var result = ImageJobNetworkParamsBuilder.Create()
            .WithType(NetworkType.Lora)
            .WithStrength(0.9m)
            .WithTriggerWord("cyberpunk")
            .WithClipStrength(1.1m)
            .Build();

        Assert.Equal(NetworkType.Lora, result.Type);
        Assert.Equal(0.9m, result.Strength);
        Assert.Equal("cyberpunk", result.TriggerWord);
        Assert.Equal(1.1m, result.ClipStrength);
    }
}
