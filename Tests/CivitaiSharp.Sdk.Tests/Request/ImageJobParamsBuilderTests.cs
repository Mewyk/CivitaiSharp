namespace CivitaiSharp.Sdk.Tests.Request;

using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Request;
using Xunit;

public sealed class ImageJobParamsBuilderTests
{
    [Fact]
    public void Create_ReturnsNewBuilderInstance()
    {
        var builder = ImageJobParamsBuilder.Create();

        Assert.NotNull(builder);
    }

    [Fact]
    public void WithPrompt_SetsPromptValue()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test prompt");

        var result = builder.Build();

        Assert.Equal("test prompt", result.Prompt);
    }

    [Fact]
    public void WithNegativePrompt_SetsNegativePromptValue()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("positive")
            .WithNegativePrompt("negative");

        var result = builder.Build();

        Assert.Equal("negative", result.NegativePrompt);
    }

    [Fact]
    public void WithScheduler_SetsSchedulerValue()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test")
            .WithScheduler(Scheduler.EulerAncestral);

        var result = builder.Build();

        Assert.Equal(Scheduler.EulerAncestral, result.Scheduler);
    }

    [Fact]
    public void WithSteps_SetsStepsValue()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test")
            .WithSteps(30);

        var result = builder.Build();

        Assert.Equal(30, result.Steps);
    }

    [Fact]
    public void WithCfgScale_SetsCfgScaleValue()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test")
            .WithCfgScale(7.5m);

        var result = builder.Build();

        Assert.Equal(7.5m, result.CfgScale);
    }

    [Fact]
    public void WithSize_SetsBothWidthAndHeight()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test")
            .WithSize(1024, 768);

        var result = builder.Build();

        Assert.Equal(1024, result.Width);
        Assert.Equal(768, result.Height);
    }

    [Fact]
    public void WithWidth_SetsWidthOnly()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test")
            .WithWidth(512);

        var result = builder.Build();

        Assert.Equal(512, result.Width);
        Assert.Null(result.Height);
    }

    [Fact]
    public void WithHeight_SetsHeightOnly()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test")
            .WithHeight(768);

        var result = builder.Build();

        Assert.Equal(768, result.Height);
        Assert.Null(result.Width);
    }

    [Fact]
    public void WithSeed_SetsSeedValue()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test")
            .WithSeed(123456789L);

        var result = builder.Build();

        Assert.Equal(123456789L, result.Seed);
    }

    [Fact]
    public void WithClipSkip_SetsClipSkipValue()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test")
            .WithClipSkip(2);

        var result = builder.Build();

        Assert.Equal(2, result.ClipSkip);
    }

    [Fact]
    public void WithSourceImage_SetsImageUrl()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test")
            .WithSourceImage("https://example.com/image.png");

        var result = builder.Build();

        Assert.Equal("https://example.com/image.png", result.Image);
    }

    [Fact]
    public void WithStrength_SetsStrengthValue()
    {
        var builder = ImageJobParamsBuilder.Create()
            .WithPrompt("test")
            .WithStrength(0.7m);

        var result = builder.Build();

        Assert.Equal(0.7m, result.Strength);
    }

    [Fact]
    public void Build_WithoutPrompt_ThrowsInvalidOperationException()
    {
        var builder = ImageJobParamsBuilder.Create();

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Prompt is required", exception.Message);
    }

    [Fact]
    public void BuilderIsImmutable_ReturnsNewInstanceOnEachMethod()
    {
        var builder1 = ImageJobParamsBuilder.Create();
        var builder2 = builder1.WithPrompt("test");
        var builder3 = builder2.WithSteps(30);

        Assert.NotSame(builder1, builder2);
        Assert.NotSame(builder2, builder3);
    }

    [Fact]
    public void CompleteFluentAPI_BuildsCorrectParams()
    {
        var result = ImageJobParamsBuilder.Create()
            .WithPrompt("A beautiful sunset")
            .WithNegativePrompt("blurry, low quality")
            .WithScheduler(Scheduler.DpmPlusPlus2M)
            .WithSteps(25)
            .WithCfgScale(7.0m)
            .WithSize(1024, 1024)
            .WithSeed(42)
            .WithClipSkip(2)
            .Build();

        Assert.Equal("A beautiful sunset", result.Prompt);
        Assert.Equal("blurry, low quality", result.NegativePrompt);
        Assert.Equal(Scheduler.DpmPlusPlus2M, result.Scheduler);
        Assert.Equal(25, result.Steps);
        Assert.Equal(7.0m, result.CfgScale);
        Assert.Equal(1024, result.Width);
        Assert.Equal(1024, result.Height);
        Assert.Equal(42, result.Seed);
        Assert.Equal(2, result.ClipSkip);
    }
}
