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

```csharp
var result = await sdkClient.Jobs
    .CreateImage()
    .WithAir(new AirIdentifier("sdxl", AirAssetType.Checkpoint, "civitai", 4201, 130072))
    .WithPositivePrompt("beautiful landscape at sunset")
    .WithDimensions(1024, 1024)
    .WithCallbackUrl("https://domain.tld/api/webhooks/civitai")
    .ExecuteAsync();

if (result is Result<JobStatusCollection>.Success success)
{
    Console.WriteLine($"Job submitted with webhook: {success.Data.Token}");
}
```

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

```csharp
using System.Text.Json;
using CivitaiSharp.Sdk.Models.Results;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/api/webhooks/civitai", async (
    [FromBody] JobStatus jobStatus,
    [FromServices] IJobProcessingService jobProcessor) =>
{
    try
    {
        // Log the webhook receipt
        app.Logger.LogInformation(
            "Received webhook for job {JobId} with status scheduled={Scheduled}",
            jobStatus.JobId,
            jobStatus.Scheduled);

        // Check if job completed successfully
        if (!jobStatus.Scheduled && jobStatus.Result?.BlobUrl is string imageUrl)
        {
            // Process the completed job
            await jobProcessor.ProcessCompletedJobAsync(jobStatus);
            
            app.Logger.LogInformation(
                "Successfully processed job {JobId}. Image URL: {ImageUrl}",
                jobStatus.JobId,
                imageUrl);
        }
        else if (!jobStatus.Scheduled && jobStatus.Result is null)
        {
            // Job failed
            app.Logger.LogWarning(
                "Job {JobId} completed but has no result (likely failed)",
                jobStatus.JobId);
        }

        // Return 200 OK to acknowledge receipt
        return Results.Ok(new { received = true, jobId = jobStatus.JobId });
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error processing webhook for job {JobId}", jobStatus.JobId);
        
        // Still return 200 to prevent retries for processing errors
        // Use a different status code (e.g., 500) if you want Civitai to retry
        return Results.Ok(new { received = true, error = "Processing failed" });
    }
});

app.Run();
```

### ASP.NET Core Controller

```csharp
using Microsoft.AspNetCore.Mvc;
using CivitaiSharp.Sdk.Models.Results;

[ApiController]
[Route("api/webhooks")]
public class CivitaiWebhookController : ControllerBase
{
    private readonly ILogger<CivitaiWebhookController> _logger;
    private readonly IJobProcessingService _jobProcessor;

    public CivitaiWebhookController(
        ILogger<CivitaiWebhookController> logger,
        IJobProcessingService jobProcessor)
    {
        _logger = logger;
        _jobProcessor = jobProcessor;
    }

    [HttpPost("civitai")]
    public async Task<IActionResult> HandleCivitaiWebhook([FromBody] JobStatus jobStatus)
    {
        _logger.LogInformation(
            "Webhook received for job {JobId}",
            jobStatus.JobId);

        try
        {
            // Process the webhook asynchronously
            await _jobProcessor.ProcessCompletedJobAsync(jobStatus);
            
            return Ok(new { received = true, jobId = jobStatus.JobId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process webhook for job {JobId}", jobStatus.JobId);
            return StatusCode(500, new { error = "Processing failed" });
        }
    }
}
```

## Processing Webhook Data

### Downloading Generated Images

```csharp
public class JobProcessingService : IJobProcessingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<JobProcessingService> _logger;

    public JobProcessingService(
        IHttpClientFactory httpClientFactory,
        ILogger<JobProcessingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task ProcessCompletedJobAsync(JobStatus jobStatus)
    {
        if (jobStatus.Result?.BlobUrl is not string imageUrl)
        {
            _logger.LogWarning("Job {JobId} has no image URL", jobStatus.JobId);
            return;
        }

        // Check if the URL is still valid
        if (jobStatus.Result.BlobUrlExpirationDate.HasValue &&
            jobStatus.Result.BlobUrlExpirationDate.Value < DateTime.UtcNow)
        {
            _logger.LogWarning(
                "Job {JobId} image URL expired at {ExpirationDate}",
                jobStatus.JobId,
                jobStatus.Result.BlobUrlExpirationDate.Value);
            return;
        }

        // Download the image
        var httpClient = _httpClientFactory.CreateClient();
        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

        // Save to storage
        var fileName = $"{jobStatus.JobId}.png";
        await File.WriteAllBytesAsync($"output/{fileName}", imageBytes);

        _logger.LogInformation(
            "Downloaded image for job {JobId}: {FileName} ({Size} bytes)",
            jobStatus.JobId,
            fileName,
            imageBytes.Length);

        // Process custom properties if provided
        if (jobStatus.Properties is not null)
        {
            foreach (var (key, value) in jobStatus.Properties)
            {
                _logger.LogInformation(
                    "Job {JobId} property {Key}: {Value}",
                    jobStatus.JobId,
                    key,
                    value.ToString());
            }
        }
    }
}
```

