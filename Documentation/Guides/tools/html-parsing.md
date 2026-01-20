---
title: HTML Parsing
description: Convert Civitai HTML to Markdown or plain text.
---

# HTML Parsing

Convert Civitai model descriptions from HTML to Markdown or plain text.

## Convert to Markdown

[!code-csharp[Program.cs](examples/Tools/Program.cs#HtmlMarkdown)]

## Convert to Plain Text

[!code-csharp[Program.cs](examples/Tools/Program.cs#HtmlPlaintext)]

## Extension Methods

[!code-csharp[Program.cs](examples/Tools/Program.cs#HtmlExtensions)]

## Supported Elements

**Block:** `h1-h6`, `p`, `ul/ol/li`, `blockquote`, `pre/code`, `br`, `hr`

**Inline:** `a`, `img`, `strong/b`, `em/i`, `code`, `s/del`

**Entities:** `&nbsp;`, `&amp;`, `&lt;`, `&gt;`, `&quot;`, `&mdash;`, etc.

## Example

**Input:**
```html
<h2>Usage</h2>
<p>Settings:</p>
<ul>
  <li>CFG: <strong>7-8</strong></li>
  <li>Steps: <em>20-30</em></li>
</ul>
```

**Markdown:**
```markdown
## Usage

Settings:

- CFG: **7-8**
- Steps: *20-30*
```

**Plain Text:**
```
Usage

Settings:

* CFG: 7-8
* Steps: 20-30
```

## Practical Usage

**Console Display:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#DisplayingInConsole)]

**Save as Markdown:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#SavingAsMarkdownFile)]

**Process Versions:**
[!code-csharp[Program.cs](examples/Tools/Program.cs#ProcessingVersionDescriptions)]

## Empty Content

[!code-csharp[Program.cs](examples/Tools/Program.cs#HandlingEmptyContent)]

## Next Steps

- [File Hashing](file-hashing.md)
- [Downloading Files](downloading-files.md)
