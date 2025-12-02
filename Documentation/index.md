---
_layout: landing
---

# CivitaiSharp

A modern, lightweight, and AOT-ready .NET 10 client library for all things Civitai.com.

Build powerful AI model discovery and generation tools with type-safe, fluent APIs.

[Get Started](Guides/installation.md){.btn .btn-primary .btn-lg .me-2}
[API Reference](Api/){.btn .btn-secondary .btn-lg}
[GitHub](https://github.com/Mewyk/CivitaiSharp){.btn .btn-outline-secondary .btn-lg}

```csharp
var result = await client.Models
    .WhereType(ModelType.Checkpoint)
    .OrderBy(ModelSort.MostDownloaded)
    .WithResultsLimit(10)
    .ExecuteAsync();
```

---

## Features

:::row:::
:::column span="4":::
### Type-Safe API
Strongly-typed models and builders with full IntelliSense support. No magic strings or runtime errors.
:::column-end:::
:::column span="4":::
### Fluent Interface
Intuitive, chainable query construction that reads like natural language.
:::column-end:::
:::column span="4":::
### Result Pattern
Explicit error handling without exceptions. Know exactly what can fail and handle it gracefully.
:::column-end:::
:::row-end:::

:::row:::
:::column span="4":::
### Immutable Design
Thread-safe, cacheable builders that can be shared across your application.
:::column-end:::
:::column span="4":::
### Native AOT Ready
Full support for ahead-of-time compilation for fast startup and reduced memory.
:::column-end:::
:::column span="4":::
### Zero Dependencies
No external .NET dependencies required. Built entirely on Microsoft libraries for maximum compatibility.
:::column-end:::
:::row-end:::

---

## Libraries

:::row:::
:::column span="6":::
### CivitaiSharp.Core
Low-level, typed client with fluent request builders for direct API access. Query models, images, tags, and creators with full control.

[Learn More](Guides/introduction.md)
:::column-end:::
:::column span="6":::
### CivitaiSharp.Sdk
High-level SDK with simplified abstractions for image generation jobs, coverage checking, and usage tracking.

[Learn More](Guides/sdk-introduction.md)
:::column-end:::
:::row-end:::

---

## Installation

Install via NuGet:

```shell
dotnet add package CivitaiSharp.Core
```

[View all packages](Guides/installation.md)

---

## Resources

- [GitHub Repository](https://github.com/Mewyk/CivitaiSharp)
- [NuGet Packages](https://www.nuget.org/packages?q=CivitaiSharp)
- [Civitai API Documentation](https://developer.civitai.com/)

---

## Legal Notice

CivitaiSharp is an independent open-source project and is not affiliated with, sponsored by, endorsed by, or officially associated with Civitai.com or Civitai, Inc. The Civitai name and any related trademarks are the property of their respective owners. Use of these names is for identification purposes only and does not imply any endorsement or partnership.

---

<p style="text-align: center; color: #888;">
CivitaiSharp is released under the MIT License.
</p>
