namespace CivitaiSharp.Tools.Downloads;

using System;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides utility methods for detecting file formats based on magic bytes.
/// </summary>
/// <remarks>
/// <para>
/// This detector identifies common image and video formats used by Civitai by examining
/// the magic bytes at the beginning of files. It does not rely on file extensions.
/// </para>
/// <para>
/// Supported formats:
/// <list type="bullet">
/// <item><description>PNG - Portable Network Graphics</description></item>
/// <item><description>JPEG - Joint Photographic Experts Group</description></item>
/// <item><description>GIF - Graphics Interchange Format</description></item>
/// <item><description>WebP - Modern image format by Google</description></item>
/// <item><description>AVIF - AV1 Image File Format</description></item>
/// <item><description>MP4 - MPEG-4 Part 14 video container</description></item>
/// <item><description>WebM - Open media container format</description></item>
/// </list>
/// </para>
/// </remarks>
public static class FileFormatDetector
{
    // Magic byte patterns for common file formats
    private static ReadOnlySpan<byte> PngMagic => [0x89, 0x50, 0x4E, 0x47];
    private static ReadOnlySpan<byte> JpegMagic => [0xFF, 0xD8, 0xFF];
    private static ReadOnlySpan<byte> GifMagic => [0x47, 0x49, 0x46];
    private static ReadOnlySpan<byte> FtypMagic => [(byte)'f', (byte)'t', (byte)'y', (byte)'p'];
    private static ReadOnlySpan<byte> WebmMagic => [0x1A, 0x45, 0xDF, 0xA3];
    private static ReadOnlySpan<byte> RiffMagic => [(byte)'R', (byte)'I', (byte)'F', (byte)'F'];
    private static ReadOnlySpan<byte> WebpMagic => [(byte)'W', (byte)'E', (byte)'B', (byte)'P'];
    private static ReadOnlySpan<byte> AvifBrand => [(byte)'a', (byte)'v', (byte)'i', (byte)'f'];
    private static ReadOnlySpan<byte> HeicBrand => [(byte)'h', (byte)'e', (byte)'i', (byte)'c'];
    private static ReadOnlySpan<byte> MifBrand => [(byte)'m', (byte)'i', (byte)'f', (byte)'1'];

    /// <summary>
    /// Detects the file format based on magic bytes at the start of the file.
    /// </summary>
    /// <param name="filePath">Path to the file to analyze.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detected format extension (e.g., "png", "jpg"), or null if unknown.</returns>
    public static async Task<string?> DetectFormatAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        try
        {
            await using var stream = File.OpenRead(filePath);
            return await DetectFormatAsync(stream, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException)
        {
            // File access errors are expected in some scenarios
            return null;
        }
    }

    /// <summary>
    /// Detects the file format based on magic bytes from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from (must be readable).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detected format extension (e.g., "png", "jpg"), or null if unknown.</returns>
    public static async Task<string?> DetectFormatAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
            return null;

        try
        {
            var buffer = new byte[16];
            var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);

            if (bytesRead < 4)
                return null;

            return DetectFormat(buffer.AsSpan(0, bytesRead));
        }
        catch (IOException)
        {
            // Stream read errors are expected in some scenarios
            return null;
        }
    }

    /// <summary>
    /// Detects the file format from a span of bytes containing the file header.
    /// </summary>
    /// <param name="header">The file header bytes to analyze (minimum 4 bytes recommended).</param>
    /// <returns>The detected format extension, or null if unknown.</returns>
    public static string? DetectFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length < 4)
            return null;

        // PNG: 89 50 4E 47
        if (header[..4].SequenceEqual(PngMagic))
            return "png";

        // JPEG: FF D8 FF
        if (header[..3].SequenceEqual(JpegMagic))
            return "jpg";

        // GIF: 47 49 46 ("GIF")
        if (header[..3].SequenceEqual(GifMagic))
            return "gif";

        // WebP: RIFF....WEBP
        if (header.Length >= 12 &&
            header[..4].SequenceEqual(RiffMagic) &&
            header.Slice(8, 4).SequenceEqual(WebpMagic))
            return "webp";

        // ISO Base Media File Format (MP4, AVIF, etc.): ftyp at offset 4
        if (header.Length >= 12 && header.Slice(4, 4).SequenceEqual(FtypMagic))
        {
            // Check brand at offset 8
            var brand = header.Slice(8, 4);

            // AVIF
            if (brand.SequenceEqual(AvifBrand) || brand.SequenceEqual(MifBrand))
                return "avif";

            // HEIC (similar structure)
            if (brand.SequenceEqual(HeicBrand))
                return "heic";

            // Default to MP4 for other ftyp containers
            return "mp4";
        }

        // WebM: EBML header 1A 45 DF A3
        if (header[..4].SequenceEqual(WebmMagic))
            return "webm";

        return null;
    }
}
