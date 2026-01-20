---
title: API Behavior and Quirks
description: Known Civitai API behaviors and workarounds.
---

# API Behavior and Quirks

## Handled Automatically

**Query String Transformation:** All parameters serialized to query string (API ignores JSON bodies)

**Array Parameters:** Repeated format (`?ids=123&ids=456`) used for reliability

**URL Encoding:** All values automatically encoded

## Known API Issues

**Commercial Use Filter:** Returns 0 results unless at least two permissions provided

**License Filters:** `WhereAllowNoCredit` and `WhereAllowDerivatives` return 0 results

**Authentication:** `WhereFavorites()` and `WhereHidden()` require authentication; unauthenticated requests have stricter rate limits and no NSFW content
