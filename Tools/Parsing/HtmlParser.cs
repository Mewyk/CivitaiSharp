namespace CivitaiSharp.Tools.Parsing;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Provides HTML parsing capabilities to convert HTML content to Markdown or plain text.
/// This parser handles common HTML elements found in Civitai model descriptions.
/// </summary>
public static partial class HtmlParser
{
    /// <summary>
    /// Converts HTML content to Markdown format.
    /// </summary>
    /// <param name="html">The HTML string to convert.</param>
    /// <returns>A Markdown representation of the HTML content.</returns>
    public static string ToMarkdown(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var result = html;

        // Normalize line endings
        result = result.Replace("\r\n", "\n").Replace("\r", "\n");

        // Process block elements first (order matters)
        result = ConvertHeadings(result);
        result = ConvertLists(result);
        result = ConvertBlockquotes(result);
        result = ConvertCodeBlocks(result);
        result = ConvertParagraphs(result);
        result = ConvertLineBreaks(result);
        result = ConvertHorizontalRules(result);

        // Process inline elements
        result = ConvertLinks(result);
        result = ConvertImages(result);
        result = ConvertBoldAndItalic(result);
        result = ConvertInlineCode(result);
        result = ConvertStrikethrough(result);

        // Clean up remaining HTML
        result = StripRemainingTags(result);
        result = DecodeHtmlEntities(result);

        // Normalize whitespace
        result = NormalizeWhitespace(result);

        return result.Trim();
    }

    /// <summary>
    /// Converts HTML content to plain text, stripping all formatting.
    /// </summary>
    /// <param name="html">The HTML string to convert.</param>
    /// <returns>A plain text representation of the HTML content.</returns>
    public static string ToPlainText(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var result = html;

        // Normalize line endings
        result = result.Replace("\r\n", "\n").Replace("\r", "\n");

        // Convert block elements to appropriate line breaks
        result = BlockToNewlines(result);

        // Extract link text (discard URLs)
        result = ExtractLinkText(result);

        // Extract image alt text
        result = ExtractImageAlt(result);

        // Handle lists - add simple bullets/numbers
        result = SimplifyLists(result);

        // Strip all remaining HTML tags
        result = StripRemainingTags(result);

        // Decode HTML entities
        result = DecodeHtmlEntities(result);

        // Normalize whitespace for plain text
        result = NormalizePlainTextWhitespace(result);

        return result.Trim();
    }

    #region Markdown Conversion Helpers

    private static string ConvertHeadings(string html)
    {
        // h1-h6
        html = Heading1Regex().Replace(html, match => $"# {match.Groups[1].Value.Trim()}\n\n");
        html = Heading2Regex().Replace(html, match => $"## {match.Groups[1].Value.Trim()}\n\n");
        html = Heading3Regex().Replace(html, match => $"### {match.Groups[1].Value.Trim()}\n\n");
        html = Heading4Regex().Replace(html, match => $"#### {match.Groups[1].Value.Trim()}\n\n");
        html = Heading5Regex().Replace(html, match => $"##### {match.Groups[1].Value.Trim()}\n\n");
        html = Heading6Regex().Replace(html, match => $"###### {match.Groups[1].Value.Trim()}\n\n");
        return html;
    }

