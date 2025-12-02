<#
.SYNOPSIS
    Generates NUGET.md from README.md for NuGet package consumption.
    
.DESCRIPTION
    Transforms the main README.md to be compatible with NuGet's Markdown renderer:
    - Converts HTML tags to plain Markdown
    - Replaces relative image paths with absolute GitHub raw URLs
    - Removes GitHub-specific elements (language switcher, etc.)
    - Cleans up formatting
    
.PARAMETER RepoOwner
    GitHub repository owner. Default: Mewyk
    
.PARAMETER RepoName
    GitHub repository name. Default: CivitaiSharp
    
.PARAMETER Branch
    Git branch for raw content URLs. Default: alpha
#>
param(
    [string]$RepoOwner = "Mewyk",
    [string]$RepoName = "CivitaiSharp",
    [string]$Branch = "alpha"
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
$readmePath = Join-Path $repoRoot "README.md"
$nugetPath = Join-Path $repoRoot "NUGET.md"

Write-Host "Generating NUGET.md from README.md..."

$content = Get-Content -Path $readmePath -Raw
$rawUrl = "https://raw.githubusercontent.com/$RepoOwner/$RepoName/$Branch"

# Step 1: Replace relative image paths with absolute GitHub raw URLs
$content = $content -replace 'src="Resources/', "src=`"$rawUrl/Resources/"
$content = $content -replace '\]\(Resources/', "]($rawUrl/Resources/"

# Step 2: Remove the entire header section (logo, title, badges, language links)
# Find where "## Table of Contents" starts and keep everything from there
$tocMatch = [regex]::Match($content, '(?m)^## Table of Contents')
if ($tocMatch.Success) {
    $headerContent = $content.Substring(0, $tocMatch.Index)
    $bodyContent = $content.Substring($tocMatch.Index)
    
    # Build a clean header (no badges - they don't render well on NuGet)
    $cleanHeader = @"
# CivitaiSharp

![CivitaiSharp Logo]($rawUrl/Resources/Logo/CivitaiSharp.128.png)

A modern, lightweight, and AOT-ready .NET 10 client library for all things Civitai.com.

> **Note:** CivitaiSharp is currently in Alpha. APIs, features, and stability are subject to change.

"@
    
    $content = $cleanHeader + $bodyContent
}

# Step 3: Convert <details><summary><strong>Title</strong></summary> pattern to **Title** with newlines
# This pattern appears before code blocks and needs proper spacing
$content = $content -replace '<details>\s*\r?\n?<summary><strong>([^<]+)</strong></summary>\s*\r?\n?', "`n**`$1**`n`n"
$content = $content -replace '</details>\s*', "`n"

# Step 4: Remove any remaining HTML details/summary tags
$content = $content -replace '<details>\s*', ''
$content = $content -replace '</details>\s*', ''
$content = $content -replace '<summary>\s*', ''
$content = $content -replace '</summary>\s*', ''

# Step 5: Convert remaining HTML strong tags
$content = $content -replace '<strong>', '**'
$content = $content -replace '</strong>', '**'

# Step 6: Remove HTML comments
$content = $content -replace '<!--[\s\S]*?-->', ''

# Step 7: Convert HTML footer to Markdown (centered paragraph with links)
# Pattern: <p align="center">..links..</p>
$footerPattern = '<p[^>]*align="center"[^>]*>\s*([\s\S]*?)\s*</p>'
$content = [regex]::Replace($content, $footerPattern, {
    param($match)
    $innerHtml = $match.Groups[1].Value
    # Extract links: <a href="url">text</a> -> [text](url)
    $linkPattern = '<a\s+href="([^"]+)"[^>]*>([^<]+)</a>'
    $links = [regex]::Matches($innerHtml, $linkPattern)
    $mdLinks = @()
    foreach ($link in $links) {
        $url = $link.Groups[1].Value
        $text = $link.Groups[2].Value
        $mdLinks += "[$text]($url)"
    }
    if ($mdLinks.Count -gt 0) {
        return "`n---`n`n" + ($mdLinks -join " | ") + "`n"
    }
    return ""
})

# Step 8: Clean up excessive blank lines (more than 2 consecutive)
$content = $content -replace '(\r?\n){3,}', "`n`n"

# Step 9: Clean up duplicate horizontal rules (--- followed by ---)
$content = $content -replace '---\s*\r?\n\s*---', '---'

# Step 10: Trim trailing whitespace from each line
$lines = $content -split "`n"
$lines = $lines | ForEach-Object { $_.TrimEnd() }
$content = $lines -join "`n"

# Step 11: Ensure file ends with single newline
$content = $content.TrimEnd() + "`n"

# Write the result
$content | Set-Content -Path $nugetPath -NoNewline -Encoding UTF8

Write-Host "Generated: $nugetPath"
Write-Host "Done."
