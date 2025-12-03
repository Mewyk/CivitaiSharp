namespace CivitaiSharp.Tools.Downloads.Options;

using System.IO;

/// <summary>
/// Configuration options for image download operations.
/// </summary>
/// <remarks>
/// <para>
/// Bind this class from configuration using the section path "CivitaiDownloads:Images".
/// </para>
/// <para>
/// Path patterns support the following tokens:
/// <list type="table">
/// <listheader>
/// <term>Token</term>
/// <description>Description</description>
/// </listheader>
/// <item><term>{Id}</term><description>The unique image identifier (required)</description></item>
/// <item><term>{PostId}</term><description>The post identifier containing the image</description></item>
/// <item><term>{Username}</term><description>The creator's username</description></item>
/// <item><term>{Width}</term><description>Image width in pixels</description></item>
/// <item><term>{Height}</term><description>Image height in pixels</description></item>
/// <item><term>{BaseModel}</term><description>The base model used for generation</description></item>
/// <item><term>{NsfwLevel}</term><description>The NSFW content level</description></item>
/// <item><term>{Date}</term><description>The creation date in yyyy-MM-dd format</description></item>
/// <item><term>{Extension}</term><description>The file extension (inferred from URL or media type)</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class ImageDownloadOptions
{
    /// <summary>
    /// Gets or sets the base directory where downloaded images will be saved.
    /// </summary>
    /// <remarks>
    /// Defaults to the system temporary directory. The directory will be created if it does not exist.
    /// </remarks>
    public string BaseDirectory { get; set; } = Path.GetTempPath();

    /// <summary>
    /// Gets or sets the path pattern for generating file names.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The pattern supports tokens that are replaced with values from the image metadata.
    /// Tokens are case-sensitive and must be enclosed in curly braces.
    /// </para>
    /// <para>
    /// Examples:
    /// <list type="bullet">
    /// <item><description>"{Id}.{Extension}" produces "12345678.png"</description></item>
    /// <item><description>"{Username}/{Id}.{Extension}" produces "ArtistName/12345678.png"</description></item>
    /// <item><description>"{BaseModel}/{Username}/{Id}.{Extension}" produces "SDXL 1.0/ArtistName/12345678.png"</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public string PathPattern { get; set; } = "{Id}.{Extension}";

    /// <summary>
    /// Gets or sets whether existing files should be overwritten.
    /// </summary>
    /// <remarks>
    /// When false (default), downloads will fail if the destination file already exists.
    /// When true, existing files will be replaced.
    /// </remarks>
    public bool OverwriteExisting { get; set; }
}
