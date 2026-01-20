---
title: Downloading Files
description: Download images and models with verification.
---

# Downloading Files

Download images and models with customizable path patterns, hash verification, and format detection.

## Service Registration

**Default:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#DefaultConfiguration)]

**From Config:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#FromConfiguration)]

**Programmatic:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#ProgrammaticConfiguration)]

## Download Images

**Basic:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadImage)]

**Custom Directory:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadImageCustom)]

## Image Path Tokens

Tokens are replaced by the service at download time:

`{Id}`, `{PostId}`, `{Username}`, `{Width}`, `{Height}`, `{BaseModel}`, `{NsfwLevel}`, `{Date}`, `{Extension}`

**Example:**
```json
{
  "PathPattern": "{BaseModel}/{Username}/{Date}_{Id}.{Extension}"
}
```
Result: `SDXL 1.0/ArtistName/2026-01-15_12345678.png`

## Download Models

**Basic:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadModel)]

**With Version:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadModelVersion)]

**From URL:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadUrl)]

## Model Path Tokens

**File tokens:** `{FileId}`, `{FileName}`, `{FileType}`, `{Format}`, `{Size}`, `{Precision}`

**Version tokens:** `{VersionId}`, `{VersionName}`, `{BaseModel}`, `{ModelId}`, `{ModelName}`, `{ModelType}`

**Example:**
```json
{
  "PathPattern": "{ModelType}/{BaseModel}/{ModelName}/{VersionName}/{FileName}"
}
```

## Hash Verification

[!code-csharp[Program.cs](examples/Tools/Program.cs#HashVerificationConfig)]

Automatically downloads, computes hash, compares against Civitai metadata, and deletes on mismatch.

**Verification Result:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#VerificationResult)]

## File Format Detection

[!code-csharp[Program.cs](examples/Tools/Program.cs#FileFormatDetection)]

**Supported:** PNG, JPEG, GIF, WebP, AVIF, HEIC, MP4, WebM

## Error Handling

**Codes:** `ImageUrlMissing`, `DownloadUrlMissing`, `HashVerificationFailed`, `HashComputationFailed`

[!code-csharp[Program.cs](examples/Tools/Program.cs#ErrorHandling)]

## Configuration

**ImageDownloadOptions:**
- `BaseDirectory` - Root directory
- `PathPattern` - Pattern with tokens
- `OverwriteExisting` - Replace files

**ModelDownloadOptions:**
- `BaseDirectory` - Root directory
- `PathPattern` - Pattern with tokens
- `OverwriteExisting` - Replace files
- `VerifyHash` - Verify after download
- `HashAlgorithm` - Algorithm for verification

## Next Steps

- [File Hashing](file-hashing.md)
- [HTML Parsing](html-parsing.md)
