<#
.SYNOPSIS
    Prepares and publishes packages for CivitaiSharp.

.DESCRIPTION
    Publishing workflow:
    1. Validate version and check prerequisites
    2. Run tests
    3. Generate and commit NUGET.md
    4. Push commit to remote branch
    5. Create and push version tag (triggers CI publishing)

.PARAMETER Version
    The version to publish (e.g., 0.9.0, 1.0.0-alpha.1).
    If not provided, auto-increments the patch version.

.PARAMETER SkipTests
    Skip running tests (not recommended).

.PARAMETER Force
    Continue even if there are uncommitted changes.

.EXAMPLE
    .\PrepareRelease.ps1 -Version 0.9.0
#>
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$Version,

    [Parameter()]
    [switch]$SkipTests,

    [Parameter()]
    [switch]$Force
)

$ErrorActionPreference = 'Stop'
$Script:ScriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$Script:RepositoryRoot = Split-Path -Parent (Split-Path -Parent $Script:ScriptDirectory)
Set-Location -Path $Script:RepositoryRoot


#region Helper Functions

function Write-StepHeader {
    param(
        [Parameter(Mandatory)]
        [string]$Message
    )
    Write-Host "`n>> $Message" -ForegroundColor Cyan
}


function Write-SuccessMessage {
    param(
        [Parameter(Mandatory)]
        [string]$Message
    )
    Write-Host "   $Message" -ForegroundColor Green
}


function Write-WarningMessage {
    param(
        [Parameter(Mandatory)]
        [string]$Message
    )
    Write-Host "   $Message" -ForegroundColor Magenta
}


function Write-ErrorMessage {
    param(
        [Parameter(Mandatory)]
        [string]$Message
    )
    Write-Host "   $Message" -ForegroundColor Red
}


function Get-LatestVersionTag {
    [OutputType([string])]
    param()

    $tags = git tag -l 'v*' | Where-Object { $_ -match '^v\d+\.\d+\.\d+' }
    if (-not $tags) {
        return $null
    }

    return $tags |
        Sort-Object -Descending {
            if ($_ -match '^v(\d+)\.(\d+)\.(\d+)') {
                [int]$Matches[1] * 10000 + [int]$Matches[2] * 100 + [int]$Matches[3]
            } else {
                0
            }
        } |
        Select-Object -First 1
}


function Get-NextPatchVersion {
    [OutputType([string])]
    param(
        [Parameter(Mandatory)]
        [string]$CurrentTag
    )

    if ($CurrentTag -match '^v(\d+)\.(\d+)\.(\d+)(-.*)?$') {
        $majorVersion = [int]$Matches[1]
        $minorVersion = [int]$Matches[2]
        $patchVersion = [int]$Matches[3]
        $prereleaseIdentifier = $Matches[4]

        if ($prereleaseIdentifier) {
            return "$majorVersion.$minorVersion.$patchVersion"
        }
        return "$majorVersion.$minorVersion.$($patchVersion + 1)"
    }
    return $null
}


function Request-UserConfirmation {
    [OutputType([bool])]
    param(
        [Parameter(Mandatory)]
        [string]$Prompt,

        [Parameter()]
        [bool]$DefaultYes = $true
    )

    $suffix = if ($DefaultYes) { '(Y/n)' } else { '(y/N)' }
    $response = Read-Host -Prompt "$Prompt $suffix"

    if ($DefaultYes) {
        return ($response -ne 'n' -and $response -ne 'N')
    }
    return ($response -eq 'y' -or $response -eq 'Y')
}

#endregion


#region Main Script

Write-Host "`nCivitaiSharp Publishing" -ForegroundColor Magenta

if (-not $Version) {
    $latestTag = Get-LatestVersionTag
    if ($latestTag) {
        $Version = Get-NextPatchVersion -CurrentTag $latestTag
        Write-Host "   Latest: $latestTag -> Next: $Version" -ForegroundColor Gray
    } else {
        $Version = Read-Host -Prompt 'No tags found. Enter version (e.g., 0.1.0)'
    }
}

if (-not $Version -or $Version -notmatch '^\d+\.\d+\.\d+(-[a-zA-Z0-9.]+)?$') {
    Write-ErrorMessage -Message "Invalid or missing version: $Version"
    exit 1
}

$tag = "v$Version"
$currentBranch = git branch --show-current
Write-SuccessMessage -Message "Version: $Version | Tag: $tag | Branch: $currentBranch"

$existingTag = git tag -l $tag
if ($existingTag) {
    Write-ErrorMessage -Message "Tag $tag already exists"
    exit 1
}

$uncommittedChanges = git status --porcelain
if ($uncommittedChanges -and -not $Force) {
    Write-ErrorMessage -Message 'Uncommitted changes exist. Use -Force to bypass.'
    $uncommittedChanges | ForEach-Object { Write-Host "   $_" -ForegroundColor Magenta }
    exit 1
}

if (-not $SkipTests) {
    Write-StepHeader -Message 'Running tests'
    dotnet test --configuration Release --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMessage -Message 'Tests failed'
        exit 1
    }
    Write-SuccessMessage -Message 'All tests passed'
}

Write-StepHeader -Message 'Generating NUGET.md'
& (Join-Path -Path $Script:ScriptDirectory -ChildPath 'GenerateNuGetReadme.ps1')
Write-SuccessMessage -Message 'NUGET.md generated'

$nugetReadmeChanged = git status --porcelain 'NUGET.md'
if ($nugetReadmeChanged) {
    Write-StepHeader -Message 'Syncing NUGET.md'
    git add 'NUGET.md'
    git commit -m "Update NUGET.md for $tag"
    git push origin $currentBranch
    Write-SuccessMessage -Message "NUGET.md synced to origin/$currentBranch"
} else {
    Write-SuccessMessage -Message 'NUGET.md unchanged'
}

Write-StepHeader -Message "Creating tag $tag"
git tag -a $tag -m "Release $Version"
Write-SuccessMessage -Message "Tag $tag created locally"

if (Request-UserConfirmation -Prompt "Push tag $tag to trigger publishing?" -DefaultYes $false) {
    git push origin $tag
    Write-SuccessMessage -Message 'Tag pushed - GitHub Actions will build and publish'
} else {
    Write-WarningMessage -Message "Tag created locally. Run 'git push origin $tag' when ready."
}

Write-Host "`nPublishing Complete`n" -ForegroundColor Magenta

#endregion
