namespace CivitaiSharp.Tools.Tests.Parsing;

using CivitaiSharp.Tools.Parsing;
using Xunit;

public sealed class HtmlParserTests
{
    [Fact]
    public void WhenConvertingNullHtmlToMarkdownThenReturnsEmptyString()
    {
        // Act
        var result = HtmlParser.ToMarkdown(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void WhenConvertingEmptyHtmlToMarkdownThenReturnsEmptyString()
    {
        // Act
        var result = HtmlParser.ToMarkdown("");

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void WhenConvertingWhitespaceHtmlToMarkdownThenReturnsEmptyString()
    {
        // Act
        var result = HtmlParser.ToMarkdown("   ");

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("<h1>Title</h1>", "# Title")]
    [InlineData("<h2>Subtitle</h2>", "## Subtitle")]
    [InlineData("<h3>Section</h3>", "### Section")]
    [InlineData("<h4>Subsection</h4>", "#### Subsection")]
    [InlineData("<h5>Minor</h5>", "##### Minor")]
    [InlineData("<h6>Smallest</h6>", "###### Smallest")]
    public void WhenConvertingHeadingsToMarkdownThenReturnsCorrectFormat(string html, string expected)
    {
        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains(expected, result);
    }

    [Fact]
    public void WhenConvertingParagraphToMarkdownThenRemovesTagsAndAddsNewlines()
    {
        // Arrange
        const string html = "<p>This is a paragraph.</p>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("This is a paragraph.", result);
    }

    [Fact]
    public void WhenConvertingStrongToMarkdownThenUsesBoldSyntax()
    {
        // Arrange
        const string html = "<strong>bold text</strong>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("**bold text**", result);
    }

    [Fact]
    public void WhenConvertingBoldTagToMarkdownThenUsesBoldSyntax()
    {
        // Arrange
        const string html = "<b>bold text</b>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("**bold text**", result);
    }

    [Fact]
    public void WhenConvertingEmphasisToMarkdownThenUsesItalicSyntax()
    {
        // Arrange
        const string html = "<em>italic text</em>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("*italic text*", result);
    }

    [Fact]
    public void WhenConvertingItalicTagToMarkdownThenUsesItalicSyntax()
    {
        // Arrange
        const string html = "<i>italic text</i>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("*italic text*", result);
    }

    [Fact]
    public void WhenConvertingInlineCodeToMarkdownThenUsesBackticks()
    {
        // Arrange
        const string html = "<code>inline code</code>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("`inline code`", result);
    }

    [Fact]
    public void WhenConvertingPreCodeToMarkdownThenUsesCodeBlock()
    {
        // Arrange
        const string html = "<pre><code>code block</code></pre>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("```\ncode block\n```", result);
    }

    [Fact]
    public void WhenConvertingLinkToMarkdownThenUsesMarkdownLinkSyntax()
    {
        // Arrange
        const string html = "<a href=\"https://example.com\">Example</a>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("[Example](https://example.com)", result);
    }

    [Fact]
    public void WhenConvertingImageToMarkdownThenUsesMarkdownImageSyntax()
    {
        // Arrange
        const string html = "<img src=\"https://example.com/image.png\" alt=\"Example Image\" />";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("![Example Image](https://example.com/image.png)", result);
    }

    [Fact]
    public void WhenConvertingUnorderedListToMarkdownThenUsesDashes()
    {
        // Arrange
        const string html = "<ul><li>Item 1</li><li>Item 2</li></ul>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("- Item 1", result);
        Assert.Contains("- Item 2", result);
    }

    [Fact]
    public void WhenConvertingOrderedListToMarkdownThenUsesNumbers()
    {
        // Arrange
        const string html = "<ol><li>First</li><li>Second</li></ol>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("1. First", result);
        Assert.Contains("2. Second", result);
    }

    [Fact]
    public void WhenConvertingBlockquoteToMarkdownThenUsesGreaterThan()
    {
        // Arrange
        const string html = "<blockquote>Quote text</blockquote>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("> Quote text", result);
    }

    [Fact]
    public void WhenConvertingLineBreakToMarkdownThenAddsNewline()
    {
        // Arrange
        const string html = "Line 1<br/>Line 2";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("Line 1\nLine 2", result);
    }

    [Fact]
    public void WhenConvertingHorizontalRuleToMarkdownThenUsesDashes()
    {
        // Arrange
        const string html = "<hr/>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("---", result);
    }

    [Fact]
    public void WhenConvertingStrikethroughToMarkdownThenUsesTildes()
    {
        // Arrange
        const string html = "<s>deleted</s>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("~~deleted~~", result);
    }

    [Fact]
    public void WhenConvertingDelTagToMarkdownThenUsesTildes()
    {
        // Arrange
        const string html = "<del>removed</del>";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("~~removed~~", result);
    }

    [Fact]
    public void WhenDecodingHtmlEntitiesToMarkdownThenDecodesCorrectly()
    {
        // Arrange
        const string html = "&amp; &lt; &gt; &quot; &#39;";

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("&", result);
        Assert.Contains("<", result);
        Assert.Contains(">", result);
        Assert.Contains("\"", result);
        Assert.Contains("'", result);
    }

    [Fact]
    public void WhenConvertingNullHtmlToPlainTextThenReturnsEmptyString()
    {
        // Act
        var result = HtmlParser.ToPlainText(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void WhenConvertingHtmlToPlainTextThenStripsAllTags()
    {
        // Arrange
        const string html = "<p>Hello <strong>world</strong>!</p>";

        // Act
        var result = HtmlParser.ToPlainText(html);

        // Assert
        Assert.Contains("Hello world!", result);
        Assert.DoesNotContain("<p>", result);
        Assert.DoesNotContain("<strong>", result);
    }

    [Fact]
    public void WhenConvertingLinkToPlainTextThenExtractsLinkText()
    {
        // Arrange
        const string html = "<a href=\"https://example.com\">Click here</a>";

        // Act
        var result = HtmlParser.ToPlainText(html);

        // Assert
        Assert.Contains("Click here", result);
        Assert.DoesNotContain("https://example.com", result);
    }

    [Fact]
    public void WhenConvertingImageToPlainTextThenExtractsAltText()
    {
        // Arrange
        const string html = "<img src=\"https://example.com/img.png\" alt=\"My Image\" />";

        // Act
        var result = HtmlParser.ToPlainText(html);

        // Assert
        Assert.Contains("My Image", result);
        Assert.DoesNotContain("https://example.com", result);
    }

    [Fact]
    public void WhenConvertingListToPlainTextThenUsesSimpleBullets()
    {
        // Arrange
        const string html = "<ul><li>Item 1</li><li>Item 2</li></ul>";

        // Act
        var result = HtmlParser.ToPlainText(html);

        // Assert
        Assert.Contains("Item 1", result);
        Assert.Contains("Item 2", result);
    }

    [Fact]
    public void WhenConvertingComplexHtmlToMarkdownThenHandlesAllElements()
    {
        // Arrange
        const string html = """
            <h1>Title</h1>
            <p>This is a <strong>bold</strong> and <em>italic</em> paragraph.</p>
            <ul>
                <li>Item 1</li>
                <li>Item 2</li>
            </ul>
            <p>Check out <a href="https://example.com">this link</a>.</p>
            """;

        // Act
        var result = HtmlParser.ToMarkdown(html);

        // Assert
        Assert.Contains("# Title", result);
        Assert.Contains("**bold**", result);
        Assert.Contains("*italic*", result);
        Assert.Contains("- Item 1", result);
        Assert.Contains("[this link](https://example.com)", result);
    }
}
