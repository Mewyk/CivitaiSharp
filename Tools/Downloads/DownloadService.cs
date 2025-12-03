namespace CivitaiSharp.Tools.Downloads;

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Models;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Tools.Downloads.Options;
using CivitaiSharp.Tools.Downloads.Patterns;
using CivitaiSharp.Tools.Hashing;
using Microsoft.Extensions.Options;

/// <summary>
/// Service for downloading images and model files from Civitai.
/// </summary>
public interface IDownloadService
{
    /// <summary>
    /// Downloads an image using the configured image download options.
    /// </summary>
    /// <param name="image">The image to download.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the downloaded file information on success.</returns>
    Task<Result<DownloadedFile>> DownloadAsync(
        Image image,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads an image to a specific base directory, using the configured path pattern.
    /// </summary>
    /// <param name="image">The image to download.</param>
    /// <param name="baseDirectory">The base directory to save the image to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the downloaded file information on success.</returns>
    Task<Result<DownloadedFile>> DownloadAsync(
        Image image,
        string baseDirectory,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a model file using the configured model download options.
    /// </summary>
    /// <param name="file">The model file to download.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the downloaded file information on success.</returns>
    Task<Result<DownloadedFile>> DownloadAsync(
        ModelFile file,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a model file to a specific base directory, using the configured path pattern.
    /// </summary>
    /// <param name="file">The model file to download.</param>
    /// <param name="baseDirectory">The base directory to save the file to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the downloaded file information on success.</returns>
    Task<Result<DownloadedFile>> DownloadAsync(
        ModelFile file,
        string baseDirectory,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a model file with version context using the configured model download options.
    /// </summary>
    /// <remarks>
    /// Providing the version context enables additional path pattern tokens such as
    /// {VersionName}, {BaseModel}, {ModelName}, {ModelType}, and {Creator}.
    /// </remarks>
    /// <param name="file">The model file to download.</param>
    /// <param name="version">The model version providing additional context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the downloaded file information on success.</returns>
    Task<Result<DownloadedFile>> DownloadAsync(
        ModelFile file,
        ModelVersion version,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a model file with version context to a specific base directory.
    /// </summary>
    /// <param name="file">The model file to download.</param>
    /// <param name="version">The model version providing additional context.</param>
    /// <param name="baseDirectory">The base directory to save the file to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the downloaded file information on success.</returns>
    Task<Result<DownloadedFile>> DownloadAsync(
        ModelFile file,
        ModelVersion version,
        string baseDirectory,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from a raw URL to a specific destination path.
    /// </summary>
    /// <param name="url">The URL to download from.</param>
    /// <param name="destinationPath">The full path where the file will be saved.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the downloaded file information on success.</returns>
    Task<Result<DownloadedFile>> DownloadAsync(
        string url,
        string destinationPath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from a raw URL with hash verification.
    /// </summary>
    /// <param name="url">The URL to download from.</param>
    /// <param name="destinationPath">The full path where the file will be saved.</param>
    /// <param name="expectedHash">The expected hash value for verification.</param>
    /// <param name="algorithm">The hash algorithm to use for verification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the downloaded file information on success.</returns>
    Task<Result<DownloadedFile>> DownloadAsync(
        string url,
        string destinationPath,
        string expectedHash,
        HashAlgorithm algorithm,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation of <see cref="IDownloadService"/>.
/// </summary>
public sealed class DownloadService : IDownloadService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFileHashingService _hashingService;
    private readonly IOptionsMonitor<ImageDownloadOptions> _imageOptions;
    private readonly IOptionsMonitor<ModelDownloadOptions> _modelOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadService"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="hashingService">The file hashing service.</param>
    /// <param name="imageOptions">The image download options.</param>
    /// <param name="modelOptions">The model download options.</param>
    public DownloadService(
        IHttpClientFactory httpClientFactory,
        IFileHashingService hashingService,
        IOptionsMonitor<ImageDownloadOptions> imageOptions,
        IOptionsMonitor<ModelDownloadOptions> modelOptions)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
        _imageOptions = imageOptions ?? throw new ArgumentNullException(nameof(imageOptions));
        _modelOptions = modelOptions ?? throw new ArgumentNullException(nameof(modelOptions));
    }

    /// <inheritdoc />
    public Task<Result<DownloadedFile>> DownloadAsync(
        Image image,
        CancellationToken cancellationToken = default)
    {
        var options = _imageOptions.CurrentValue;
        return DownloadAsync(image, options.BaseDirectory, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<DownloadedFile>> DownloadAsync(
        Image image,
        string baseDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDirectory);

        if (string.IsNullOrWhiteSpace(image.Url))
        {
            return new Result<DownloadedFile>.Failure(
                Error.Create(ErrorCode.ImageUrlMissing, "Image URL is missing or empty."));
        }

        var options = _imageOptions.CurrentValue;
        var extension = ImagePatternTokens.InferExtension(image);
        var tokenValues = ImagePatternTokens.ExtractTokenValues(image, extension);
        var relativePath = PathPatternProcessor.Process(options.PathPattern, tokenValues);
        var destinationPath = Path.GetFullPath(Path.Combine(baseDirectory, relativePath));

        // Check if file exists
        if (!options.OverwriteExisting && File.Exists(destinationPath))
        {
            return new Result<DownloadedFile>.Failure(
                Error.Create(ErrorCode.IoError, $"File already exists and OverwriteExisting is false: {destinationPath}"));
        }

        return await DownloadAsync(image.Url, destinationPath, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<Result<DownloadedFile>> DownloadAsync(
        ModelFile file,
        CancellationToken cancellationToken = default)
    {
        var options = _modelOptions.CurrentValue;
        return DownloadModelFileAsync(file, null, options.BaseDirectory, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<DownloadedFile>> DownloadAsync(
        ModelFile file,
        string baseDirectory,
        CancellationToken cancellationToken = default)
    {
        return DownloadModelFileAsync(file, null, baseDirectory, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<DownloadedFile>> DownloadAsync(
        ModelFile file,
        ModelVersion version,
        CancellationToken cancellationToken = default)
    {
        var options = _modelOptions.CurrentValue;
        return DownloadModelFileAsync(file, version, options.BaseDirectory, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<DownloadedFile>> DownloadAsync(
        ModelFile file,
        ModelVersion version,
        string baseDirectory,
        CancellationToken cancellationToken = default)
    {
        return DownloadModelFileAsync(file, version, baseDirectory, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<DownloadedFile>> DownloadAsync(
        string url,
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return new Result<DownloadedFile>.Failure(
                Error.Create(ErrorCode.InvalidUrl, "URL must be an absolute HTTP or HTTPS URI."));
        }

        // Ensure directory exists
        var directoryResult = EnsureDirectory(destinationPath);
        if (directoryResult.IsFailure)
            return new Result<DownloadedFile>.Failure(directoryResult.ErrorInfo);

        var client = _httpClientFactory.CreateClient(HttpClientNames.CivitaiPublicApi);

        return await DownloadToFileAsync(client, url, destinationPath, null, null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Result<DownloadedFile>> DownloadAsync(
        string url,
        string destinationPath,
        string expectedHash,
        HashAlgorithm algorithm,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedHash);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return new Result<DownloadedFile>.Failure(
                Error.Create(ErrorCode.InvalidUrl, "URL must be an absolute HTTP or HTTPS URI."));
        }

        // Ensure directory exists
        var directoryResult = EnsureDirectory(destinationPath);
        if (directoryResult.IsFailure)
            return new Result<DownloadedFile>.Failure(directoryResult.ErrorInfo);

        var client = _httpClientFactory.CreateClient(HttpClientNames.CivitaiPublicApi);

        return await DownloadToFileAsync(client, url, destinationPath, expectedHash, algorithm, cancellationToken).ConfigureAwait(false);
    }

    private async Task<Result<DownloadedFile>> DownloadModelFileAsync(
        ModelFile file,
        ModelVersion? version,
        string baseDirectory,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDirectory);

        if (string.IsNullOrWhiteSpace(file.DownloadUrl))
        {
            return new Result<DownloadedFile>.Failure(
                Error.Create(ErrorCode.DownloadUrlMissing, "Model file download URL is missing or empty."));
        }

        var options = _modelOptions.CurrentValue;

        var tokenValues = version is not null
            ? ModelPatternTokens.ExtractTokenValues(file, version)
            : ModelPatternTokens.ExtractTokenValues(file);

        var relativePath = PathPatternProcessor.Process(options.PathPattern, tokenValues);
        var destinationPath = Path.GetFullPath(Path.Combine(baseDirectory, relativePath));

        // Check if file exists
        if (!options.OverwriteExisting && File.Exists(destinationPath))
        {
            return new Result<DownloadedFile>.Failure(
                Error.Create(ErrorCode.IoError, $"File already exists and OverwriteExisting is false: {destinationPath}"));
        }

        // Determine hash verification parameters
        string? expectedHash = null;
        HashAlgorithm? algorithm = null;

        if (options.VerifyHash && file.Hashes is not null)
        {
            (expectedHash, algorithm) = GetExpectedHash(file.Hashes, options.HashAlgorithm);
        }

        // Ensure directory exists
        var directoryResult = EnsureDirectory(destinationPath);
        if (directoryResult.IsFailure)
            return new Result<DownloadedFile>.Failure(directoryResult.ErrorInfo);

        var client = _httpClientFactory.CreateClient(HttpClientNames.CivitaiPublicApi);

        return await DownloadToFileAsync(client, file.DownloadUrl, destinationPath, expectedHash, algorithm, cancellationToken).ConfigureAwait(false);
    }

    private async Task<Result<DownloadedFile>> DownloadToFileAsync(
        HttpClient client,
        string url,
        string destinationPath,
        string? expectedHash,
        HashAlgorithm? algorithm,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException exception)
        {
            return new Result<DownloadedFile>.Failure(
                Error.Create(ErrorCode.HttpError, $"HTTP request failed: {exception.Message}", exception));
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (TaskCanceledException exception)
        {
            return new Result<DownloadedFile>.Failure(
                Error.Create(ErrorCode.Timeout, "The download request timed out.", exception));
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return new Result<DownloadedFile>.Failure(
                    Error.Create(ErrorCode.HttpError, $"Failed to download: HTTP {(int)response.StatusCode} {response.ReasonPhrase}"));
            }

            // Download the content
            try
            {
                await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                await using (var fileStream = File.Open(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await contentStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
                    await fileStream.FlushAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (IOException exception)
            {
                return new Result<DownloadedFile>.Failure(
                    Error.Create(ErrorCode.FileWriteFailed, $"Failed to write file: {exception.Message}", exception));
            }
        }

        // Get file size
        var fileInfo = new FileInfo(destinationPath);
        var fileSize = fileInfo.Length;

        // Hash verification if requested
        if (!string.IsNullOrWhiteSpace(expectedHash) && algorithm.HasValue)
        {
            var hashResult = await _hashingService.ComputeHashAsync(destinationPath, algorithm.Value, cancellationToken).ConfigureAwait(false);

            if (hashResult.IsFailure)
            {
                // Clean up the downloaded file on hash computation failure
                TryDeleteFile(destinationPath);

                return new Result<DownloadedFile>.Failure(
                    Error.Create(ErrorCode.HashComputationFailed, $"Failed to compute file hash: {hashResult.ErrorInfo.Message}"));
            }

            var computedHash = hashResult.Value.Hash;
            var isVerified = string.Equals(computedHash, expectedHash, StringComparison.OrdinalIgnoreCase);

            if (!isVerified)
            {
                // Clean up the downloaded file on hash mismatch
                TryDeleteFile(destinationPath);

                return new Result<DownloadedFile>.Failure(
                    Error.Create(ErrorCode.HashVerificationFailed,
                        $"Hash verification failed. Expected: {expectedHash}, Computed: {computedHash}"));
            }

            return new Result<DownloadedFile>.Success(
                new DownloadedFile(destinationPath, fileSize, IsVerified: true, computedHash));
        }

        return new Result<DownloadedFile>.Success(
            new DownloadedFile(destinationPath, fileSize, IsVerified: false, ComputedHash: null));
    }

    private static (string? Hash, HashAlgorithm? Algorithm) GetExpectedHash(Hashes hashes, HashAlgorithm preferredAlgorithm)
    {
        // Try preferred algorithm first
        var hash = preferredAlgorithm switch
        {
            HashAlgorithm.Sha256 => hashes.Sha256,
            HashAlgorithm.Blake3 => hashes.Blake3,
            HashAlgorithm.Crc32 => hashes.Crc32,
            _ => null
        };

        if (!string.IsNullOrWhiteSpace(hash))
            return (hash, preferredAlgorithm);

        // Fall back to any available hash
        if (!string.IsNullOrWhiteSpace(hashes.Sha256))
            return (hashes.Sha256, HashAlgorithm.Sha256);

        if (!string.IsNullOrWhiteSpace(hashes.Blake3))
            return (hashes.Blake3, HashAlgorithm.Blake3);

        if (!string.IsNullOrWhiteSpace(hashes.Crc32))
            return (hashes.Crc32, HashAlgorithm.Crc32);

        return (null, null);
    }

    private static Result<Unit> EnsureDirectory(string destinationPath)
    {
        var directory = Path.GetDirectoryName(destinationPath);
        if (string.IsNullOrEmpty(directory))
            return new Result<Unit>.Success(Unit.Value);

        try
        {
            Directory.CreateDirectory(directory);
            return new Result<Unit>.Success(Unit.Value);
        }
        catch (Exception exception)
        {
            return new Result<Unit>.Failure(
                Error.Create(ErrorCode.DirectoryCreationFailed, $"Failed to create directory '{directory}': {exception.Message}", exception));
        }
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Ignore deletion errors during cleanup
        }
    }
}
