# SDK Library

The **CivitaiSharp.Sdk** library extends the Core library with advanced features, utilities, and specialized functionality for working with Civitai resources. The primary focus is the **AIR (AI Resource)** system for standardized resource identification.

## Overview

The SDK library provides tools for working with AIR identifiers, which are URN-based identifiers that uniquely identify AI resources across different platforms and ecosystems. AIR makes it easy to reference models, versions, and other resources in a standardized format.

## Key Features

- **AIR Identifiers** - Standardized URN-based resource identification
- **Image Generation** - Submit and track image generation jobs
- **Coverage Service** - Check model and resource availability
- **Usage Tracking** - Monitor API consumption and credits

## Quick Links

- [Introduction](introduction.md) - Getting started with the SDK
- [AIR Identifier](air-identifier.md) - Parsing and using AIR identifiers
- [Create a Job](create-job.md) - Submit image generation jobs
- [Query a Job](query-job.md) - Track job status
- [Coverage](coverage.md) - Check model and resource availability
- [Usage](usage.md) - Monitor API consumption and credits

## Installation

```bash
dotnet add package CivitaiSharp.Sdk
```

Note: The SDK package includes the Core library as a dependency.

For complete examples, see [Introduction](introduction.md).

## Next Steps

- Start with [Introduction](introduction.md) to understand AIR concepts
- Learn [AIR Identifier](air-identifier.md) for parsing and validation
