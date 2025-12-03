namespace CivitaiSharp.Tools.Downloads.Options;

/// <summary>
/// Root configuration options container for Civitai download operations.
/// </summary>
/// <remarks>
/// <para>
/// Bind this class from configuration using the section path "CivitaiDownloads".
/// </para>
/// <para>
/// Example appsettings.json:
/// <code>
/// {
///   "CivitaiDownloads": {
///     "Images": {
///       "BaseDirectory": "C:\\Downloads\\Images",
///       "PathPattern": "{BaseModel}/{Username}/{Id}.{Extension}",
///       "OverwriteExisting": false
///     },
///     "Models": {
///       "BaseDirectory": "C:\\Models",
///       "PathPattern": "{ModelType}/{BaseModel}/{Creator}/{ModelName}/{VersionName}/{FileName}",
///       "OverwriteExisting": true,
///       "VerifyHash": true,
///       "HashAlgorithm": "Sha256"
///     }
///   }
/// }
/// </code>
/// </para>
/// </remarks>
public sealed class DownloadOptions
{
    /// <summary>
    /// Gets or sets the configuration section name for binding.
    /// </summary>
    public const string SectionName = "CivitaiDownloads";

    /// <summary>
    /// Gets or sets the image download options.
    /// </summary>
    public ImageDownloadOptions Images { get; set; } = new();

    /// <summary>
    /// Gets or sets the model download options.
    /// </summary>
    public ModelDownloadOptions Models { get; set; } = new();
}
