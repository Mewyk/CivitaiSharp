namespace CivitaiSharp.Tools.Hashing;

using System;
using System.Buffers;
using System.IO;
using System.IO.Hashing;
using System.Threading;
using System.Threading.Tasks;
using CivitaiSharp.Core.Response;

/// <summary>
/// Service responsible for computing cryptographic hashes for files and streams.
/// </summary>
public interface IFileHashingService
{
    /// <summary>
    /// Computes the hash of a file at the given path using the specified algorithm.
    /// </summary>
    /// <param name="filePath">The path to the file to hash.</param>
    /// <param name="algorithm">The hash algorithm to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the hash information on success, or an error on failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is empty or whitespace.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    Task<Result<HashedFile>> ComputeHashAsync(
        string filePath,
        HashAlgorithm algorithm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Computes the hash of the data available in the provided stream.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The stream will be read to completion. Callers should assume the stream position
    /// is advanced to the end after this call.
    /// </para>
    /// <para>
    /// For non-seekable streams, the reported file size in the result will be -1.
    /// </para>
    /// </remarks>
    /// <param name="stream">The stream to hash.</param>
    /// <param name="algorithm">The hash algorithm to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the hash information on success, or an error on failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    Task<Result<HashedFile>> ComputeHashAsync(
        Stream stream,
        HashAlgorithm algorithm,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation of <see cref="IFileHashingService"/>.
/// </summary>
/// <remarks>
/// <para>
/// This service supports the following hash algorithms:
/// <list type="bullet">
/// <item><description><see cref="HashAlgorithm.Sha256"/> - SHA-256 (64 hex characters)</description></item>
/// <item><description><see cref="HashAlgorithm.Sha512"/> - SHA-512 (128 hex characters)</description></item>
/// <item><description><see cref="HashAlgorithm.Blake3"/> - BLAKE3 (64 hex characters)</description></item>
/// <item><description><see cref="HashAlgorithm.Crc32"/> - CRC32 (8 hex characters)</description></item>
/// </list>
/// </para>
/// <para>
/// All hash values are returned as lowercase hexadecimal strings.
/// </para>
/// </remarks>
public sealed class FileHashingService : IFileHashingService
{
    private const int BufferSize = 81920; // 80 KB buffer for efficient streaming

    /// <inheritdoc />
    public async Task<Result<HashedFile>> ComputeHashAsync(
        string filePath,
        HashAlgorithm algorithm,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            return new Result<HashedFile>.Failure(
                Error.Create(ErrorCode.FileNotFound, $"File not found: {filePath}"));
        }

        try
        {
            await using var fileStream = File.OpenRead(filePath);
            var result = await ComputeHashAsync(fileStream, algorithm, cancellationToken).ConfigureAwait(false);

            if (result is Result<HashedFile>.Success success)
            {
                // Update with accurate file size and file path
                return new Result<HashedFile>.Success(
                    success.Data with { FilePath = filePath, FileSize = fileStream.Length });
            }

            return result;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return new Result<HashedFile>.Failure(
                Error.Create(ErrorCode.HashComputationFailed, exception.Message, exception));
        }
    }

    /// <inheritdoc />
    public async Task<Result<HashedFile>> ComputeHashAsync(
        Stream stream,
        HashAlgorithm algorithm,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
        {
            return new Result<HashedFile>.Failure(
                Error.Create(ErrorCode.StreamNotReadable, "Stream is not readable."));
        }

        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var startPosition = stream.CanSeek ? stream.Position : 0L;

            byte[] hashBytes = algorithm switch
            {
                HashAlgorithm.Sha256 => await ComputeSha256Async(stream, cancellationToken).ConfigureAwait(false),
                HashAlgorithm.Sha512 => await ComputeSha512Async(stream, cancellationToken).ConfigureAwait(false),
                HashAlgorithm.Blake3 => await ComputeBlake3Async(stream, cancellationToken).ConfigureAwait(false),
                HashAlgorithm.Crc32 => await ComputeCrc32Async(stream, cancellationToken).ConfigureAwait(false),
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Unsupported hash algorithm.")
            };

            stopwatch.Stop();

            var hexHash = Convert.ToHexString(hashBytes).ToLowerInvariant();
            var size = stream.CanSeek ? stream.Length - startPosition : -1L;

            return new Result<HashedFile>.Success(
                new HashedFile(null, hexHash, algorithm, size, stopwatch.Elapsed));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return new Result<HashedFile>.Failure(
                Error.Create(ErrorCode.HashComputationFailed, exception.Message, exception));
        }
    }

    private static async Task<byte[]> ComputeSha256Async(Stream stream, CancellationToken cancellationToken)
    {
        return await System.Security.Cryptography.SHA256.HashDataAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<byte[]> ComputeSha512Async(Stream stream, CancellationToken cancellationToken)
    {
        return await System.Security.Cryptography.SHA512.HashDataAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<byte[]> ComputeBlake3Async(Stream stream, CancellationToken cancellationToken)
    {
        var hasher = Blake3.Hasher.New();
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);

        try
        {
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, BufferSize), cancellationToken).ConfigureAwait(false)) > 0)
            {
                hasher.Update(buffer.AsSpan(0, bytesRead));
            }

            var result = hasher.Finalize();
            return result.AsSpan().ToArray();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private static async Task<byte[]> ComputeCrc32Async(Stream stream, CancellationToken cancellationToken)
    {
        var crc32 = new Crc32();
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);

        try
        {
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, BufferSize), cancellationToken).ConfigureAwait(false)) > 0)
            {
                crc32.Append(buffer.AsSpan(0, bytesRead));
            }

            return crc32.GetCurrentHash();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
