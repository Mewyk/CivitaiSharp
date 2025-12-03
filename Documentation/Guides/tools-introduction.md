---
title: CivitaiSharp.Tools Introduction
description: Learn about CivitaiSharp.Tools, a utility library for file hashing, downloading models and images, and parsing HTML descriptions from Civitai.
---

# CivitaiSharp.Tools

CivitaiSharp.Tools provides utility functionality for working with Civitai resources including file hashing, download management, and HTML parsing. It is designed to complement CivitaiSharp.Core by providing common operations needed when working with AI models and images.

## Key Features

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

```csharp
public class MyService(IFileHashingService hashingService)
{
    public async Task VerifyFileAsync(string filePath)
    {
        var result = await hashingService.ComputeHashAsync(filePath, HashAlgorithm.Sha256);
        
        if (result is Result<HashedFile>.Success success)
        {
            Console.WriteLine($"Hash: {success.Data.Hash}");
            Console.WriteLine($"Size: {success.Data.FileSize} bytes");
            Console.WriteLine($"Time: {success.Data.ComputationTime.TotalMilliseconds}ms");
        }
    }
}
```

### Download Service

The `IDownloadService` downloads images and model files with automatic path generation:

```csharp
public class MyService(IDownloadService downloadService, IApiClient apiClient)
{
    public async Task DownloadModelAsync()
    {
        // Get a model version
        var modelResult = await apiClient.Models.GetByIdAsync(123456);
        if (modelResult is not Result<Model>.Success modelSuccess)
            return;
            
        var model = modelSuccess.Data;
        var version = model.ModelVersions?.FirstOrDefault();
        var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
        
        if (file is null || version is null)
            return;
        
        // Download with hash verification
        var result = await downloadService.DownloadAsync(file, version);
        
        if (result is Result<DownloadedFile>.Success success)
        {
            Console.WriteLine($"Downloaded to: {success.Data.FilePath}");
            Console.WriteLine($"Verified: {success.Data.IsVerified}");
        }
    }
}
```

### HTML Parser

The `HtmlParser` converts Civitai's HTML descriptions to readable formats:

```csharp
// Using the static parser
var markdown = HtmlParser.ToMarkdown(model.Description);
var plainText = HtmlParser.ToPlainText(model.Description);

// Or using extension methods
var markdown = model.GetDescriptionAsMarkdown();
var plainText = model.GetDescriptionAsPlainText();
```

## Configuration

Configure download behavior via appsettings.json:

```json
{
  "CivitaiDownloads": {
    "Images": {
      "BaseDirectory": "C:\\Downloads\\Images",
      "PathPattern": "{BaseModel}/{Username}/{Id}.{Extension}",
      "OverwriteExisting": false
    },
    "Models": {
      "BaseDirectory": "C:\\Models",
      "PathPattern": "{ModelType}/{BaseModel}/{ModelName}/{VersionName}/{FileName}",
      "OverwriteExisting": true,
      "VerifyHash": true,
      "HashAlgorithm": "Sha256"
    }
  }
}
```

## Guides

- [File Hashing](file-hashing.md) - Compute and verify file hashes
- [Downloading Files](downloading-files.md) - Download images and model files
- [HTML Parsing](html-parsing.md) - Convert descriptions to Markdown or plain text
