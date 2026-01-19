---
title: Getting a Civitai API Key
description: Learn how to obtain a Civitai API key for authenticated requests, accessing favorites, hidden models, NSFW content, and higher rate limits.
---

# Getting a Civitai.com API Key

This guide walks you through the process of obtaining an API key from Civitai. An API key is required for authenticated requests, including accessing your favorites, hidden models, NSFW content (Mature/X levels), and getting higher rate limits.

## Prerequisites

- A Civitai account. If you don't have one, visit [civitai.com](https://civitai.com) and sign up.

## Create an API Key

1. Navigate to your [Account Settings](https://civitai.com/user/account) page on Civitai
2. Select your profile icon in the top right corner
3. Click **Account settings** from the dropdown menu
4. Scroll down to find the **API Keys** section
5. Click **Add API key** to generate a new key

<!-- TODO: Add screenshot once available
![API key management section in profile](Images/api-key-generation.png)
-->

> [!NOTE]
> The API key works for both the public REST API and CivitaiSharp.

After creating the key, it will be displayed once. **Copy it immediately** and store it securely. You won't be able to see the full key again after leaving this page.

> [!WARNING]
> **Keep your API key secret!** Never commit your API key to source control or share it publicly. Anyone with your API key can make requests on your behalf.

## Using Your API Key

Once you have your API key, configure CivitaiSharp using one of these methods:

- **Options configuration** - Directly in code via `AddCivitaiApi(options => { options.ApiKey = "your-key"; })`
- **Configuration file** - From appsettings.json
- **Environment variables**
- **User secrets** - For development

## Managing API Keys

You can create multiple API keys for different applications or purposes. To revoke a key:

1. Go back to your [Account Settings](https://civitai.com/user/account)
2. Navigate to the API Keys section
3. Find the key you want to revoke
4. Click the delete/revoke button next to it

## What Requires an API Key?

### CivitaiSharp.Core (Public API)

| Feature | API Key Required |
|---------|------------------|
| Public model queries | No |
| Public image queries | No |
| Tag and creator queries | No |
| Favorites | **Yes** |
| Hidden models | **Yes** |
| Higher rate limits | **Yes** |

### CivitaiSharp.Sdk (Generator API)

| Feature | API Token Required |
|---------|--------------------|
| All SDK operations | **Yes** |

> [!IMPORTANT]
> The SDK (Generator/Orchestration API) **always requires authentication**. You cannot use CivitaiSharp.Sdk without providing an `ApiToken`. This is different from the Core library which supports anonymous access for public endpoints.

## Troubleshooting

### "Unauthorized" Error

If you receive an `Unauthorized` error:

- Verify your API key is correct and hasn't been revoked
- Check that the key is properly configured in your application
- Ensure there are no extra spaces or characters in the key

### Rate Limiting

If you're being rate limited even with an API key:

- Check the `RetryAfter` value in the error response
- Implement exponential backoff in your requests
- Consider caching responses where appropriate

## Next Steps

- [Quick Start Guide](quick-start.md) - Get started with CivitaiSharp
- [Error Handling](../common/error-handling.md) - Handle API errors gracefully
