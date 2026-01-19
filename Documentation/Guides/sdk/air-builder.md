---
title: AIR Builder
description: Learn how to use the AirBuilder to create AIR (Artificial Intelligence Resource) identifiers with a fluent API.
---

# AIR Builder

The `AirBuilder` class provides a fluent, validated approach to constructing AIR (Artificial Intelligence Resource) identifiers for Civitai models and assets. See [AIR Identifier](air-identifier.md) for format details.

## Getting Started

### Installation

The `AirBuilder` is part of the `CivitaiSharp.Sdk` package.

```bash
dotnet add package CivitaiSharp.Sdk --prerelease
```

### Basic Usage

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#BasicUsage)]

## Builder Methods

All builder methods return a new instance (immutable pattern):

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#MethodChaining)]

### Immutability

The builder is immutable and thread-safe. Each method returns a new instance:

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#ResetReuse)]

## Validation

Validation occurs on input and at build time:

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#BuildValidation)]

## Complete Examples

### Building from Civitai Model

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#BuildFromModel)]

### Builder with Error Handling

[!code-csharp[Program.cs](examples/AirBuilder/Program.cs#BuilderErrorHandling)]

## Best Practices

1. **Reuse builders** - Create base configurations and derive specific instances
2. **Validate early** - Check input before passing to builder methods  
3. **Use method chaining** - Take advantage of the fluent API for concise code

## Related Resources

- [AIR Identifier Guide](air-identifier.md) - Understanding AIR identifiers
- [CivitaiSharp.Sdk Introduction](introduction.md) - Working with Civitai's AI orchestration platform
- [Tools Introduction](../tools/introduction.md) - Overview of CivitaiSharp.Tools utilities
