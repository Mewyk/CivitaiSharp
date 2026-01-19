---
title: API Behavior and Quirks
description: Understand Civitai API behaviors handled by CivitaiSharp and other known quirks.
---

# API Behavior and Quirks

This guide documents how CivitaiSharp handles specific API behaviors and lists other known API quirks.

## Handled Behaviors

CivitaiSharp automatically handles several API requirements to ensure reliable requests:

### Query String Transformation
The Civitai API ignores JSON request bodies for GET requests. CivitaiSharp automatically serializes all filter and pagination parameters into the query string, so you don't need to manually verify parameter placement.

### Array Parameter Formatting
CivitaiSharp uses the repeated parameter format (e.g., `?ids=123&ids=456`) for all array parameters by default. This is the most reliable format accepted by the API, unlike comma-separated values which fail on certain endpoints.

### URL Encoding
All parameter values are automatically URL-encoded (e.g., `Highest Rated` becomes `Highest%20Rated`), ensuring compatibility with endpoints that contain special characters or spaces.

## Known API Quirks

The following behaviors are inherent to the Civitai API and should be kept in mind:

*   **Commercial Use Filter**: The `WhereCommercialUse()` filter is known to return 0 results unless at least two permission values are provided.
*   **License Filters**: Filters like `WhereAllowNoCredit` and `WhereAllowDerivatives` have been observed to return 0 results regardless of the boolean value passed.
*   **Authentication & Results**:
    *   `WhereFavorites()` and `WhereHidden()` require authentication; otherwise they behave as if no results exist.
    *   Unauthenticated requests have stricter rate limits and do not return NSFW content.
