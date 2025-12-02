---
title: Installation
description: Install CivitaiSharp via NuGet. Choose between the Core library for low-level API access or the SDK for high-level image generation workflows.
---

# Installation

CivitaiSharp can be installed via NuGet. Choose the package that fits your needs:

## CivitaiSharp.Core

The core library provides low-level access to the Civitai API with fluent request builders, typed models, and result-based error handling.

### Via .NET CLI

```bash
dotnet add package CivitaiSharp.Core --prerelease
```

### Via Package Manager Console

```powershell
Install-Package CivitaiSharp.Core -PreRelease
```

## CivitaiSharp.Sdk

The SDK provides high-level abstractions for common workflows like image generation, job management, and usage tracking.

### Via .NET CLI

```bash
dotnet add package CivitaiSharp.Sdk --prerelease
```

### Via Package Manager Console

```powershell
Install-Package CivitaiSharp.Sdk -PreRelease
```

## CivitaiSharp.Tools

Utility library for downloading models, hashing files, and parsing HTML metadata.

### Via .NET CLI

```bash
dotnet add package CivitaiSharp.Tools --prerelease
```

### Via Package Manager Console

```powershell
Install-Package CivitaiSharp.Tools -PreRelease
```

## Requirements

- **.NET 10.0 or later**
- **C# 14** (for latest language features)
