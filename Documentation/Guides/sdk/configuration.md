---
title: SDK Configuration
description: Configure the CivitaiSharp.Sdk client with API token and timeout settings for the Civitai Generator API.
---

# SDK Configuration

This guide covers configuration options for the CivitaiSharp.Sdk client.

## Automatic Configuration (Best Practice)

> [!TIP]
> **Always use automatic configuration.** It's the cleanest, most maintainable approach and follows .NET configuration best practices.

Place your API token in `appsettings.json` under the `CivitaiSdk` section:

[!code-json[appsettings.json](examples/Configuration/appsettings.json)]

Then register the services:

[!code-csharp[Program.cs](examples/Configuration/Program.cs#AutomaticConfiguration)]

That's it! The SDK automatically discovers and reads the `CivitaiSdk` configuration section. No manual property mapping, no hardcoded values, no configuration keys to remember.

### Why Automatic Configuration?

- **Clean code** - One line instead of lambda configuration blocks
- **Separation of concerns** - Configuration stays in config files, not code
- **Environment-specific** - Easy to override per environment (dev/staging/prod)
- **Secure** - Use User Secrets, Azure Key Vault, or environment variables
- **Standard** - Follows .NET configuration patterns

## Manual Configuration (Not Recommended)

> [!WARNING]
> Manual configuration with hardcoded values is not recommended. Use automatic configuration from `appsettings.json` instead.

If you must configure manually, you still need the API token in configuration or hardcoded (not recommended). The SDK always requires authentication.

## Advanced: Configuration with Overrides

If you need to load base settings from `appsettings.json` but override specific options:

[!code-csharp[Program.cs](examples/Configuration/Program.cs#ConfigurationWithOverrides)]

## Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `ApiToken` | (required) | Your Civitai API token. Required for all operations. |
| `TimeoutSeconds` | `600` | HTTP request timeout. Must be between 1 and 1800 seconds (30 minutes). Generator jobs can take several minutes. |

## Secure Token Storage

### Development
Use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
dotnet user-secrets set "CivitaiSdk:ApiToken" "your-api-token"
```

### Production
- **Azure**: Use Azure Key Vault
- **Environment Variables**: Set `CivitaiSdk__ApiToken`
- **Docker**: Mount secrets as environment variables

## Getting an API Token

See the [Getting an API Key](../core/getting-api-key.md) guide for instructions on obtaining your Civitai API token.

> [!NOTE]
> The terms "API Key" and "API Token" refer to the same credential. Civitai uses "API Key" in their interface, while the SDK uses `ApiToken` in code for clarity.

## Next Steps

- [SDK Introduction](introduction.md) - Learn about SDK features
- [Jobs Service](jobs.md) - Submit and manage generation jobs
- [Coverage Service](coverage.md) - Check model availability
