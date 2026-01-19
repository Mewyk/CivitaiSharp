---
title: CivitaiSharp.Tools Introduction
description: Learn about CivitaiSharp.Tools, a utility library for file hashing, downloading models and images, and parsing HTML descriptions from Civitai.
---

# CivitaiSharp.Tools

CivitaiSharp.Tools provides utility functionality for working with Civitai resources including file hashing, download management, and HTML parsing. It is designed to complement CivitaiSharp.Core by providing common operations needed when working with AI models and images.

## Key Features

- **AIR Builder** - Fluent API for constructing AIR (Artificial Intelligence Resource) identifiers with validation
- **File Hashing** - Compute SHA256, SHA512, BLAKE3, and CRC32 hashes for file verification
- **Download Management** - Download images and model files with configurable path patterns and hash verification
- **HTML Parsing** - Convert Civitai's HTML descriptions to Markdown or plain text
- **File Format Detection** - Identify image and video formats from magic bytes
- **AOT Compatible** - Full support for Native AOT compilation and trimming

## Getting Started

### Installation

```bash
dotnet add package CivitaiSharp.Tools --prerelease
```

### Registration

Register the Tools services using dependency injection:

```csharp
// Basic registration with default options
services.AddCivitaiDownloads();

// Or with configuration from IConfiguration
services.AddCivitaiDownloads(configuration.GetSection("CivitaiDownloads"));

// Or with programmatic configuration
services.AddCivitaiDownloads(options =>
{
    options.Images.BaseDirectory = @"C:\Downloads\Images";
    options.Images.PathPattern = "{Username}/{Id}.{Extension}";
    
    options.Models.BaseDirectory = @"C:\Models";
    options.Models.PathPattern = "{ModelType}/{ModelName}/{FileName}";
    options.Models.VerifyHash = true;
    options.Models.HashAlgorithm = HashAlgorithm.Sha256;
});
```

## Services Overview

### File Hashing Service

The `IFileHashingService` computes cryptographic hashes for files and streams:

[!code-csharp[Program.cs](examples/Tools/Program.cs#FileHashingServiceUsage)]

### Download Service

The `IDownloadService` downloads images and model files with automatic path generation:

[!code-csharp[Program.cs](examples/Tools/Program.cs#DownloadServiceUsage)]

### HTML Parser

The `HtmlParser` converts Civitai's HTML descriptions to readable formats:

[!code-csharp[Program.cs](examples/Tools/Program.cs#HtmlParserUsage)]

## Configuration

Configure download behavior via appsettings.json:

[!code-json[appsettings.json](examples/Tools/appsettings.json#L5-L18)]

## Guides

- [AIR Builder](../sdk/air-builder.md) - Build AIR identifiers with a fluent API
- [File Hashing](file-hashing.md) - Compute and verify file hashes
- [Downloading Files](downloading-files.md) - Download images and model files
- [HTML Parsing](html-parsing.md) - Convert descriptions to Markdown or plain text