### Handling Custom Properties

Use custom properties to track additional context:

```csharp
// When submitting the job
var result = await sdkClient.Jobs
    .CreateImage()
    .WithAir(model)
    .WithPositivePrompt(prompt)
    .WithProperty("userId", JsonSerializer.SerializeToElement(userId))
    .WithProperty("orderId", JsonSerializer.SerializeToElement(orderId))
    .WithProperty("category", JsonSerializer.SerializeToElement("portrait"))
    .WithCallbackUrl("https://domain.tld/api/webhooks/civitai")
    .ExecuteAsync();

// In the webhook handler
public async Task ProcessCompletedJobAsync(JobStatus jobStatus)
{
    if (jobStatus.Properties is null) return;

    // Extract custom properties
    if (jobStatus.Properties.TryGetValue("userId", out var userIdElement))
    {
        var userId = userIdElement.GetString();
        await NotifyUserAsync(userId, jobStatus);
    }

    if (jobStatus.Properties.TryGetValue("orderId", out var orderIdElement))
    {
        var orderId = orderIdElement.GetInt32();
        await UpdateOrderStatusAsync(orderId, jobStatus);
    }

    if (jobStatus.Properties.TryGetValue("category", out var categoryElement))
    {
        var category = categoryElement.GetString();
        await CategorizeImageAsync(category, jobStatus);
    }
}
```

## Security Considerations

### Validating Webhook Requests

While Civitai doesn't currently provide webhook signature verification, implement these security measures:

```csharp
[HttpPost("civitai")]
public async Task<IActionResult> HandleCivitaiWebhook(
    [FromBody] JobStatus jobStatus,
    [FromHeader(Name = "User-Agent")] string? userAgent)
{
    // Basic validation
    if (jobStatus.JobId == Guid.Empty)
    {
        _logger.LogWarning("Received webhook with invalid job ID");
        return BadRequest("Invalid job ID");
    }

    // Optional: Check User-Agent (Civitai typically identifies itself)
    if (string.IsNullOrEmpty(userAgent) || !userAgent.Contains("civitai", StringComparison.OrdinalIgnoreCase))
    {
        _logger.LogWarning("Received webhook with unexpected User-Agent: {UserAgent}", userAgent);
        // Decide whether to reject or just log
    }

    // Optional: Verify the job ID exists in your database
    var expectedJob = await _database.GetJobAsync(jobStatus.JobId);
    if (expectedJob is null)
    {
        _logger.LogWarning("Received webhook for unknown job {JobId}", jobStatus.JobId);
        return NotFound("Job not found");
    }

    // Process the webhook
    await _jobProcessor.ProcessCompletedJobAsync(jobStatus);
    return Ok();
}
```

### Rate Limiting

Implement rate limiting to protect against abuse:

```csharp
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("webhook", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });
});

var app = builder.Build();
app.UseRateLimiter();

app.MapPost("/api/webhooks/civitai", async (JobStatus jobStatus) =>
{
    // Handler implementation
})
.RequireRateLimiting("webhook");

app.Run();
```

## Error Handling and Retries

### Handling Failed Jobs

```csharp
public async Task ProcessCompletedJobAsync(JobStatus jobStatus)
{
    // Check if the job failed (completed but no result)
    if (!jobStatus.Scheduled && jobStatus.Result is null)
    {
        _logger.LogError("Job {JobId} failed (no result available)", jobStatus.JobId);
        
        // Notify the user or trigger a retry
        await HandleFailedJobAsync(jobStatus.JobId);
        return;
    }

    // Check if result is not yet available
    if (jobStatus.Result is { Available: false })
    {
        _logger.LogWarning(
            "Job {JobId} completed but result not yet available",
            jobStatus.JobId);
        
        // Queue for later processing
        await QueueForRetryAsync(jobStatus.JobId);
        return;
    }

    // Process successful job
    if (jobStatus.Result?.BlobUrl is string imageUrl)
    {
        await DownloadAndProcessImageAsync(jobStatus.JobId, imageUrl);
    }
}
```

