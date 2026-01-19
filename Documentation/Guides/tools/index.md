# Tools Library

The **CivitaiSharp.Tools** library provides utility features for working with Civitai resources, including file hashing, downloading, and HTML parsing. These tools complement the Core and SDK libraries by handling common tasks when working with AI models and images.

## Overview

The Tools library offers practical utilities for:

- **File Hashing**: Compute and verify hashes for model integrity
- **Downloads**: Download models and images with path templating
- **HTML Parsing**: Convert Civitai HTML descriptions to markdown or plain text

## Key Features

### File Hashing

- Support for multiple hash algorithms (SHA256, SHA512, BLAKE3, CRC32)
- Stream-based hashing for large files
- Hash verification against Civitai metadata
- Performance metrics (computation time, file size)

### Downloads

- Automatic directory creation
- Path pattern templating (`{Username}`, `{ModelName}`, `{ModelType}`, etc.)
- Hash verification during download
- Resume support
- Progress tracking
- Custom download locations

### HTML Parsing

- Convert HTML to Markdown
- Extract plain text from HTML
- Extension methods for Model and ModelVersion
- Preserve formatting and structure

## Quick Links

- [Introduction](introduction.md) - Getting started with Tools
- [File Hashing](file-hashing.md) - Computing and verifying hashes
- [Downloading Files](downloading-files.md) - Download models and images
- [HTML Parsing](html-parsing.md) - Convert HTML descriptions

## Installation

```bash
dotnet add package CivitaiSharp.Tools
```

Note: The Tools package includes the Core library as a dependency.

## Basic Example

```csharp
using CivitaiSharp.Core;
using CivitaiSharp.Core.Extensions;
using CivitaiSharp.Tools.Extensions;
using CivitaiSharp.Tools.Hashing;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCivitaiApi();

// Configure download options
builder.Services.AddCivitaiDownloads(options =>
{
    options.Models.BaseDirectory = @"C:\Models";
    options.Models.PathPattern = "{ModelType}/{ModelName}/{FileName}";
    options.Models.VerifyHash = true;
});

var host = builder.Build();
await host.StartAsync();

var apiClient = host.Services.GetRequiredService<IApiClient>();
var hashingService = host.Services.GetRequiredService<IFileHashingService>();
var downloadService = host.Services.GetRequiredService<IDownloadService>();

// Compute hash of a file
var hashResult = await hashingService.ComputeHashAsync(
    @"C:\Models\model.safetensors",
    HashAlgorithm.Sha256);

if (hashResult is Result<HashedFile>.Success success)
{
    Console.WriteLine($"SHA256: {success.Data.Hash}");
    Console.WriteLine($"Size: {success.Data.FileSize:N0} bytes");
}

// Download a model with automatic path creation
var modelResult = await apiClient.Models.GetByIdAsync(4201);
if (modelResult is Result<Model>.Success modelSuccess)
{
    var version = modelSuccess.Data.ModelVersions?.FirstOrDefault();
    var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
    
    if (file is not null && version is not null)
    {
        var downloadResult = await downloadService.DownloadAsync(file, version);
        
        if (downloadResult is Result<DownloadedFile>.Success dlSuccess)
        {
            Console.WriteLine($"Downloaded to: {dlSuccess.Data.FilePath}");
            Console.WriteLine($"Verified: {dlSuccess.Data.IsVerified}");
        }
    }
}

await host.StopAsync();
```

## Next Steps

- Start with [Introduction](introduction.md) for an overview
- Learn [File Hashing](file-hashing.md) for integrity verification
- Explore [Downloading Files](downloading-files.md) for automated downloads
- Use [HTML Parsing](html-parsing.md) to work with descriptions
