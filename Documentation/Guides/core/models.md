---
title: Working with Models
description: Query AI models from Civitai.
---

# Working with Models

Query AI models: checkpoints, LoRAs, embeddings, VAEs, ControlNets, and more.

## Model Types

`Checkpoint`, `Lora`, `TextualInversion`, `Controlnet`, `Hypernetwork`, `AestheticGradient`, `Vae`, `Poses`, `Wildcards`, `MotionModule`, `Upscaler`, `Workflows`

## Query Examples

**By Type:**
[!code-csharp[Program.cs](examples/Models/Program.cs#ByType)]

**By Tag:**
[!code-csharp[Program.cs](examples/Models/Program.cs#ByTag)]

**By Creator:**
[!code-csharp[Program.cs](examples/Models/Program.cs#ByCreator)]

**By Name:**
[!code-csharp[Program.cs](examples/Models/Program.cs#ByName)]

**By ID:**
[!code-csharp[Program.cs](examples/Models/Program.cs#ById)]

**By Version ID:**
[!code-csharp[Program.cs](examples/Models/Program.cs#ByVersionId)]

**By Hash:**
[!code-csharp[Program.cs](examples/Models/Program.cs#ByVersionHash)]

## Model Versions

Access version metadata:
[!code-csharp[Program.cs](examples/Models/Program.cs#VersionSpecificInformation)]

## Filtering

**Permissions:**
[!code-csharp[Program.cs](examples/Models/Program.cs#Permissions)]

**Favorites (requires auth):**
[!code-csharp[Program.cs](examples/Models/Program.cs#Favorites)]

**Hidden (requires auth):**
[!code-csharp[Program.cs](examples/Models/Program.cs#HiddenModels)]

## Sorting

[!code-csharp[Program.cs](examples/Models/Program.cs#Sorting)]

**Options:** `HighestRated`, `MostDownloaded`, `Newest`

**Periods:** `Day`, `Week`, `Month`, `Year`, `AllTime`

## Next Steps

- [Images](images.md)
- [Tags](tags.md)
- [Error Handling](../common/error-handling.md)
- [Pagination](../common/pagination.md)
