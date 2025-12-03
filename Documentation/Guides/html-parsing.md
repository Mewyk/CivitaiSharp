---
title: HTML Parsing
description: Convert Civitai HTML descriptions to Markdown or plain text using CivitaiSharp.Tools for readable model and version documentation.
---

# HTML Parsing

Civitai model descriptions are stored as HTML. CivitaiSharp.Tools provides the `HtmlParser` class to convert these descriptions to Markdown or plain text format, making them easier to display and process.

## Basic Usage

### Convert to Markdown

[!code-csharp[Program.cs](Tools/Program.cs#html-markdown)]

### Convert to Plain Text

[!code-csharp[Program.cs](Tools/Program.cs#html-plaintext)]

## Extension Methods

For convenience, extension methods are available on `Model` and `ModelVersion`:

[!code-csharp[Program.cs](Tools/Program.cs#html-extensions)]

## Supported HTML Elements

### Block Elements

| HTML Element | Markdown Output | Plain Text Output |
|--------------|-----------------|-------------------|
| `<h1>` - `<h6>` | `#` to `######` | Text with newlines |
| `<p>` | Text with blank line | Text with newlines |
| `<ul>`, `<li>` | `- item` | `* item` |
| `<ol>`, `<li>` | `1. item` | `1. item` |
| `<blockquote>` | `> quote` | Text |
| `<pre>`, `<code>` | ` ``` code ``` ` | Code text |
| `<br>` | Newline | Newline |
| `<hr>` | `---` | Newline |

### Inline Elements

| HTML Element | Markdown Output | Plain Text Output |
|--------------|-----------------|-------------------|
| `<a href="">` | `[text](url)` | Link text only |
| `<img src="" alt="">` | `![alt](src)` | `[alt]` or removed |
| `<strong>`, `<b>` | `**text**` | Text |
| `<em>`, `<i>` | `*text*` | Text |
| `<code>` | `` `text` `` | Text |
| `<s>`, `<del>` | `~~text~~` | Text |

## HTML Entity Decoding

The parser decodes common HTML entities:

| Entity | Output |
|--------|--------|
| `&nbsp;` | Space |
| `&amp;` | `&` |
| `&lt;` | `<` |
| `&gt;` | `>` |
| `&quot;` | `"` |
| `&mdash;` | `-` |
| `&hellip;` | `...` |
| `&#39;` | `'` |
| `&#x27;` | `'` |

Numeric entities (both decimal `&#123;` and hex `&#x7B;`) are also decoded.

## Examples

### Input HTML

```html
<h2>Usage</h2>
<p>This model works best with the following settings:</p>
<ul>
  <li>CFG Scale: <strong>7-8</strong></li>
  <li>Steps: <em>20-30</em></li>
  <li>Sampler: DPM++ 2M Karras</li>
</ul>
<p>Download from <a href="https://civitai.com">Civitai</a>.</p>
```

### Markdown Output

```markdown
## Usage

This model works best with the following settings:

- CFG Scale: **7-8**
- Steps: *20-30*
- Sampler: DPM++ 2M Karras

Download from [Civitai](https://civitai.com).
```

### Plain Text Output

```
Usage

This model works best with the following settings:

* CFG Scale: 7-8
* Steps: 20-30
* Sampler: DPM++ 2M Karras

Download from Civitai.
```

## Processing Model Descriptions

### Displaying in Console

```csharp
var result = await apiClient.Models.GetByIdAsync(123456);

if (result is Result<Model>.Success success)
{
    var model = success.Data;
    
    Console.WriteLine($"# {model.Name}");
    Console.WriteLine();
    Console.WriteLine(model.GetDescriptionAsPlainText());
}
```

### Saving as Markdown File

```csharp
var result = await apiClient.Models.GetByIdAsync(123456);

if (result is Result<Model>.Success success)
{
    var model = success.Data;
    var markdown = model.GetDescriptionAsMarkdown();
    
    await File.WriteAllTextAsync($"{model.Name}.md", markdown);
}
```

### Processing Version Descriptions

```csharp
if (result is Result<Model>.Success success)
{
    var model = success.Data;
    
    foreach (var version in model.ModelVersions ?? [])
    {
        if (!string.IsNullOrWhiteSpace(version.Description))
        {
            Console.WriteLine($"## {version.Name}");
            Console.WriteLine(version.GetDescriptionAsMarkdown());
            Console.WriteLine();
        }
    }
}
```

## Whitespace Normalization

The parser normalizes whitespace in output:

- Multiple consecutive spaces are collapsed to single spaces
- More than two consecutive newlines are collapsed to two
- Leading and trailing whitespace on lines is trimmed
- The overall output is trimmed

## Handling Empty Content

Both methods safely handle null or empty input:

```csharp
var markdown = HtmlParser.ToMarkdown(null);     // Returns ""
var plainText = HtmlParser.ToPlainText("");     // Returns ""
var whitespace = HtmlParser.ToMarkdown("   ");  // Returns ""
```

## Integration with Downloads

Combine HTML parsing with download services for complete model documentation:

```csharp
public class ModelDownloader(IDownloadService downloadService, IApiClient apiClient)
{
    public async Task DownloadWithReadmeAsync(long modelId, string outputDirectory)
    {
        var result = await apiClient.Models.GetByIdAsync(modelId);
        
        if (result is not Result<Model>.Success success)
            return;
            
        var model = success.Data;
        var version = model.ModelVersions?.FirstOrDefault();
        var file = version?.Files?.FirstOrDefault(f => f.Primary == true);
        
        if (file is null || version is null)
            return;
        
        // Download the model file
        var downloadResult = await downloadService.DownloadAsync(file, version, outputDirectory);
        
        if (downloadResult is Result<DownloadedFile>.Success downloadSuccess)
        {
            // Create README.md alongside the model
            var directory = Path.GetDirectoryName(downloadSuccess.Data.FilePath);
            var readmePath = Path.Combine(directory!, "README.md");
            
            var readme = $"""
                # {model.Name}
                
                **Type:** {model.Type}
                **Creator:** {model.Creator?.Username}
                **Version:** {version.Name}
                **Base Model:** {version.BaseModel}
                
                ## Description
                
                {model.GetDescriptionAsMarkdown()}
                
                ## Version Notes
                
                {version.GetDescriptionAsMarkdown()}
                
                ## Trigger Words
                
                {string.Join(", ", version.TrainedWords ?? [])}
                """;
            
            await File.WriteAllTextAsync(readmePath, readme);
        }
    }
}
```

## Performance

The HTML parser:

- Uses compiled regular expressions via `GeneratedRegex` for optimal performance
- Processes content in a single pass where possible
- Minimizes memory allocations using `StringBuilder`
- Is stateless and thread-safe (all methods are static)

## Limitations

The parser is designed for Civitai's HTML content and handles common elements. Complex or nested HTML may not render perfectly. For advanced HTML processing, consider a full HTML parser library.

## Next Steps

- [File Hashing](file-hashing.md) - Verify downloaded files
- [Downloading Files](downloading-files.md) - Download models and images
