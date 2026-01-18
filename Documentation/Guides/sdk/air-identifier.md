---
title: AI Resource Identifier (AIR)
description: Understand the AIR URN system for identifying AI resources like models, LoRAs, and embeddings across platforms like Civitai and Hugging Face.
---

# AI Resource Identifier (AIR)

The AI Resource Identifier (AIR) is a Uniform Resource Naming system for identifying AI resources like models, LoRAs, embeddings, and more across different platforms and ecosystems.

## What is AIR?

In response to challenges communicating what resource service providers and users should use when working with AI content, Civitai proposed a Uniform Resource Naming system called AIR (Artificial Intelligence Resource). This provides a standardized way to reference AI resources across platforms like Civitai, Hugging Face, and others.

For more information about Uniform Resource Names, see [Wikipedia: Uniform Resource Name](https://en.wikipedia.org/wiki/Uniform_Resource_Name).

## Enabling AIR Display on Civitai

To see AIR identifiers on Civitai model pages:

1. Navigate to your [Account Settings](https://civitai.com/user/account) page
2. Find the **Browsing settings** section
3. Toggle on the **AI Resource Identifier** option

<!-- TODO: Add screenshot once available
![Browsing settings toggles for AIR](Images/air-toggle.png)
-->

Once enabled, you'll see the AIR identifier displayed on model pages.

<!-- TODO: Add screenshot once available
![Find AIR on model page](Images/air-model-page.png)
-->

## AIR Specification

The AIR format follows this structure:

```
urn:air:{ecosystem}:{type}:{source}:{id}@{version?}:{layer?}.?{format?}
```

### Components

| Component | Description | Required |
|-----------|-------------|----------|
| `urn` | Uniform Resource Name prefix | Yes |
| `air` | Artificial Intelligence Resource identifier | Yes |
| `{ecosystem}` | Type of the ecosystem (`sd1`, `sd2`, `sdxl`, `flux1`, etc.) | Yes |
| `{type}` | Type of the resource (`model`, `lora`, `embedding`, `hypernet`) | Yes |
| `{source}` | Supported network source (e.g., `civitai`, `huggingface`, `openai`) | Yes |
| `{id}` | ID of the resource from the source | Yes |
| `{version}` | Specific version of the resource | No |
| `{layer}` | The specific layer of a model | No |
| `{format}` | The format of the model (`safetensor`, `ckpt`, `diffuser`, `tensor rt`) | No |

### Examples

```
urn:air:sd1:model:civitai:2421@43533
urn:air:sd2:model:civitai:2421@43533
urn:air:sdxl:lora:civitai:328553@368189
urn:air:dalle:model:openai:dalle@2
urn:air:gpt:model:openai:gpt@4
urn:air:model:huggingface:stabilityai/sdxl-vae
urn:air:model:leonardo:345435
```

## Using AIR with CivitaiSharp

The CivitaiSharp.Sdk library provides strongly-typed utilities for working with AIR identifiers.

### Parsing an AIR

[!code-csharp[Program.cs](examples/AirIdentifier/Program.cs#ParseAir)]

### Creating an AIR

[!code-csharp[Program.cs](examples/AirIdentifier/Program.cs#CreateAir)]

### Using the Builder Pattern

For more complex scenarios, use the fluent builder:

[!code-csharp[Program.cs](examples/AirIdentifier/Program.cs#BuilderPattern)]

### Validating an AIR

[!code-csharp[Program.cs](examples/AirIdentifier/Program.cs#ValidateAir)]

### Supported Sources

CivitaiSharp supports four platforms as defined in the official AIR specification:

| Source | Enum Value | Description |
|--------|------------|-------------|
| Civitai | `AirSource.Civitai` | Civitai platform resources |
| Hugging Face | `AirSource.HuggingFace` | Hugging Face model hub |
| OpenAI | `AirSource.OpenAi` | OpenAI models (DALL-E, GPT) |
| Leonardo | `AirSource.Leonardo` | Leonardo.Ai platform |

All sources are strongly typed as enums, providing compile-time safety and IDE intellisense.

## Common Use Cases

### Referencing Models in Applications

AIR provides a standardized way to reference models in your application configuration:

[!code-json[appsettings.json](examples/AirIdentifier/appsettings.json)]

### Multi-Provider Resource References

Since AIR is a universal identifier, it can be used to reference resources across different AI model providers:

[!code-csharp[Program.cs](examples/AirIdentifier/Program.cs#multi-provider)]

## Next Steps

- [SDK Introduction](introduction.md) - Learn more about the CivitaiSharp SDK
- [Getting an API Key](../core/getting-api-key.md) - Set up authentication for API access
- [Quick Start Guide](../core/quick-start.md) - Get started with CivitaiSharp
