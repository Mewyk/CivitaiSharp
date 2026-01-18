---
title: SDK Configuration
description: Configure the CivitaiSharp.Sdk client with API token and timeout settings for the Civitai Generator API.
---

# SDK Configuration

This guide covers configuration options for the CivitaiSharp.Sdk client.

> [!IMPORTANT]
> Unlike CivitaiSharp.Core which can access public endpoints anonymously, the SDK **always requires authentication**. All Generator API operations require a valid API token.

## Basic Configuration

[!code-csharp[Program.cs](examples/Configuration/Program.cs#SdkConfiguration)]

## Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `ApiToken` | (required) | Your Civitai API token. Required for all operations. |
| `TimeoutSeconds` | `600` | HTTP request timeout. Must be between 1 and 1800 seconds (30 minutes). Generator jobs can take several minutes. |

## Getting an API Token

See the [Getting an API Key](../core/getting-api-key.md) guide for instructions on obtaining your Civitai API token.

## Next Steps

- [SDK Introduction](introduction.md) - Learn about SDK features
- [Jobs Service](jobs.md) - Submit and manage generation jobs
- [Coverage Service](coverage.md) - Check model availability
