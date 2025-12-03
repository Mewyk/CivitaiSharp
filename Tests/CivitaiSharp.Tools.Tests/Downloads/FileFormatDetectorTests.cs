namespace CivitaiSharp.Tools.Tests.Downloads;

using CivitaiSharp.Tools.Downloads;
using Xunit;

public sealed class FileFormatDetectorTests
{
    [Theory]
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "png")]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46 }, "jpg")]
    [InlineData(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x00, 0x00 }, "gif")]
    [InlineData(new byte[] { 0x1A, 0x45, 0xDF, 0xA3, 0x00, 0x00, 0x00, 0x00 }, "webm")]
    public void WhenDetectingFormatFromMagicBytesThenReturnsCorrectFormat(byte[] header, string expectedFormat)
    {
        // Act
        var result = FileFormatDetector.DetectFormat(header);

        // Assert
        Assert.Equal(expectedFormat, result);
    }

    [Fact]
    public void WhenDetectingWebPFormatThenReturnsWebp()
    {
        // Arrange - RIFF....WEBP
        var header = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50 };

        // Act
        var result = FileFormatDetector.DetectFormat(header);

        // Assert
        Assert.Equal("webp", result);
    }

    [Fact]
    public void WhenDetectingMp4FormatThenReturnsMp4()
    {
        // Arrange - ftyp at offset 4
        var header = new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D };

        // Act
        var result = FileFormatDetector.DetectFormat(header);

        // Assert
        Assert.Equal("mp4", result);
    }

    [Fact]
    public void WhenDetectingAvifFormatThenReturnsAvif()
    {
        // Arrange - ftyp at offset 4 with avif brand
        var header = new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x61, 0x76, 0x69, 0x66 };

        // Act
        var result = FileFormatDetector.DetectFormat(header);

        // Assert
        Assert.Equal("avif", result);
    }

    [Fact]
    public void WhenDetectingHeicFormatThenReturnsHeic()
    {
        // Arrange - ftyp at offset 4 with heic brand
        var header = new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x68, 0x65, 0x69, 0x63 };

        // Act
        var result = FileFormatDetector.DetectFormat(header);

        // Assert
        Assert.Equal("heic", result);
    }

    [Fact]
    public void WhenDetectingUnknownFormatThenReturnsNull()
    {
        // Arrange
        var header = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

        // Act
        var result = FileFormatDetector.DetectFormat(header);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void WhenDetectingFormatFromShortHeaderThenReturnsNull()
    {
        // Arrange
        var header = new byte[] { 0x89, 0x50, 0x4E };

        // Act
        var result = FileFormatDetector.DetectFormat(header);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task WhenDetectingFormatFromStreamThenReturnsCorrectFormat()
    {
        // Arrange - PNG magic bytes
        var header = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        using var stream = new MemoryStream(header);

        // Act
        var result = await FileFormatDetector.DetectFormatAsync(stream);

        // Assert
        Assert.Equal("png", result);
    }

    [Fact]
    public async Task WhenDetectingFormatFromNonReadableStreamThenReturnsNull()
    {
        // Arrange
        using var stream = new MemoryStream();
        stream.Close();

        // Act
        var result = await FileFormatDetector.DetectFormatAsync(stream);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task WhenDetectingFormatFromFileThenReturnsCorrectFormat()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var pngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            await File.WriteAllBytesAsync(tempFile, pngHeader);

            // Act
            var result = await FileFormatDetector.DetectFormatAsync(tempFile);

            // Assert
            Assert.Equal("png", result);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public async Task WhenDetectingFormatFromNonExistentFileThenReturnsNull()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}.bin");

        // Act
        var result = await FileFormatDetector.DetectFormatAsync(nonExistentPath);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task WhenDetectingFormatFromNullStreamThenThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => FileFormatDetector.DetectFormatAsync((Stream)null!));
    }

    [Fact]
    public async Task WhenDetectingFormatFromNullFilePathThenThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => FileFormatDetector.DetectFormatAsync((string)null!));
    }
}