    private static string ConvertLists(string html)
    {
        // Unordered lists
        html = UnorderedListRegex().Replace(html, match =>
        {
            var items = ListItemRegex().Matches(match.Groups[1].Value);
            var stringBuilder = new StringBuilder();
            foreach (Match item in items)
            {
                stringBuilder.AppendLine($"- {item.Groups[1].Value.Trim()}");
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        });

        // Ordered lists
        html = OrderedListRegex().Replace(html, match =>
        {
            var items = ListItemRegex().Matches(match.Groups[1].Value);
            var stringBuilder = new StringBuilder();
            var index = 1;
            foreach (Match item in items)
            {
                stringBuilder.AppendLine($"{index}. {item.Groups[1].Value.Trim()}");
                index++;
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        });

        return html;
    }

    private static string ConvertBlockquotes(string html)
    {
        return BlockquoteRegex().Replace(html, match =>
        {
            var content = match.Groups[1].Value.Trim();
            var lines = content.Split('\n');
            var stringBuilder = new StringBuilder();
            foreach (var line in lines)
            {
                stringBuilder.AppendLine($"> {line.Trim()}");
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        });
    }

    private static string ConvertCodeBlocks(string html)
    {
        // Pre/code blocks
        html = PreCodeRegex().Replace(html, match => $"```\n{match.Groups[1].Value.Trim()}\n```\n\n");
        html = PreRegex().Replace(html, match => $"```\n{match.Groups[1].Value.Trim()}\n```\n\n");
        return html;
    }

    private static string ConvertParagraphs(string html)
    {
        return ParagraphRegex().Replace(html, match => $"{match.Groups[1].Value.Trim()}\n\n");
    }

    private static string ConvertLineBreaks(string html)
    {
        html = BreakRegex().Replace(html, "\n");
        return html;
    }

    private static string ConvertHorizontalRules(string html)
    {
        return HorizontalRuleRegex().Replace(html, "\n---\n\n");
    }

    private static string ConvertLinks(string html)
    {
        return AnchorRegex().Replace(html, match =>
        {
            var href = match.Groups[1].Value;
            var text = match.Groups[2].Value.Trim();
            if (string.IsNullOrWhiteSpace(text))
                text = href;
            return $"[{text}]({href})";
        });
    }

    private static string ConvertImages(string html)
    {
        // First try to match images with alt text (in either order)
        html = ImageWithAltRegex().Replace(html, match =>
        {
            var src = match.Groups["src"].Value;
            var alt = match.Groups["alt"].Value;
            if (string.IsNullOrWhiteSpace(alt))
                alt = "image";
            return $"![{alt}]({src})";
        });

        // Then match any remaining images without alt text
        html = ImageSrcOnlyRegex().Replace(html, match =>
        {
            var src = match.Groups["src"].Value;
            return $"![image]({src})";
        });

        return html;
    }

    private static string ConvertBoldAndItalic(string html)
    {
        // Strong/bold
        html = StrongRegex().Replace(html, "**$1**");
        html = BoldRegex().Replace(html, "**$1**");

        // Emphasis/italic
        html = EmphasisRegex().Replace(html, "*$1*");
        html = ItalicRegex().Replace(html, "*$1*");

        return html;
    }

    private static string ConvertInlineCode(string html)
    {
        return InlineCodeRegex().Replace(html, "`$1`");
    }

    private static string ConvertStrikethrough(string html)
    {
        html = StrikeRegex().Replace(html, "~~$1~~");
        html = DeleteRegex().Replace(html, "~~$1~~");
        return html;
    }

    #endregion

    #region Plain Text Conversion Helpers

    private static string BlockToNewlines(string html)
    {
        // Block elements get double newlines
        html = DivRegex().Replace(html, "$1\n\n");
        html = ParagraphRegex().Replace(html, "$1\n\n");
        html = HeadingAnyRegex().Replace(html, "$1\n\n");
        html = BlockquoteRegex().Replace(html, "$1\n\n");
        html = PreRegex().Replace(html, "$1\n\n");
        html = PreCodeRegex().Replace(html, "$1\n\n");

        // Single newlines for br, hr
        html = BreakRegex().Replace(html, "\n");
        html = HorizontalRuleRegex().Replace(html, "\n");

        return html;
    }

    private static string ExtractLinkText(string html)
    {
        return AnchorRegex().Replace(html, "$2");
    }

    private static string ExtractImageAlt(string html)
    {
        // First try to match images with alt text
        html = ImageWithAltRegex().Replace(html, match =>
        {
            var alt = match.Groups["alt"].Value;
            return string.IsNullOrWhiteSpace(alt) ? string.Empty : $"[{alt}]";
        });

        // Remove any remaining images without alt text
        html = ImageSrcOnlyRegex().Replace(html, string.Empty);

        return html;
    }

    private static string SimplifyLists(string html)
    {
        // Unordered lists
        html = UnorderedListRegex().Replace(html, match =>
        {
            var items = ListItemRegex().Matches(match.Groups[1].Value);
            var stringBuilder = new StringBuilder();
            foreach (Match item in items)
            {
                stringBuilder.AppendLine($"* {item.Groups[1].Value.Trim()}");
            }
            return stringBuilder.ToString();
        });

        // Ordered lists
        html = OrderedListRegex().Replace(html, match =>
        {
            var items = ListItemRegex().Matches(match.Groups[1].Value);
            var stringBuilder = new StringBuilder();
            var index = 1;
            foreach (Match item in items)
            {
                stringBuilder.AppendLine($"{index}. {item.Groups[1].Value.Trim()}");
                index++;
            }
            return stringBuilder.ToString();
        });

        return html;
    }

    private static string NormalizePlainTextWhitespace(string text)
    {
        // Collapse multiple spaces into one
        text = MultipleSpacesRegex().Replace(text, " ");

        // Collapse multiple newlines into at most two
        text = ExcessiveNewlinesRegex().Replace(text, "\n\n");

        // Trim each line
        var lines = text.Split('\n');
        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            lines[lineIndex] = lines[lineIndex].Trim();
        }
        return string.Join("\n", lines);
    }

    #endregion

    #region Common Helpers

    /// <summary>
    /// Common HTML entities and their replacements.
    /// </summary>
    private static readonly FrozenDictionary<string, string> HtmlEntities = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "&nbsp;", " " },
        { "&amp;", "&" },
        { "&lt;", "<" },
        { "&gt;", ">" },
        { "&quot;", "\"" },
        { "&apos;", "'" },
        { "&#39;", "'" },
        { "&mdash;", "-" },
        { "&ndash;", "-" },
        { "&ldquo;", "\"" },
        { "&rdquo;", "\"" },
        { "&lsquo;", "'" },
        { "&rsquo;", "'" },
        { "&hellip;", "..." },
        { "&copy;", "(c)" },
        { "&reg;", "(R)" },
        { "&trade;", "(TM)" },
        { "&bull;", "*" },
        { "&middot;", "*" }
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private static string StripRemainingTags(string html)
    {
        return HtmlTagRegex().Replace(html, string.Empty);
    }

    private static string DecodeHtmlEntities(string html)
    {
        foreach (var (entity, replacement) in HtmlEntities)
        {
            html = html.Replace(entity, replacement, StringComparison.OrdinalIgnoreCase);
        }

        // Numeric entities (decimal)
        html = DecimalEntityRegex().Replace(html, match =>
        {
            if (int.TryParse(match.Groups[1].Value, out var code) && IsValidUnicodeCodePoint(code))
            {
                return char.ConvertFromUtf32(code);
            }
            return match.Value;
        });

        // Numeric entities (hex)
        html = HexEntityRegex().Replace(html, match =>
        {
            if (int.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var code) && IsValidUnicodeCodePoint(code))
            {
                return char.ConvertFromUtf32(code);
            }
            return match.Value;
        });

        return html;
    }

