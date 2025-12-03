---
title: API Behavior and Quirks
description: Understand Civitai API behaviors including parameter passing, rate limiting, error codes, and endpoint-specific quirks.
---

# API Behavior and Quirks

This guide documents known behaviors and quirks of the Civitai API based on extensive testing. Understanding these behaviors helps you write more reliable code and troubleshoot issues.

## Parameter Passing

### Query String Parameters Only

**All filter and pagination parameters must be passed as query string parameters.** The Civitai API completely ignores JSON request bodies on GET requests.

CivitaiSharp automatically handles this correctly - you don't need to worry about it. However, if you're debugging network requests, you'll see all parameters in the URL query string, not in the request body.

### Array Parameter Formats

When passing multiple values for the same parameter, the API supports different formats depending on the parameter:

#### Repeated Parameters (Universal)

**This format works for all array parameters:**

```
?ids=122359&ids=58390&ids=42567
```

CivitaiSharp uses this format by default as it's the most reliable.

#### Comma-Separated (Selective Support)

**Some** parameters accept comma-separated values:

```
?ids=122359,58390,42567
```

However, **other parameters return 400 Bad Request** with comma-separated values:

```
?baseModels=SD 1.5,SDXL 1.0  ❌ 400 Bad Request
```

**Recommendation:** Always use the repeated parameter format, which CivitaiSharp does automatically.

### URL Encoding

Parameters containing spaces (like "Highest Rated", "SD 1.5") are automatically URL-encoded by CivitaiSharp:

```
?sort=Highest%20Rated
?baseModels=SD%201.5
```

The API accepts both encoded and unencoded spaces, but encoding is safer for reliability.

## Authentication Requirements

Some filters require authentication via an API key configured in `ApiClientOptions.ApiKey`:

| Filter Method | Requires Auth | Behavior Without Auth |
|---------------|---------------|----------------------|
| `WhereFavorites()` | ✅ Yes | Returns 0 results |
| `WhereHidden()` | ✅ Yes | Returns 0 results |
| All other filters | ❌ No | Works normally |

### Configuring Authentication

```csharp
services.AddCivitaiApi(options =>
{
    options.ApiKey = "your-api-key-here";
});
```

Without authentication:
- You can still query all public models, images, tags, and creators
- Rate limits are lower (more restrictive)
- Favorites and hidden model filters return empty results

## Known Parameter Quirks

### Commercial Use Permissions

The `WhereCommercialUse()` filter exhibits unusual behavior:

```csharp
// ❌ Single value typically returns 0 results
var result = await apiClient.Models
    .WhereCommercialUse(CommercialUsePermission.None)
    .ExecuteAsync();

// ✅ Multiple values work correctly
var result = await apiClient.Models
    .WhereCommercialUse(
        CommercialUsePermission.Image, 
        CommercialUsePermission.Sell)
    .ExecuteAsync();
```

**Behavior:** The API requires at least two permission values to return results. This may reflect the API's data model or filtering logic.

**Workaround:** Always provide at least two permission values when filtering by commercial use.

### License Filters

During testing, these boolean filters showed inconsistent behavior:

| Filter Method | Observed Behavior |
|---------------|-------------------|
| `WhereAllowNoCredit(bool)` | Returns 0 results for both `true` and `false` |
| `WhereAllowDerivatives(bool)` | Returns 0 results for both `true` and `false` |

**Possible Causes:**
- API-side data conditions not met
- Requires authentication (not confirmed)
- Requires combination with other filters
- API implementation issue

**Recommendation:** Use these filters with caution. Test thoroughly in your specific use case and verify results.

### All Other Boolean Filters

These boolean filters work correctly:

- `WhereNsfw(bool)` - ✅ Works for both true and false
- `WherePrimaryFileOnly()` - ✅ Works (always true)
- `WhereAllowDifferentLicenses(bool)` - ✅ Works for both true and false
- `WhereSupportsGeneration(bool)` - ✅ Works for both true and false

## Endpoint Reliability

### Creators Endpoint

The `/api/v1/creators` endpoint has shown reliability issues:

