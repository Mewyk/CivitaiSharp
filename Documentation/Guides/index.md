---
title: Documentation
description: Comprehensive guides and tutorials for CivitaiSharp - the .NET client library for Civitai.com
---

# Documentation

Welcome to the CivitaiSharp documentation. These guides will help you get started and make the most of the library.

## Getting Started

New to CivitaiSharp? Start here:

| Guide | Description |
|-------|-------------|
| [Installation](installation.md) | Install CivitaiSharp via NuGet |
| [Getting an API Key](core/getting-api-key.md) | Obtain your Civitai API key |
| [Quick Start](core/quick-start.md) | Get up and running in minutes |

## AI Resource Identifier (AIR)

Understand and work with AIR identifiers for model references:

| Guide | Description |
|-------|-------------|
| [AIR Overview](sdk/air-identifier.md) | Parse, validate, and work with AIR identifiers |
| [AIR Builder](sdk/air-builder.md) | Fluent builder pattern for constructing AIR identifiers |

## Core Library

The Core library provides low-level access to the Civitai Public API with fluent request builders.

| Guide | Description |
|-------|-------------|
| [Introduction](core/introduction.md) | Overview of CivitaiSharp.Core |
| [Request Builders](core/request-builders.md) | Build type-safe API queries |
| [Models](core/models.md) | Query and filter AI models |
| [Images](core/images.md) | Search generated images |
| [Tags](core/tags.md) | Browse model tags |
| [Creators](core/creators.md) | Find content creators |
| [Error Handling](core/error-handling.md) | Handle errors with the Result pattern |
| [Pagination](core/pagination.md) | Navigate paginated results |
| [API Behavior and Quirks](core/api-quirks.md) | Known API behaviors and workarounds |

## SDK Library

The SDK provides high-level abstractions for image generation and advanced workflows.

| Guide | Description |
|-------|-------------|
| [SDK Introduction](sdk/introduction.md) | Overview of CivitaiSharp.Sdk |
| [Jobs Service](sdk/jobs.md) | Submit and manage image generation jobs |
| [Coverage Service](sdk/coverage.md) | Check model availability before job submission |
| [Usage Service](sdk/usage.md) | Monitor resource usage and limits |

## Tools Library

The Tools library provides utilities for file hashing, downloading, and HTML parsing.

| Guide | Description |
|-------|-------------|
| [Tools Introduction](tools/introduction.md) | Overview of CivitaiSharp.Tools |
| [File Hashing](tools/file-hashing.md) | Compute SHA256, SHA512, BLAKE3, and CRC32 hashes |
| [Downloading Files](tools/downloading-files.md) | Download images and models with path patterns |
| [HTML Parsing](tools/html-parsing.md) | Convert descriptions to Markdown or plain text |
