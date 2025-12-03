namespace CivitaiSharp.Tools.Downloads.Options;

using System.IO;
using CivitaiSharp.Tools.Hashing;

/// <summary>
/// Configuration options for model file download operations.
/// </summary>
/// <remarks>
/// <para>
/// Bind this class from configuration using the section path "CivitaiDownloads:Models".
/// </para>
/// <para>
/// Path patterns support the following tokens when downloading a <see cref="CivitaiSharp.Core.Models.ModelFile"/>:
/// <list type="table">
/// <listheader>
/// <term>Token</term>
/// <description>Description</description>
/// </listheader>
/// <item><term>{FileId}</term><description>The unique file identifier</description></item>
/// <item><term>{FileName}</term><description>The original file name (required)</description></item>
/// <item><term>{FileType}</term><description>The file type (e.g., "Model", "Training Data")</description></item>
/// <item><term>{Format}</term><description>The model format (e.g., "SafeTensor")</description></item>
/// <item><term>{Size}</term><description>The model size specification (e.g., "full", "pruned")</description></item>
/// <item><term>{Precision}</term><description>The floating point precision (e.g., "fp16", "fp32")</description></item>
/// </list>
/// </para>
/// <para>
/// When a <see cref="CivitaiSharp.Core.Models.ModelVersion"/> is provided, additional tokens are available:
/// <list type="table">
/// <listheader>
/// <term>Token</term>
/// <description>Description</description>
/// </listheader>
/// <item><term>{VersionId}</term><description>The model version identifier</description></item>
/// <item><term>{VersionName}</term><description>The version name</description></item>
/// <item><term>{BaseModel}</term><description>The base model (e.g., "SDXL 1.0")</description></item>
/// <item><term>{ModelId}</term><description>The parent model identifier</description></item>
/// <item><term>{ModelName}</term><description>The parent model name</description></item>
/// <item><term>{ModelType}</term><description>The model type (e.g., "Checkpoint", "LORA")</description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class ModelDownloadOptions
{
    /// <summary>
    /// Gets or sets the base directory where downloaded model files will be saved.
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
    /// The pattern supports tokens that are replaced with values from the model file metadata.
    /// Tokens are case-sensitive and must be enclosed in curly braces.
    /// </para>
    /// <para>
    /// Examples:
    /// <list type="bullet">
    /// <item><description>"{FileName}" produces "myModel_v1.safetensors"</description></item>
    /// <item><description>"{ModelType}/{FileName}" produces "Checkpoint/myModel_v1.safetensors"</description></item>
    /// <item><description>"{Creator}/{ModelName}/{VersionName}/{FileName}" produces "CreatorName/Amazing Model/v1.0/myModel_v1.safetensors"</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public string PathPattern { get; set; } = "{FileName}";

    /// <summary>
    /// Gets or sets whether existing files should be overwritten.
    /// </summary>
    /// <remarks>
    /// When true (default), existing files will be replaced.
    /// When false, downloads will fail if the destination file already exists.
    /// </remarks>
    public bool OverwriteExisting { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to verify the downloaded file's hash against the expected hash from Civitai.
    /// </summary>
    /// <remarks>
    /// When true (default), the downloaded file's hash will be computed and compared against
    /// the hash provided by Civitai in the model file metadata. If the hashes do not match,
    /// the download is considered failed and the file may be deleted.
    /// </remarks>
    public bool VerifyHash { get; set; } = true;

    /// <summary>
    /// Gets or sets the hash algorithm to use for verification.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="HashAlgorithm.Sha256"/> which is the most commonly available
    /// hash in Civitai model metadata. BLAKE3 is also available for many models.
    /// </remarks>
    public HashAlgorithm HashAlgorithm { get; set; } = HashAlgorithm.Sha256;
}
