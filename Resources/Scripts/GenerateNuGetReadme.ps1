<#
.SYNOPSIS
    Generates NUGET.md from README.md for NuGet package consumption.

.DESCRIPTION
    Transforms the main README.md to be compatible with NuGet's Markdown renderer:
    - Converts HTML tags to plain Markdown
    - Replaces relative image paths with absolute GitHub raw URLs
    - Removes GitHub-specific elements (language switcher, etc.)
    - Cleans up formatting

.PARAMETER RepositoryOwner
    GitHub repository owner. Default: Mewyk

.PARAMETER RepositoryName
    GitHub repository name. Default: CivitaiSharp

.PARAMETER Branch
    Git branch for raw content URLs. Default: alpha
#>
param(
    [Parameter()]
    [string]$RepositoryOwner = 'Mewyk',

    [Parameter()]
    [string]$RepositoryName = 'CivitaiSharp',

    [Parameter()]
    [string]$Branch = 'alpha'
)

$ErrorActionPreference = 'Stop'
$Script:ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$Script:RepositoryRoot = Split-Path -Parent (Split-Path -Parent $Script:ScriptDirectory)
$readmeFilePath = Join-Path -Path $Script:RepositoryRoot -ChildPath 'README.md'
$nugetReadmeFilePath = Join-Path -Path $Script:RepositoryRoot -ChildPath 'NUGET.md'

Write-Host 'Generating NUGET.md from README.md...'

$readmeContent = Get-Content -Path $readmeFilePath -Raw
$rawGitHubUrl = "https://raw.githubusercontent.com/$RepositoryOwner/$RepositoryName/$Branch"

$readmeContent = $readmeContent -replace 'src="Resources/', "src=`"$rawGitHubUrl/Resources/"
$readmeContent = $readmeContent -replace '\]\(Resources/', "]($rawGitHubUrl/Resources/"

$tableOfContentsMatch = [regex]::Match($readmeContent, '(?m)^## Table of Contents')
if ($tableOfContentsMatch.Success) {
    $bodyContent = $readmeContent.Substring($tableOfContentsMatch.Index)

    $cleanHeader = @"
# CivitaiSharp

![CivitaiSharp Logo]($rawGitHubUrl/Resources/Logo/CivitaiSharp.128.png)

A modern, lightweight, and AOT-ready .NET 10 client library for all things Civitai.com.

> **Note:** CivitaiSharp is currently in Alpha. APIs, features, and stability are subject to change.

"@

    $readmeContent = $cleanHeader + $bodyContent
}

$readmeContent = $readmeContent -replace '<details>\s*\r?\n?<summary><strong>([^<]+)</strong></summary>\s*\r?\n?', "`n**`$1**`n`n"
$readmeContent = $readmeContent -replace '</details>\s*', "`n"
$readmeContent = $readmeContent -replace '<details>\s*', ''
$readmeContent = $readmeContent -replace '</details>\s*', ''
$readmeContent = $readmeContent -replace '<summary>\s*', ''
$readmeContent = $readmeContent -replace '</summary>\s*', ''
$readmeContent = $readmeContent -replace '<strong>', '**'
$readmeContent = $readmeContent -replace '</strong>', '**'
$readmeContent = $readmeContent -replace '<!--[\s\S]*?-->', ''

$footerPattern = '<p[^>]*align="center"[^>]*>\s*([\s\S]*?)\s*</p>'
$readmeContent = [regex]::Replace($readmeContent, $footerPattern, {
    param($match)
    $innerHtml = $match.Groups[1].Value
    $linkPattern = '<a\s+href="([^"]+)"[^>]*>([^<]+)</a>'
    $linkMatches = [regex]::Matches($innerHtml, $linkPattern)
    $markdownLinks = @()
    foreach ($linkMatch in $linkMatches) {
        $url = $linkMatch.Groups[1].Value
        $text = $linkMatch.Groups[2].Value
        $markdownLinks += "[$text]($url)"
    }
    if ($markdownLinks.Count -gt 0) {
        return "`n---`n`n" + ($markdownLinks -join ' | ') + "`n"
    }
    return ''
})

$readmeContent = $readmeContent -replace '(\r?\n){3,}', "`n`n"
$readmeContent = $readmeContent -replace '---\s*\r?\n\s*---', '---'

$lines = $readmeContent -split "`n"
$lines = $lines | ForEach-Object { $_.TrimEnd() }
$readmeContent = $lines -join "`n"
$readmeContent = $readmeContent.TrimEnd() + "`n"

$readmeContent | Set-Content -Path $nugetReadmeFilePath -NoNewline -Encoding UTF8

Write-Host "Generated: $nugetReadmeFilePath"