### Implementing Idempotency

Ensure webhook handlers are idempotent to handle duplicate deliveries:

```csharp
public class JobProcessingService : IJobProcessingService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<JobProcessingService> _logger;

    public async Task ProcessCompletedJobAsync(JobStatus jobStatus)
    {
        var cacheKey = $"processed-job-{jobStatus.JobId}";
        
        // Check if already processed
        var processed = await _cache.GetStringAsync(cacheKey);
        if (processed is not null)
        {
            _logger.LogInformation(
                "Job {JobId} already processed, skipping",
                jobStatus.JobId);
            return;
        }

        try
        {
            // Process the job
            await DownloadAndProcessImageAsync(jobStatus);

            // Mark as processed (expires after 24 hours)
            await _cache.SetStringAsync(
                cacheKey,
                "processed",
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process job {JobId}", jobStatus.JobId);
            throw;
        }
    }
}
```

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

```csharp
#if DEBUG
app.MapPost("/api/test/simulate-webhook", async (
    [FromServices] IJobProcessingService jobProcessor) =>
{
    var mockJobStatus = new JobStatus(
        JobId: Guid.NewGuid(),
        Cost: 10.5m,
        Result: new JobResult(
            BlobKey: "test-key",
            Available: true,
            BlobUrl: "https://domain.tld/test-image.png",
            BlobUrlExpirationDate: DateTime.UtcNow.AddHours(1)
        ),
        Scheduled: false,
        Properties: new Dictionary<string, JsonElement>
        {
            ["userId"] = JsonSerializer.SerializeToElement("test-user-123"),
            ["category"] = JsonSerializer.SerializeToElement("test")
        },
        ServiceProviders: null,
        Position: null
    );

    await jobProcessor.ProcessCompletedJobAsync(mockJobStatus);
    return Results.Ok("Webhook simulation complete");
});
#endif
```

## Complete Example

### End-to-End Webhook Implementation

```csharp
using CivitaiSharp.Sdk;
using CivitaiSharp.Sdk.Extensions;
using CivitaiSharp.Sdk.Models.Results;
using CivitaiSharp.Sdk.Air;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddCivitaiSdk(options =>
{
    options.ApiToken = builder.Configuration["Civitai:ApiToken"]!;
});
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IJobProcessingService, JobProcessingService>();

var app = builder.Build();

// Webhook endpoint
app.MapPost("/api/webhooks/civitai", async (
    [FromBody] JobStatus jobStatus,
    [FromServices] IJobProcessingService jobProcessor,
    ILogger<Program> logger) =>
{
    logger.LogInformation("Webhook received for job {JobId}", jobStatus.JobId);

    try
    {
        await jobProcessor.ProcessCompletedJobAsync(jobStatus);
        return Results.Ok(new { received = true });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Webhook processing failed");
        return Results.StatusCode(500);
    }
});

// Submit job endpoint
app.MapPost("/api/jobs/submit", async (
    [FromBody] JobRequest request,
    [FromServices] ISdkClient sdkClient,
    IConfiguration configuration) =>
{
    var webhookUrl = configuration["Civitai:WebhookUrl"]!;
    
    var result = await sdkClient.Jobs
        .CreateImage()
        .WithAir(new AirIdentifier("sdxl", AirAssetType.Checkpoint, "civitai", 4201, 130072))
        .WithPositivePrompt(request.Prompt)
        .WithDimensions(1024, 1024)
        .WithProperty("userId", JsonSerializer.SerializeToElement(request.UserId))
        .WithCallbackUrl(webhookUrl)
        .ExecuteAsync();

    return result switch
    {
        Result<JobStatusCollection>.Success success => 
            Results.Ok(new { token = success.Data.Token }),
        Result<JobStatusCollection>.Failure failure => 
            Results.BadRequest(new { error = failure.Error.Message }),
        _ => Results.StatusCode(500)
    };
});

app.Run();

public record JobRequest(string Prompt, string UserId);
```

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

- [Jobs Service Guide](sdk-jobs.md) - Complete job submission and management
- [SDK Introduction](sdk-introduction.md) - Overview of all SDK features
- [Error Handling](error-handling.md) - Comprehensive error handling patterns

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
