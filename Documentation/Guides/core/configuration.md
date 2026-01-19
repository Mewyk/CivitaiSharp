---
title: Configuration
description: Configure CivitaiSharp API clients with options for API keys, base URLs, timeouts, and resilience policies.
---

# Configuration

This guide covers all configuration options for the CivitaiSharp API clients.

## CivitaiSharp.Core Configuration

The `ApiClientOptions` class configures the low-level API client.

### Automatic Configuration (Best Practice)

> [!TIP]
> Automatic configuration from `appsettings.json` is the recommended approach. It keeps your code clean, secure, and follows .NET configuration standards.

Configure options in `appsettings.json`:

[!code-json[appsettings.json](examples/Configuration/appsettings.json)]

Then register the services:

[!code-csharp[Program.cs](examples/Configuration/Program.cs#IConfiguration)]

Available options: `ApiKey` (optional, for authenticated features), `TimeoutSeconds` (default 30, range 1-300).

> [!IMPORTANT]
> **Never commit API keys to source control.** Use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local development or [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/overview) for production.

### Manual Configuration (Not Recommended)

> [!WARNING]
> Manual configuration with hardcoded values is not recommended. Use automatic configuration from `appsettings.json` instead.

If you must configure manually:

[!code-csharp[Program.cs](../examples/Common/Program.cs#CoreSetupWithApiKey)]

## Next Steps

- [Request Builders](request-builders.md) - Learn the fluent API pattern
- [Working with Models](models.md) - Query and filter models
