namespace CivitaiSharp.Tools.Tests.Downloads;

using System.Text;
using CivitaiSharp.Core.Response;
using CivitaiSharp.Tools.Hashing;
using Xunit;

public sealed class FileHashingServiceTests
{
    private readonly FileHashingService _service = new();

    [Fact]
    public async Task WhenComputingBlake3HashFromStreamThenReturnsValidHash()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Hello, World!");
        using var stream = new MemoryStream(data);

        // Act
        var result = await _service.ComputeHashAsync(stream, HashAlgorithm.Blake3);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.Hash);
        Assert.Equal(HashAlgorithm.Blake3, result.Value.Algorithm);
        Assert.Equal(data.Length, result.Value.FileSize);
    }

    [Fact]
    public async Task WhenComputingSha256HashFromStreamThenReturnsValidHash()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Hello, World!");
        using var stream = new MemoryStream(data);

        // Act
        var result = await _service.ComputeHashAsync(stream, HashAlgorithm.Sha256);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.Hash);
        Assert.Equal(HashAlgorithm.Sha256, result.Value.Algorithm);
        Assert.Equal(64, result.Value.Hash.Length); // SHA256 produces 64 hex characters
    }

    [Fact]
    public async Task WhenComputingHashFromNullStreamThenThrowsArgumentNullException()
    {
        // Arrange
        Stream? nullStream = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.ComputeHashAsync(nullStream!, HashAlgorithm.Blake3));
    }

    [Fact]
    public async Task WhenComputingHashFromNonReadableStreamThenReturnsFailure()
    {
        // Arrange
        using var stream = new MemoryStream();
        stream.Close(); // Closing makes it non-readable

        // Act
        var result = await _service.ComputeHashAsync(stream, HashAlgorithm.Blake3);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task WhenComputingHashFromFileThatDoesNotExistThenReturnsFailure()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}.txt");

        // Act
        var result = await _service.ComputeHashAsync(nonExistentPath, HashAlgorithm.Blake3);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.FileNotFound, result.ErrorInfo.Code);
    }

    [Fact]
    public async Task WhenComputingHashFromNullFilePathThenThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.ComputeHashAsync((string)null!, HashAlgorithm.Blake3));
    }

    [Fact]
    public async Task WhenComputingHashFromEmptyFilePathThenThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.ComputeHashAsync("", HashAlgorithm.Blake3));
    }

    [Fact]
    public async Task WhenComputingHashFromFileThenReturnsCorrectFileSize()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var data = Encoding.UTF8.GetBytes("Test data for hashing");
            await File.WriteAllBytesAsync(tempFile, data);

            // Act
            var result = await _service.ComputeHashAsync(tempFile, HashAlgorithm.Blake3);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(data.Length, result.Value.FileSize);
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
    public async Task WhenComputingHashThenComputationTimeIsRecorded()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Hello, World!");
        using var stream = new MemoryStream(data);

        // Act
        var result = await _service.ComputeHashAsync(stream, HashAlgorithm.Blake3);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.ComputationTime >= TimeSpan.Zero);
    }

    [Fact]
    public async Task WhenComputingBlake3HashThenHashIsLowercase()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");
        using var stream = new MemoryStream(data);

        // Act
        var result = await _service.ComputeHashAsync(stream, HashAlgorithm.Blake3);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Hash, result.Value.Hash.ToLowerInvariant());
    }

    [Fact]
    public async Task WhenComputingSha256HashThenHashIsLowercase()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");
        using var stream = new MemoryStream(data);

        // Act
        var result = await _service.ComputeHashAsync(stream, HashAlgorithm.Sha256);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Hash, result.Value.Hash.ToLowerInvariant());
    }

    [Fact]
    public async Task WhenComputingHashOfSameDataThenProducesSameHash()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Consistent data");
        using var stream1 = new MemoryStream(data);
        using var stream2 = new MemoryStream(data);

        // Act
        var result1 = await _service.ComputeHashAsync(stream1, HashAlgorithm.Blake3);
        var result2 = await _service.ComputeHashAsync(stream2, HashAlgorithm.Blake3);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.Equal(result1.Value.Hash, result2.Value.Hash);
    }

    [Fact]
    public async Task WhenComputingHashOfDifferentDataThenProducesDifferentHash()
    {
        // Arrange
        using var stream1 = new MemoryStream(Encoding.UTF8.GetBytes("Data 1"));
        using var stream2 = new MemoryStream(Encoding.UTF8.GetBytes("Data 2"));

        // Act
        var result1 = await _service.ComputeHashAsync(stream1, HashAlgorithm.Blake3);
        var result2 = await _service.ComputeHashAsync(stream2, HashAlgorithm.Blake3);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.NotEqual(result1.Value.Hash, result2.Value.Hash);
    }

    [Fact]
    public async Task WhenComputingSha512HashThenReturnsCorrectLength()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data for SHA512");
        using var stream = new MemoryStream(data);

        // Act
        var result = await _service.ComputeHashAsync(stream, HashAlgorithm.Sha512);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(HashAlgorithm.Sha512, result.Value.Algorithm);
        Assert.Equal(128, result.Value.Hash.Length); // SHA512 produces 128 hex characters
    }

    [Fact]
    public async Task WhenComputingCrc32HashThenReturnsCorrectLength()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data for CRC32");
        using var stream = new MemoryStream(data);

        // Act
        var result = await _service.ComputeHashAsync(stream, HashAlgorithm.Crc32);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(HashAlgorithm.Crc32, result.Value.Algorithm);
        Assert.Equal(8, result.Value.Hash.Length); // CRC32 produces 8 hex characters
    }

    [Fact]
    public async Task WhenComputingHashFromFileThenFilePathIsIncludedInResult()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var data = Encoding.UTF8.GetBytes("Test data");
            await File.WriteAllBytesAsync(tempFile, data);

            // Act
            var result = await _service.ComputeHashAsync(tempFile, HashAlgorithm.Sha256);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(tempFile, result.Value.FilePath);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}
