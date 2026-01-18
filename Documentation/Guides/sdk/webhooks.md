---
title: Webhook Callbacks
description: Learn how to implement and handle webhook callbacks for asynchronous job completion notifications in CivitaiSharp.Sdk.
---

# Webhook Callbacks

Webhook callbacks provide an efficient way to receive notifications when image generation jobs complete, eliminating the need for continuous polling. When a job finishes (successfully or with errors), Civitai sends an HTTP POST request to your specified webhook URL with the complete job status.

## Overview

Instead of repeatedly querying job status, you can configure a callback URL when submitting jobs. Civitai will automatically notify your endpoint when the job completes, allowing your application to process results immediately.

**Benefits:**
- **Reduced API calls** - No need for continuous polling
- **Immediate notifications** - React to completed jobs in real-time
- **Scalable architecture** - Handle high-volume job submissions efficiently
- **Resource efficient** - Lower server load and API rate limit usage

## Setting Up Webhooks

### Basic Webhook Configuration

Configure a webhook URL when creating an image generation job:

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#BasicWebhookConfiguration)]

### Webhook URL Requirements

Your webhook endpoint must:
- **Use HTTPS** - Civitai only sends callbacks to secure endpoints
- **Be publicly accessible** - Must be reachable from Civitai's servers
- **Respond within 10 seconds** - Avoid long-running operations in the webhook handler
- **Return 2xx status code** - Return 200-299 to acknowledge receipt

## Webhook Payload Structure

When a job completes, Civitai sends an HTTP POST request with the job status in JSON format. The payload structure matches the `JobStatus` model:

```json
{
  "jobId": "123e4567-e89b-12d3-a456-426614174000",
  "cost": 10.5,
  "scheduled": false,
  "result": {
    "blobKey": "abc123xyz",
    "available": true,
    "blobUrl": "https://orchestration.civitai.com/api/download/jobs/abc123xyz",
    "blobUrlExpirationDate": "2026-02-15T12:00:00Z"
  },
  "properties": {
    "userId": "12345",
    "sessionId": "session-abc"
  },
  "serviceProviders": {
    "provider": "civitai"
  },
  "position": null
}
```

### Payload Properties

| Property | Type | Description |
|----------|------|-------------|
| `jobId` | `Guid` | Unique job identifier |
| `cost` | `decimal` | Buzz cost incurred for the job |
| `scheduled` | `bool` | `false` when job is complete |
| `result` | `JobResult?` | Contains image URL and availability information |
| `result.blobUrl` | `string?` | Temporary download URL for the generated image |
| `result.blobUrlExpirationDate` | `DateTime?` | When the download URL expires |
| `result.available` | `bool` | Whether the result is ready for download |
| `properties` | `Dictionary<string, JsonElement>?` | Custom properties from your original request |
| `serviceProviders` | `JsonElement?` | Information about the service provider |
| `position` | `int?` | Queue position (null when complete) |

## Implementing a Webhook Endpoint

### ASP.NET Core Minimal API

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#MinimalApiWebhookEndpoint)]

### ASP.NET Core Controller

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#ControllerWebhookEndpoint)]

## Processing Webhook Data

### Downloading Generated Images

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#DownloadGeneratedImages)]

### Handling Custom Properties

Use custom properties to track additional context:

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#HandlingCustomProperties)]

## Security Considerations

### Validating Webhook Requests

While Civitai doesn't currently provide webhook signature verification, implement these security measures:

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#ValidatingWebhookRequests)]

### Rate Limiting

Implement rate limiting to protect against abuse:

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#RateLimiting)]

## Error Handling and Retries

### Handling Failed Jobs

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#HandlingFailedJobs)]

### Implementing Idempotency

Ensure webhook handlers are idempotent to handle duplicate deliveries:

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#ImplementingIdempotency)]

## Testing Webhooks Locally

### Using ngrok for Local Testing

During development, expose your local server to the internet:

```shell
# Install ngrok: https://ngrok.com/download

# Start your local API on port 5000
dotnet run

# In another terminal, create a tunnel
ngrok http 5000
```

ngrok will provide a public HTTPS URL (e.g., `https://abc123.ngrok-free.app`) that you can use as your webhook URL:

```csharp
var result = await sdkClient.Jobs
    .CreateImage()
    .WithAir(model)
    .WithPositivePrompt(prompt)
    .WithCallbackUrl("https://example.tld/api/webhooks/civitai")
    .ExecuteAsync();
```

### Manual Testing with Mock Payloads

Create a test endpoint to simulate webhook calls:

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#ManualTestingWithMockPayloads)]

## Complete Example

### End-to-End Webhook Implementation

[!code-csharp[Program.cs](examples/Webhooks/Program.cs#EndToEndWebhookImplementation)]

## Best Practices

1. **Always use HTTPS** - Civitai only sends webhooks to secure endpoints
2. **Respond quickly** - Return 2xx status within 10 seconds, process asynchronously
3. **Implement idempotency** - Handle duplicate webhook deliveries gracefully
4. **Validate payloads** - Check job IDs and other required fields
5. **Log everything** - Maintain detailed logs for debugging and auditing
6. **Handle failures gracefully** - Check for missing results and unavailable images
7. **Set expiration times** - Download images before blob URLs expire
8. **Use custom properties** - Track context needed for processing
9. **Test locally** - Use ngrok or similar tools during development
10. **Monitor webhook health** - Track delivery success rates and processing times

## Comparison: Webhooks vs Polling

| Aspect | Webhooks | Polling |
|--------|----------|---------|
| **Latency** | Immediate notification | Delay based on poll interval |
| **API calls** | None after submission | Continuous until completion |
| **Server load** | Minimal | Higher (constant requests) |
| **Scalability** | Excellent | Limited by rate limits |
| **Complexity** | Requires public endpoint | Simpler (client-side only) |
| **Best for** | Production, high volume | Development, low volume |

## Next Steps

- [Jobs Service Guide](jobs.md) - Complete job submission and management
- [SDK Introduction](introduction.md) - Overview of all SDK features
- [Error Handling](../core/error-handling.md) - Comprehensive error handling patterns

## Troubleshooting

**Webhook not received:**
- Verify the URL is publicly accessible via HTTPS
- Check firewall and network security group rules
- Ensure endpoint returns 2xx status code
- Review server logs for incoming requests

**Duplicate webhooks:**
- Implement idempotency using distributed cache or database
- Check for retry logic in your error handling

**Image URL expired:**
- Download images immediately upon webhook receipt
- Check `BlobUrlExpirationDate` before attempting download
- Consider implementing a queue for reliable processing
