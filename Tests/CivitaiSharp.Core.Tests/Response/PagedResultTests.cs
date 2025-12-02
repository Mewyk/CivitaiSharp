namespace CivitaiSharp.Core.Tests.Response;

using CivitaiSharp.Core.Models.Common;
using CivitaiSharp.Core.Response;
using Xunit;

public sealed class PagedResultTests
{
    [Fact]
    public void WhenCreatingPagedResultThenItemsAreSet()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };

        // Act
        var result = new PagedResult<string>(items);

        // Assert
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("item1", result.Items[0]);
        Assert.Null(result.Metadata);
    }

    [Fact]
    public void WhenCreatingPagedResultWithMetadataThenMetadataIsSet()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };
        var metadata = new PaginationMetadata(
            TotalItems: 100,
            CurrentPage: 1,
            PageSize: 10,
            TotalPages: 10,
            NextCursor: "cursor123");

        // Act
        var result = new PagedResult<string>(items, metadata);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.NotNull(result.Metadata);
        Assert.Equal(100, result.Metadata.TotalItems);
        Assert.Equal(1, result.Metadata.CurrentPage);
        Assert.Equal("cursor123", result.Metadata.NextCursor);
    }

    [Fact]
    public void WhenCreatingPagedResultWithEmptyItemsThenItemsIsEmpty()
    {
        // Arrange
        var items = new List<int>();

        // Act
        var result = new PagedResult<int>(items);

        // Assert
        Assert.Empty(result.Items);
    }
}