**Observed Problems:**
- Request timeouts (10+ seconds with no response)
- HTTP 500 Internal Server Error responses
- Higher failure rate compared to other endpoints

**Example Test Results:**
```
✅ Models endpoint: 100% success rate
✅ Images endpoint: 100% success rate
✅ Tags endpoint: 100% success rate
❌ Creators endpoint: ~50% success rate (timeouts/500 errors)
```

**Mitigation:**
- CivitaiSharp includes automatic retry via resilience policies
- Default timeout is 30 seconds
- Consider increasing timeout for this endpoint if needed

```csharp
services.AddCivitaiApi(options =>
{
    options.TimeoutSeconds = 60; // Increase for problematic endpoints
});
```

## Enum Sort Values

### Values with Spaces

Sort enum values that map to strings with spaces work correctly when URL-encoded:

```csharp
// "Highest Rated" → URL-encoded as "Highest%20Rated"
var result = await apiClient.Models
    .OrderBy(ModelSort.HighestRated)
    .ExecuteAsync();
```

**Supported Sort Values:**

| Enum Value | API String | Encoding Required |
|------------|------------|-------------------|
| `ModelSort.HighestRated` | "Highest Rated" | ✅ Yes |
| `ModelSort.MostDownloaded` | "Most Downloaded" | ✅ Yes |
| `ModelSort.Newest` | "Newest" | ❌ No (single word) |
| `ImageSort.MostReactions` | "Most Reactions" | ✅ Yes |
| `ImageSort.MostComments` | "Most Comments" | ✅ Yes |
| `ImageSort.MostCollected` | "Most Collected" | ✅ Yes |

CivitaiSharp automatically handles all URL encoding.

## Testing Results Summary

Based on comprehensive testing on November 30, 2025:

| Test Category | Tests Run | Pass Rate | Notes |
|---------------|-----------|-----------|-------|
| Query String Parameters | 84 | 95.2% | 4 failures were creators endpoint issues |
| Boolean Filters | 14 | 85.7% | `allowNoCredit` and `allowDerivatives` returned 0 results |
| Enum Parameters | 18 | 100% | All enum values work correctly |
| Array Parameters | 12 | 100% | Repeated parameter format works for all |
| Authentication Filters | 4 | 100% | Work correctly with valid API key |
| Comma-Separated Arrays | 4 | 50% | Only `ids` supports this format |

## Best Practices

### Filter Composition

Build filters incrementally and test each addition:

```csharp
var builder = apiClient.Models
    .WhereType(ModelType.Lora)
    .OrderBy(ModelSort.HighestRated);

// Test base filter
var baseResult = await builder.ExecuteAsync();

// Add more filters after confirming base works
var refinedResult = await builder
    .WherePeriod(TimePeriod.Month)
    .ExecuteAsync(resultsLimit: 10);
```

### Error Handling

Always handle potential errors, especially for the creators endpoint:

```csharp
var result = await apiClient.Creators
    .WhereName("artist")
    .ExecuteAsync();

if (result is Result<PagedResult<Creator>>.Failure failure)
{
    if (failure.Error.Code == ErrorCode.Timeout)
    {
        // Retry with longer timeout or different approach
    }
    else if (failure.Error.Code == ErrorCode.ServerError)
    {
        // Log and handle server errors
    }
}
```

### Verify Results

For filters with known quirks, verify results match expectations:

```csharp
var result = await apiClient.Models
    .WhereCommercialUse(CommercialUsePermission.Image)
    .ExecuteAsync();

if (result is Result<PagedResult<Model>>.Success success)
{
    if (success.Data.Items.Count == 0)
    {
        // May indicate the single-value quirk
        // Retry with multiple values
    }
}
```

## Reporting Issues

If you discover additional API quirks or inconsistencies:

1. Test against the live Civitai API directly to confirm behavior
2. Document test conditions (authentication, parameters, date/time)
3. Report findings via GitHub Issues with test results
4. Include API response examples when possible

## Next Steps

- [Error Handling](error-handling.md) - Handle API errors gracefully
- [Configuration](configuration.md) - Configure timeout and authentication
- [Working with Models](models.md) - Model query examples
