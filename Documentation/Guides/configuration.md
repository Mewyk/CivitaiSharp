---
title: Configuration
description: Configure CivitaiSharp API clients with options for API keys, base URLs, timeouts, and resilience policies.
---

# Configuration

This guide covers all configuration options for the CivitaiSharp API clients.

## CivitaiSharp.Core Configuration

The `ApiClientOptions` class configures the low-level API client.

### Using an Action Delegate

[!code-csharp[Program.cs](Configuration/Program.cs#L1-L18)]

### Using IConfiguration

You can also configure options from `appsettings.json`:

```json
{
  "CivitaiApi": {
    "BaseUrl": "https://civitai.com",
    "ApiVersion": "v1",
    "ApiKey": "your-api-key",
    "TimeoutSeconds": 30
  }
}
```

[!code-csharp[Program.cs](Configuration/Program.cs#L20-L32)]

### Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `BaseUrl` | `https://civitai.com` | The base URL for the Civitai API. Must be a valid HTTP or HTTPS URL. |
| `ApiVersion` | `v1` | The API version path segment. |
| `ApiKey` | `null` | Optional API key for authenticated requests. Required for favorites, hidden models, and higher rate limits. |
| `TimeoutSeconds` | `30` | HTTP request timeout. Must be between 1 and 300 seconds. |

> [!NOTE]
> The Core library can query public endpoints (models, images, tags, creators) without an API key. An API key is only required for authenticated features.

### Validation

Options are validated on assignment:

- `BaseUrl` must be a valid absolute HTTP or HTTPS URI
- `ApiVersion` cannot be null or whitespace
- `TimeoutSeconds` must be between 1 and 300

## CivitaiSharp.Sdk Configuration

The `CivitaiSdkClientOptions` class configures the Generator SDK client.

> [!IMPORTANT]
> Unlike CivitaiSharp.Core which can access public endpoints anonymously, the SDK **always requires authentication**. All Generator API operations require a valid API token.

[!code-csharp[SdkConfiguration.cs](Configuration/SdkConfiguration.cs#sdk-configuration)]

### Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `BaseUrl` | `https://orchestration.civitai.com` | The base URL for the Orchestration API. |
| `ApiVersion` | `v1` | The API version path segment. |
| `ApiToken` | (required) | Your Civitai API token. Required for all operations. |
| `TimeoutSeconds` | `600` | HTTP request timeout. Must be between 1 and 1800 seconds. |

## Next Steps

- [Request Builders](request-builders.md) - Learn the fluent API pattern
- [Working with Models](models.md) - Query and filter models
