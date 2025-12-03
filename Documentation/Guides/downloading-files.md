---
title: Downloading Files
description: Download images and model files from Civitai with configurable path patterns, automatic hash verification, and format detection.
---

# Downloading Files

CivitaiSharp.Tools provides the `IDownloadService` for downloading images and model files from Civitai. The service supports customizable path patterns, automatic hash verification, and handles file organization.

## Service Registration

### Default Configuration

```csharp
// Uses system temp directory with minimal patterns
services.AddCivitaiDownloads();
```

### From Configuration

```csharp
services.AddCivitaiDownloads(configuration.GetSection("CivitaiDownloads"));
```

### Programmatic Configuration

```csharp
services.AddCivitaiDownloads(options =>
{
    options.Images.BaseDirectory = @"C:\Downloads\Images";
    options.Images.PathPattern = "{Username}/{Id}.{Extension}";
    options.Images.OverwriteExisting = false;
    
    options.Models.BaseDirectory = @"C:\Models";
    options.Models.PathPattern = "{ModelType}/{ModelName}/{FileName}";
    options.Models.OverwriteExisting = true;
    options.Models.VerifyHash = true;
    options.Models.HashAlgorithm = HashAlgorithm.Sha256;
});
```

## Downloading Images

### Basic Image Download

[!code-csharp[Program.cs](Tools/Program.cs#download-image)]

### Custom Directory

[!code-csharp[Program.cs](Tools/Program.cs#download-image-custom)]

## Image Path Tokens

Path patterns for images support these tokens:

| Token | Description | Example |
|-------|-------------|---------|
| `{Id}` | Unique image identifier | `12345678` |
| `{PostId}` | Post containing the image | `987654` |
| `{Username}` | Creator's username | `ArtistName` |
| `{Width}` | Image width in pixels | `1024` |
| `{Height}` | Image height in pixels | `1024` |
| `{BaseModel}` | Base model used | `SDXL 1.0` |
| `{NsfwLevel}` | Content level | `None`, `Soft`, `Mature` |
| `{Date}` | Creation date | `2024-01-15` |
| `{Extension}` | File extension | `png`, `jpg`, `webp` |

### Pattern Examples

```json
{
  "Images": {
    "PathPattern": "{Id}.{Extension}"
  }
}
```
Result: `12345678.png`

```json
{
  "Images": {
    "PathPattern": "{Username}/{Id}.{Extension}"
  }
}
```
Result: `ArtistName/12345678.png`

```json
{
  "Images": {
    "PathPattern": "{BaseModel}/{Username}/{Date}_{Id}.{Extension}"
  }
}
```
Result: `SDXL 1.0/ArtistName/2024-01-15_12345678.png`

## Downloading Model Files

### Basic Model Download

[!code-csharp[Program.cs](Tools/Program.cs#download-model)]

### Download with Version Context

[!code-csharp[Program.cs](Tools/Program.cs#download-model-version)]

### Download from URL

[!code-csharp[Program.cs](Tools/Program.cs#download-url)]

## Model Path Tokens

### File-Only Tokens

Available when downloading a `ModelFile`:

| Token | Description | Example |
|-------|-------------|---------|
| `{FileId}` | Unique file identifier | `456789` |
| `{FileName}` | Original file name | `model_v1.safetensors` |
| `{FileType}` | File type | `Model`, `Training Data` |
| `{Format}` | Model format | `SafeTensor`, `PickleTensor` |
| `{Size}` | Size specification | `full`, `pruned` |
| `{Precision}` | Float precision | `fp16`, `fp32` |

### Version Tokens

Additional tokens when `ModelVersion` is provided:

| Token | Description | Example |
|-------|-------------|---------|
| `{VersionId}` | Version identifier | `130072` |
| `{VersionName}` | Version name | `v1.0` |
| `{BaseModel}` | Base model | `SDXL 1.0` |
| `{ModelId}` | Parent model ID | `4201` |
| `{ModelName}` | Parent model name | `Realistic Vision` |
| `{ModelType}` | Model type | `Checkpoint`, `LORA` |

### Pattern Examples

```json
{
  "Models": {
    "PathPattern": "{FileName}"
  }
}
```
Result: `model_v1.safetensors`

```json
{
  "Models": {
    "PathPattern": "{ModelType}/{FileName}"
  }
}
```
Result: `Checkpoint/model_v1.safetensors`

```json
{
  "Models": {
    "PathPattern": "{ModelType}/{BaseModel}/{ModelName}/{VersionName}/{FileName}"
  }
}
```
Result: `Checkpoint/SDXL 1.0/Realistic Vision/v1.0/model_v1.safetensors`

## Hash Verification

Enable automatic hash verification for model downloads:

```csharp
services.AddCivitaiDownloads(options =>
{
    options.Models.VerifyHash = true;
    options.Models.HashAlgorithm = HashAlgorithm.Sha256;
});
```

The service:
1. Downloads the file
2. Computes the hash using the specified algorithm
3. Compares against Civitai's metadata
4. Deletes the file if verification fails
5. Returns success with verification status

### Verification Result

```csharp
if (result is Result<DownloadedFile>.Success success)
{
    if (success.Data.IsVerified)
    {
        Console.WriteLine($"Verified hash: {success.Data.ComputedHash}");
    }
    else
    {
        Console.WriteLine("Hash verification was not performed");
    }
}
```

## The DownloadedFile Record

| Property | Type | Description |
|----------|------|-------------|
| `FilePath` | `string` | Absolute path to downloaded file |
| `SizeBytes` | `long` | File size in bytes |
| `IsVerified` | `bool` | True if hash was verified |
| `ComputedHash` | `string?` | Hash value if computed |

## File Format Detection

The `FileFormatDetector` identifies file types from magic bytes:

```csharp
// Detect from file path
var format = await FileFormatDetector.DetectFormatAsync(filePath);
Console.WriteLine($"Detected format: {format}"); // "png", "jpg", "mp4", etc.

// Detect from stream
await using var stream = File.OpenRead(filePath);
var format = await FileFormatDetector.DetectFormatAsync(stream);

// Detect from bytes
var header = new byte[16];
await stream.ReadAsync(header);
var format = FileFormatDetector.DetectFormat(header.AsSpan());
```

### Supported Formats

| Format | Extension | Description |
|--------|-----------|-------------|
| PNG | `png` | Portable Network Graphics |
| JPEG | `jpg` | Joint Photographic Experts Group |
| GIF | `gif` | Graphics Interchange Format |
| WebP | `webp` | Modern image format |
| AVIF | `avif` | AV1 Image File Format |
| HEIC | `heic` | High Efficiency Image Container |
| MP4 | `mp4` | MPEG-4 video |
| WebM | `webm` | Open media container |

## Error Handling

Download operations return `Result<DownloadedFile>` with specific error codes:

| Error Code | Description |
|------------|-------------|
| `ImageUrlMissing` | Image URL is null or empty |
| `DownloadUrlMissing` | Model download URL is missing |
| `InvalidUrl` | URL is not a valid HTTP/HTTPS URI |
| `HttpError` | HTTP request failed |
| `Timeout` | Request timed out |
| `FileWriteFailed` | Failed to write file to disk |
| `DirectoryCreationFailed` | Failed to create directory |
| `HashVerificationFailed` | Hash mismatch after download |
| `HashComputationFailed` | Failed to compute file hash |
| `IoError` | File exists and overwrite disabled |

### Example Error Handling

```csharp
var result = await downloadService.DownloadAsync(file, version);

switch (result)
{
    case Result<DownloadedFile>.Success success:
        Console.WriteLine($"Downloaded: {success.Data.FilePath}");
        break;
        
    case Result<DownloadedFile>.Failure { Error.Code: ErrorCode.HashVerificationFailed } failure:
        Console.WriteLine($"Corrupted download: {failure.Error.Message}");
        break;
        
    case Result<DownloadedFile>.Failure { Error.Code: ErrorCode.IoError } failure:
        Console.WriteLine($"File exists: {failure.Error.Message}");
        break;
        
    case Result<DownloadedFile>.Failure failure:
        Console.WriteLine($"Download failed: {failure.Error.Message}");
        break;
}
```

## Configuration Options

### ImageDownloadOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BaseDirectory` | `string` | Temp path | Root directory for images |
| `PathPattern` | `string` | `{Id}.{Extension}` | Path pattern with tokens |
| `OverwriteExisting` | `bool` | `false` | Replace existing files |

### ModelDownloadOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BaseDirectory` | `string` | Temp path | Root directory for models |
| `PathPattern` | `string` | `{FileName}` | Path pattern with tokens |
| `OverwriteExisting` | `bool` | `true` | Replace existing files |
| `VerifyHash` | `bool` | `true` | Verify after download |
| `HashAlgorithm` | `HashAlgorithm` | `Sha256` | Algorithm for verification |

## Path Sanitization

The service automatically sanitizes path segments:

- Invalid characters are replaced with underscores
- Windows reserved names (CON, PRN, etc.) are suffixed with underscore
- Directory separators are normalized for the current OS
- Duplicate separators are collapsed

## Next Steps

- [File Hashing](file-hashing.md) - Manual hash computation
- [HTML Parsing](html-parsing.md) - Parse model descriptions