    /// <summary>
    /// Checks if the code point is a valid Unicode code point that can be converted to a string.
    /// </summary>
    private static bool IsValidUnicodeCodePoint(int codePoint) =>
        codePoint is >= 0 and <= 0x10FFFF and not (>= 0xD800 and <= 0xDFFF);

    private static string NormalizeWhitespace(string text)
    {
        // Collapse multiple spaces into one
        text = MultipleSpacesRegex().Replace(text, " ");

        // Collapse more than 2 consecutive newlines into 2
        text = ExcessiveNewlinesRegex().Replace(text, "\n\n");

        return text;
    }

    #endregion

    #region Generated Regexes

    [GeneratedRegex(@"<h1[^>]*>(.*?)</h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex Heading1Regex();

    [GeneratedRegex(@"<h2[^>]*>(.*?)</h2>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex Heading2Regex();

    [GeneratedRegex(@"<h3[^>]*>(.*?)</h3>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex Heading3Regex();

    [GeneratedRegex(@"<h4[^>]*>(.*?)</h4>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex Heading4Regex();

    [GeneratedRegex(@"<h5[^>]*>(.*?)</h5>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex Heading5Regex();

    [GeneratedRegex(@"<h6[^>]*>(.*?)</h6>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex Heading6Regex();

    [GeneratedRegex(@"<h[1-6][^>]*>(.*?)</h[1-6]>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex HeadingAnyRegex();

    [GeneratedRegex(@"<ul[^>]*>(.*?)</ul>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex UnorderedListRegex();

    [GeneratedRegex(@"<ol[^>]*>(.*?)</ol>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex OrderedListRegex();

    [GeneratedRegex(@"<li[^>]*>(.*?)</li>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ListItemRegex();

    [GeneratedRegex(@"<blockquote[^>]*>(.*?)</blockquote>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex BlockquoteRegex();

    [GeneratedRegex(@"<pre[^>]*><code[^>]*>(.*?)</code></pre>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex PreCodeRegex();

    [GeneratedRegex(@"<pre[^>]*>(.*?)</pre>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex PreRegex();

    [GeneratedRegex(@"<p[^>]*>(.*?)</p>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ParagraphRegex();

    [GeneratedRegex(@"<div[^>]*>(.*?)</div>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex DivRegex();

    [GeneratedRegex(@"<br\s*/?>", RegexOptions.IgnoreCase)]
    private static partial Regex BreakRegex();

    [GeneratedRegex(@"<hr\s*/?>", RegexOptions.IgnoreCase)]
    private static partial Regex HorizontalRuleRegex();

    [GeneratedRegex(@"<a[^>]*href=[""']([^""']*)[""'][^>]*>(.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex AnchorRegex();

    [GeneratedRegex(@"<img[^>]*\bsrc=[""'](?<src>[^""']*)[""'][^>]*>", RegexOptions.IgnoreCase)]
    private static partial Regex ImageSrcOnlyRegex();

    [GeneratedRegex(@"<img[^>]*\balt=[""'](?<alt>[^""']*)[""'][^>]*\bsrc=[""'](?<src>[^""']*)[""'][^>]*>|<img[^>]*\bsrc=[""'](?<src>[^""']*)[""'][^>]*\balt=[""'](?<alt>[^""']*)[""'][^>]*>", RegexOptions.IgnoreCase)]
    private static partial Regex ImageWithAltRegex();

    [GeneratedRegex(@"<strong[^>]*>(.*?)</strong>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex StrongRegex();

    [GeneratedRegex(@"<b[^>]*>(.*?)</b>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex BoldRegex();

    [GeneratedRegex(@"<em[^>]*>(.*?)</em>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex EmphasisRegex();

    [GeneratedRegex(@"<i[^>]*>(.*?)</i>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex ItalicRegex();

    [GeneratedRegex(@"<code[^>]*>(.*?)</code>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex InlineCodeRegex();

    [GeneratedRegex(@"<s[^>]*>(.*?)</s>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex StrikeRegex();

    [GeneratedRegex(@"<del[^>]*>(.*?)</del>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex DeleteRegex();

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex(@"&#(\d+);")]
    private static partial Regex DecimalEntityRegex();

    [GeneratedRegex(@"&#x([0-9a-fA-F]+);")]
    private static partial Regex HexEntityRegex();

    [GeneratedRegex(@"[ \t]+")]
    private static partial Regex MultipleSpacesRegex();

    [GeneratedRegex(@"\n{3,}")]
    private static partial Regex ExcessiveNewlinesRegex();

    #endregion
}
