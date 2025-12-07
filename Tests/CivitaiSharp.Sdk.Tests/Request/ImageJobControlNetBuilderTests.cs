namespace CivitaiSharp.Sdk.Tests.Request;

using CivitaiSharp.Sdk.Enums;
using CivitaiSharp.Sdk.Request;
using Xunit;

public sealed class ImageJobControlNetBuilderTests
{
    [Fact]
    public void Create_ReturnsNewBuilderInstance()
    {
        var builder = ImageJobControlNetBuilder.Create();

        Assert.NotNull(builder);
    }

    [Fact]
    public void WithImageUrl_SetsImageUrlAndClearsImageData()
    {
        var builder = ImageJobControlNetBuilder.Create()
            .WithImageUrl("https://example.com/control.png");

        var result = builder.Build();

        Assert.Equal("https://example.com/control.png", result.ImageUrl);
        Assert.Null(result.Image);
    }

    [Fact]
    public void WithImageData_SetsImageDataAndClearsImageUrl()
    {
        var builder = ImageJobControlNetBuilder.Create()
            .WithImageData("data:image/png;base64,iVBORw0KGgo=");

        var result = builder.Build();

        Assert.Equal("data:image/png;base64,iVBORw0KGgo=", result.Image);
        Assert.Null(result.ImageUrl);
    }

    [Fact]
    public void WithImageUrl_AfterWithImageData_OverwritesImageData()
    {
        var builder = ImageJobControlNetBuilder.Create()
            .WithImageData("data:image/png;base64,iVBORw0KGgo=")
            .WithImageUrl("https://example.com/control.png");

        var result = builder.Build();

        Assert.Equal("https://example.com/control.png", result.ImageUrl);
        Assert.Null(result.Image);
    }

    [Fact]
    public void WithPreprocessor_SetsPreprocessorValue()
    {
        var builder = ImageJobControlNetBuilder.Create()
            .WithImageUrl("https://example.com/control.png")
            .WithPreprocessor(ControlNetPreprocessor.Canny);

        var result = builder.Build();

        Assert.Equal(ControlNetPreprocessor.Canny, result.Preprocessor);
    }

    [Fact]
    public void WithWeight_SetsWeightValue()
    {
        var builder = ImageJobControlNetBuilder.Create()
            .WithImageUrl("https://example.com/control.png")
            .WithWeight(0.9m);

        var result = builder.Build();

        Assert.Equal(0.9m, result.Weight);
    }

    [Fact]
    public void WithStartStep_SetsStartStepValue()
    {
        var builder = ImageJobControlNetBuilder.Create()
            .WithImageUrl("https://example.com/control.png")
            .WithStartStep(0.1m);

        var result = builder.Build();

        Assert.Equal(0.1m, result.StartStep);
    }

    [Fact]
    public void WithEndStep_SetsEndStepValue()
    {
        var builder = ImageJobControlNetBuilder.Create()
            .WithImageUrl("https://example.com/control.png")
            .WithEndStep(0.9m);

        var result = builder.Build();

        Assert.Equal(0.9m, result.EndStep);
    }

    [Fact]
    public void WithStepRange_SetsBothStartAndEndStep()
    {
        var builder = ImageJobControlNetBuilder.Create()
            .WithImageUrl("https://example.com/control.png")
            .WithStepRange(0.2m, 0.8m);

        var result = builder.Build();

        Assert.Equal(0.2m, result.StartStep);
        Assert.Equal(0.8m, result.EndStep);
    }

    [Fact]
    public void Build_WithoutImageSource_ThrowsInvalidOperationException()
    {
        var builder = ImageJobControlNetBuilder.Create();

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Either ImageUrl or ImageData must be provided", exception.Message);
    }

    [Fact]
    public void BuilderIsImmutable_ReturnsNewInstanceOnEachMethod()
    {
        var builder1 = ImageJobControlNetBuilder.Create();
        var builder2 = builder1.WithImageUrl("https://example.com/control.png");
        var builder3 = builder2.WithWeight(0.5m);

        Assert.NotSame(builder1, builder2);
        Assert.NotSame(builder2, builder3);
    }

    [Fact]
    public void CompleteFluentAPI_BuildsCorrectControlNet()
    {
        var result = ImageJobControlNetBuilder.Create()
            .WithImageUrl("https://example.com/control.png")
            .WithPreprocessor(ControlNetPreprocessor.Depth)
            .WithWeight(0.85m)
            .WithStepRange(0.1m, 0.9m)
            .Build();

        Assert.Equal("https://example.com/control.png", result.ImageUrl);
        Assert.Equal(ControlNetPreprocessor.Depth, result.Preprocessor);
        Assert.Equal(0.85m, result.Weight);
        Assert.Equal(0.1m, result.StartStep);
        Assert.Equal(0.9m, result.EndStep);
    }
}
