---
title: Getting a Civitai API Key
description: Obtain a Civitai API key for authenticated requests.
---

# Getting a Civitai API Key

An API key is required for authenticated requests: favorites, hidden models, NSFW content, and all SDK operations.

## Create an API Key

1. Go to [Account Settings](https://civitai.com/user/account)
2. Navigate to **API Keys** section
3. Click **Add API key**
4. Copy the key immediately (shown only once)

Keep your API key secret. Never commit it to source control.

## Configuration

Use application settings (recommended):

[!code-csharp[](examples/GettingApiKey/Program.cs#ConfigurationFile)]

Example `appsettings.json`:

[!code-json[](examples/GettingApiKey/appsettings.json)]

For local development, use User Secrets to store the API key securely.

## What Requires Authentication

**Core Library:**
- Favorites
- Hidden models
- Higher rate limits
- NSFW content (Mature/X levels)

**SDK Library:**
- All operations (authentication always required)

## Next Steps

- [Quick Start](quick-start.md)

- [Quick Start Guide](quick-start.md) - Get started with CivitaiSharp
- [Error Handling](../common/error-handling.md) - Handle API errors gracefully
