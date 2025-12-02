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

The CivitaiSharp.Sdk library provides utilities for working with AIR identifiers.

### Parsing an AIR

```csharp
using CivitaiSharp.Sdk.Air;

var air = AirUrn.Parse("urn:air:sdxl:lora:civitai:328553@368189");

Console.WriteLine($"Ecosystem: {air.Ecosystem}");  // sdxl
Console.WriteLine($"Type: {air.Type}");            // lora
Console.WriteLine($"Source: {air.Source}");        // civitai
Console.WriteLine($"Id: {air.Id}");                // 328553
Console.WriteLine($"Version: {air.Version}");      // 368189
```

### Creating an AIR

```csharp
using CivitaiSharp.Sdk.Air;

var air = new AirUrn
{
    Ecosystem = "sdxl",
    Type = "lora",
    Source = "civitai",
    Id = "328553",
    Version = "368189"
};

Console.WriteLine(air.ToString());
// Output: urn:air:sdxl:lora:civitai:328553@368189
```

### Validating an AIR

```csharp
using CivitaiSharp.Sdk.Air;

if (AirUrn.TryParse("urn:air:sdxl:lora:civitai:328553@368189", out var air))
{
    Console.WriteLine($"Valid AIR: {air}");
}
else
{
    Console.WriteLine("Invalid AIR format");
}
```

## Common Use Cases

### Referencing Models in Applications

AIR provides a standardized way to reference models in your application configuration:

```json
{
  "models": {
    "checkpoint": "urn:air:sdxl:model:civitai:101055@128078",
    "lora": "urn:air:sdxl:lora:civitai:328553@368189"
  }
}
```

### Multi-Provider Resource References

Since AIR is a universal identifier, it can be used to reference resources across different AI model providers:

```csharp
// Civitai resource
var civitaiModel = AirUrn.Parse("urn:air:sdxl:model:civitai:101055@128078");

// Hugging Face resource
var hfModel = AirUrn.Parse("urn:air:model:huggingface:stabilityai/sdxl-vae");
```

## Next Steps

- [SDK Introduction](sdk-introduction.md) - Learn more about the CivitaiSharp SDK
- [Getting an API Key](getting-api-key.md) - Set up authentication for API access
- [Quick Start Guide](quick-start.md) - Get started with CivitaiSharp
