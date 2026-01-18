---
title: Configuration
description: Configure CivitaiSharp API clients with options for API keys, base URLs, timeouts, and resilience policies.
---

# Configuration

This guide covers all configuration options for the CivitaiSharp API clients.

## CivitaiSharp.Core Configuration

The `ApiClientOptions` class configures the low-level API client.

### Using an Action Delegate

[!code-csharp[Program.cs](../examples/Common/Program.cs#CoreSetupWithApiKey)]

### Using IConfiguration

You can also configure options from `appsettings.json`:

```json
{
  "CivitaiApi": {
    "ApiKey": "your-api-key",
    "TimeoutSeconds": 30
  }
}
```

[!code-csharp[Program.cs](examples/Configuration/Program.cs#IConfiguration)]

### Configuration Options
| Option | Default | Description |
|--------|---------|-------------|
| `ApiKey` | `null` | Optional API key for authenticated requests. Required for favorites, hidden models, and higher rate limits. |
| `TimeoutSeconds` | `30` | HTTP request timeout. Must be between 1 and 300 seconds. |

> [!NOTE]
> The Core library can query public endpoints (models, images, tags, creators) without an API key. An API key is only required for authenticated features.

### Validation

All options are validated on assignment:
- `ApiVersion` cannot be null or whitespace
- `TimeoutSeconds` must be between 1 and 300

## Next Steps

- [Request Builders](request-builders.md) - Learn the fluent API pattern
- [Working with Models](models.md) - Query and filter models
