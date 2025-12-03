---
title: File Hashing
description: Compute SHA256, SHA512, BLAKE3, and CRC32 hashes for files using CivitaiSharp.Tools for verification against Civitai model metadata.
---

# File Hashing

CivitaiSharp.Tools provides the `IFileHashingService` for computing cryptographic hashes of files and streams. This is essential for verifying downloaded model files against the hashes provided by Civitai.

## Supported Algorithms

| Algorithm | Description | Output Length |
|-----------|-------------|---------------|
| `Sha256` | SHA-256, widely compatible and used by Civitai | 64 hex characters |
| `Sha512` | SHA-512, stronger security | 128 hex characters |
| `Blake3` | BLAKE3, fast and modern, used by Civitai | 64 hex characters |
| `Crc32` | CRC32, fast integrity check | 8 hex characters |

## Basic Usage

### Hashing a File

[!code-csharp[Program.cs](Tools/Program.cs#hash-file)]

### Hashing a Stream

[!code-csharp[Program.cs](Tools/Program.cs#hash-stream)]

## The HashedFile Record

The `HashedFile` record contains comprehensive hash information:

| Property | Type | Description |
|----------|------|-------------|
| `FilePath` | `string?` | Absolute path to the hashed file, or null if from stream |
| `Hash` | `string` | Computed hash as lowercase hexadecimal |
| `Algorithm` | `HashAlgorithm` | Algorithm used for computation |
| `FileSize` | `long` | Size in bytes (-1 for non-seekable streams) |
| `ComputationTime` | `TimeSpan` | Time taken to compute the hash |

## Verifying Downloads

Use file hashing to verify downloaded model files match Civitai's metadata:

[!code-csharp[Program.cs](Tools/Program.cs#verify-download)]

## Comparing Multiple Hashes

Compute multiple hash types for a single file:

[!code-csharp[Program.cs](Tools/Program.cs#multiple-hashes)]

## Performance Considerations

### Buffer Size

The service uses an 80 KB buffer for efficient streaming. This balances memory usage with I/O performance.

### BLAKE3 Performance

BLAKE3 is significantly faster than SHA256 for large files while maintaining cryptographic security. Consider using BLAKE3 when:

- Processing many large files
- Hash values are available from Civitai (many models include BLAKE3 hashes)
- Performance is critical

### Async Operations

All hashing operations are asynchronous and support cancellation:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

var result = await hashingService.ComputeHashAsync(
    largeFilePath, 
    HashAlgorithm.Blake3, 
    cts.Token);
```

## Error Handling

The service returns `Result<HashedFile>` which can indicate various failures:

| Error Code | Description |
|------------|-------------|
| `FileNotFound` | The specified file does not exist |
| `StreamNotReadable` | The provided stream cannot be read |
| `HashComputationFailed` | An error occurred during hash computation |

Handle errors using pattern matching:

```csharp
var result = await hashingService.ComputeHashAsync(filePath, HashAlgorithm.Sha256);

switch (result)
{
    case Result<HashedFile>.Success success:
        Console.WriteLine($"Hash: {success.Data.Hash}");
        break;
    case Result<HashedFile>.Failure { Error.Code: ErrorCode.FileNotFound }:
        Console.WriteLine("File not found");
        break;
    case Result<HashedFile>.Failure failure:
        Console.WriteLine($"Error: {failure.Error.Message}");
        break;
}
```

## Integration with Downloads

The download service automatically verifies hashes when `VerifyHash` is enabled:

```csharp
services.AddCivitaiDownloads(options =>
{
    options.Models.VerifyHash = true;
    options.Models.HashAlgorithm = HashAlgorithm.Sha256;
});
```

Downloaded files are verified against Civitai's provided hashes, and verification failures result in automatic cleanup of corrupted files.

## Next Steps

- [Downloading Files](downloading-files.md) - Download with automatic verification
- [HTML Parsing](html-parsing.md) - Parse model descriptions
