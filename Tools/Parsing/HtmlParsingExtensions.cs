namespace CivitaiSharp.Tools.Parsing;

using CivitaiSharp.Core.Models;

/// <summary>
/// Extension methods for parsing HTML content in Civitai API models.
/// </summary>
public static class HtmlParsingExtensions
{
    /// <summary>
    /// Gets the model description as Markdown.
    /// </summary>
    /// <param name="model">The model containing an HTML description.</param>
    /// <returns>The description converted to Markdown, or an empty string if null.</returns>
    public static string GetDescriptionAsMarkdown(this Model model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return HtmlParser.ToMarkdown(model.Description);
    }

    /// <summary>
    /// Gets the model description as plain text.
    /// </summary>
    /// <param name="model">The model containing an HTML description.</param>
    /// <returns>The description converted to plain text, or an empty string if null.</returns>
    public static string GetDescriptionAsPlainText(this Model model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return HtmlParser.ToPlainText(model.Description);
    }

    /// <summary>
    /// Gets the model version description as Markdown.
    /// </summary>
    /// <param name="modelVersion">The model version containing an HTML description.</param>
    /// <returns>The description converted to Markdown, or an empty string if null.</returns>
    public static string GetDescriptionAsMarkdown(this ModelVersion modelVersion)
    {
        ArgumentNullException.ThrowIfNull(modelVersion);
        return HtmlParser.ToMarkdown(modelVersion.Description);
    }

    /// <summary>
    /// Gets the model version description as plain text.
    /// </summary>
    /// <param name="modelVersion">The model version containing an HTML description.</param>
    /// <returns>The description converted to plain text, or an empty string if null.</returns>
    public static string GetDescriptionAsPlainText(this ModelVersion modelVersion)
    {
        ArgumentNullException.ThrowIfNull(modelVersion);
        return HtmlParser.ToPlainText(modelVersion.Description);
    }
}
