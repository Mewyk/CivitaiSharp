---
title: AI Resource Identifier (AIR)
description: URN system for identifying AI resources.
---

# AI Resource Identifier (AIR)

Uniform Resource Naming system for identifying AI resources across platforms (Civitai, Hugging Face, etc.).

## Format

```
urn:air:{ecosystem}:{type}:{source}:{id}@{version?}:{layer?}.?{format?}
```

**Required:** `urn`, `air`, `ecosystem`, `type`, `source`, `id`

**Optional:** `version`, `layer`, `format`

## Examples

```
urn:air:sd1:model:civitai:2421@43533
urn:air:sdxl:lora:civitai:328553@368189
urn:air:dalle:model:openai:dalle@2
urn:air:model:huggingface:stabilityai/sdxl-vae
```

## Usage

**Parse:**
[!code-csharp[Program.cs](examples/AirIdentifier/Program.cs#ParseAir)]

**Create:**
[!code-csharp[Program.cs](examples/AirIdentifier/Program.cs#CreateAir)]

**Builder:**
[!code-csharp[Program.cs](examples/AirIdentifier/Program.cs#BuilderPattern)]

**Validate:**
[!code-csharp[Program.cs](examples/AirIdentifier/Program.cs#ValidateAir)]

## Supported Sources

- `AirSource.Civitai`
- `AirSource.HuggingFace`
- `AirSource.OpenAi`
- `AirSource.Leonardo`

## Enabling AIR on Civitai

1. Go to [Account Settings](https://civitai.com/user/account)
2. Find **Browsing settings**
3. Toggle **AI Resource Identifier**

## Next Steps

- [Create a Job](create-job.md)
- [SDK Introduction](introduction.md)
