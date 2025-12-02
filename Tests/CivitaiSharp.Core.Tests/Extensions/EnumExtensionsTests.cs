namespace CivitaiSharp.Core.Tests.Extensions;

using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Core.Models;
using Xunit;

public sealed class EnumExtensionsTests
{
    [Fact]
    public void WhenConvertingEnumWithAttributeThenReturnsAttributeValue()
    {
        // Arrange
        var sort = ModelSort.HighestRated;

        // Act
        var result = sort.ToApiString();

        // Assert
        Assert.Equal("Highest Rated", result);
    }

    [Fact]
    public void WhenConvertingEnumWithoutAttributeThenReturnsEnumName()
    {
        // Arrange
        var sort = ModelSort.Newest;

        // Act
        var result = sort.ToApiString();

        // Assert
        Assert.Equal("Newest", result);
    }

    [Theory]
    [InlineData(ModelSort.HighestRated, "Highest Rated")]
    [InlineData(ModelSort.MostDownloaded, "Most Downloaded")]
    [InlineData(ModelSort.Newest, "Newest")]
    public void WhenConvertingModelSortThenReturnsExpectedValue(ModelSort sort, string expected)
    {
        // Act
        var result = sort.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ImageSort.MostReactions, "Most Reactions")]
    [InlineData(ImageSort.MostComments, "Most Comments")]
    [InlineData(ImageSort.MostCollected, "Most Collected")]
    [InlineData(ImageSort.Newest, "Newest")]
    [InlineData(ImageSort.Oldest, "Oldest")]
    [InlineData(ImageSort.Random, "Random")]
    public void WhenConvertingImageSortThenReturnsExpectedValue(ImageSort sort, string expected)
    {
        // Act
        var result = sort.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(TimePeriod.AllTime, "AllTime")]
    [InlineData(TimePeriod.Year, "Year")]
    [InlineData(TimePeriod.Month, "Month")]
    [InlineData(TimePeriod.Week, "Week")]
    [InlineData(TimePeriod.Day, "Day")]
    public void WhenConvertingTimePeriodThenReturnsExpectedValue(TimePeriod period, string expected)
    {
        // Act
        var result = period.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ModelType.Checkpoint, "Checkpoint")]
    [InlineData(ModelType.Lora, "LORA")]
    [InlineData(ModelType.Vae, "VAE")]
    [InlineData(ModelType.DoRa, "DoRA")]
    [InlineData(ModelType.LoCon, "LoCon")]
    public void WhenConvertingModelTypeThenReturnsExpectedValue(ModelType type, string expected)
    {
        // Act
        var result = type.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ImageNsfwLevel.None, "None")]
    [InlineData(ImageNsfwLevel.Soft, "Soft")]
    [InlineData(ImageNsfwLevel.Mature, "Mature")]
    [InlineData(ImageNsfwLevel.Explicit, "X")]
    public void WhenConvertingImageNsfwLevelThenReturnsExpectedValue(ImageNsfwLevel level, string expected)
    {
        // Act
        var result = level.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(CommercialUsePermission.None, "None")]
    [InlineData(CommercialUsePermission.Image, "Image")]
    [InlineData(CommercialUsePermission.Rent, "Rent")]
    [InlineData(CommercialUsePermission.Sell, "Sell")]
    public void WhenConvertingCommercialUsePermissionThenReturnsExpectedValue(CommercialUsePermission permission, string expected)
    {
        // Act
        var result = permission.ToApiString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void WhenConvertingSameEnumMultipleTimesThenReturnsCachedValue()
    {
        // Arrange
        var sort = ModelSort.HighestRated;

        // Act - Call multiple times to ensure caching works
        var result1 = sort.ToApiString();
        var result2 = sort.ToApiString();
        var result3 = sort.ToApiString();

        // Assert - All should be the same reference (cached)
        Assert.Equal("Highest Rated", result1);
        Assert.Equal("Highest Rated", result2);
        Assert.Equal("Highest Rated", result3);
    }
}
