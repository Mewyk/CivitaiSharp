---
title: Downloading Files
description: Download images and model files from Civitai with configurable path patterns, automatic hash verification, and format detection.
---

# Downloading Files

CivitaiSharp.Tools provides the `IDownloadService` for downloading images and model files from Civitai. The service supports customizable path patterns, automatic hash verification, and handles file organization.

## Service Registration

### Default Configuration

[!code-csharp[Program.cs](examples/Tools/Program.cs#DefaultConfiguration)]

### From Configuration

[!code-csharp[Program.cs](examples/Tools/Program.cs#FromConfiguration)]

### Programmatic Configuration

[!code-csharp[Program.cs](examples/Tools/Program.cs#ProgrammaticConfiguration)]

## Downloading Images

### Basic Image Download

[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadImage)]

### Custom Directory

[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadImageCustom)]

## Image Path Tokens

Path patterns for images support these tokens. These are **template tokens** replaced by the download service, not C# string interpolation:

> **Note**: Tokens use `{TokenName}` syntax but are processed by the service, not by C# string interpolation. They are replaced at download time with actual values.

| Token | Description | Example |
|-------|-------------|---------|
| `{Id}` | Unique image identifier | `12345678` |
| `{PostId}` | Post containing the image | `987654` |
| `{Username}` | Creator's username | `ArtistName` |
| `{Width}` | Image width in pixels | `1024` |
| `{Height}` | Image height in pixels | `1024` |
| `{BaseModel}` | Base model used | `SDXL 1.0` |
| `{NsfwLevel}` | Content level | `None`, `Soft`, `Mature` |
| `{Date}` | Creation date | `2026-01-15` |
| `{Extension}` | File extension | `png`, `jpg`, `webp` |

### Pattern Example

[!code-json[appsettings.json](examples/Tools/appsettings.json#L6-L10)]

Result: `SDXL 1.0/ArtistName/2026-01-15_12345678.png`

## Downloading Model Files

### Basic Model Download

[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadModel)]

### Download with Version Context

[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadModelVersion)]

### Download from URL

[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadUrl)]

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

### Pattern Example

[!code-json[appsettings.json](examples/Tools/appsettings.json#L11-L17)]

Result: `Checkpoint/SDXL 1.0/Realistic Vision/v1.0/model_v1.safetensors`

## Hash Verification

Enable automatic hash verification for model downloads:

[!code-csharp[Program.cs](examples/Tools/Program.cs#HashVerificationConfig)]

The service:
1. Downloads the file
2. Computes the hash using the specified algorithm
3. Compares against Civitai's metadata
4. Deletes the file if verification fails
5. Returns success with verification status

### Verification Result

[!code-csharp[Program.cs](examples/Tools/Program.cs#VerificationResult)]

## The DownloadedFile Record

| Property | Type | Description |
|----------|------|-------------|
| `FilePath` | `string` | Absolute path to downloaded file |
| `SizeBytes` | `long` | File size in bytes |
| `IsVerified` | `bool` | True if hash was verified |
| `ComputedHash` | `string?` | Hash value if computed |

## File Format Detection

The `FileFormatDetector` identifies file types from magic bytes:

[!code-csharp[Program.cs](examples/Tools/Program.cs#FileFormatDetection)]

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

[!code-csharp[Program.cs](examples/Tools/Program.cs#ErrorHandling)]

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
