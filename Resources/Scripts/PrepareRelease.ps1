<#
.SYNOPSIS
    Prepares and creates a new release tag for CivitaiSharp.
    
.DESCRIPTION
    This script performs pre-release verification and tasks:
    - Verifies the working directory is clean
    - Runs all tests
    - Generates the NuGet README from README.md
    - Commits any changes (if needed)
    - Creates and pushes a new version tag
    
.PARAMETER Version
    The version to release (e.g., 0.9.0, 1.0.0-alpha.1).
    If not provided, auto-increments the patch version from the latest tag.
    
.PARAMETER SkipTests
    Skip running tests (not recommended).

.PARAMETER Force
    Continue even if there are uncommitted changes.

.PARAMETER Confirm
    Skip confirmation prompts for intermediate steps.
    The final push step ALWAYS requires confirmation.
    
.PARAMETER DryRun
    Show what would be done without making changes.
    
.EXAMPLE
    .\PrepareRelease.ps1
    # Auto-increments patch version (e.g., 0.8.0 -> 0.8.1)
    
.EXAMPLE
    .\PrepareRelease.ps1 -Version 0.9.0
    
.EXAMPLE
    .\PrepareRelease.ps1 -Version 1.0.0-alpha.1 -DryRun
    
.EXAMPLE
    .\PrepareRelease.ps1 -Confirm:$false
    # Skip intermediate confirmations, but final push still requires confirmation
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string]$Version,
    [switch]$SkipTests,
    [switch]$Force,
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)

Set-Location $repoRoot

function Write-Step {
    param([string]$Message)
    Write-Host "`n>> $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "   $Message" -ForegroundColor Green
}

function Write-Warn {
    param([string]$Message)
    Write-Host "   $Message" -ForegroundColor Magenta
}

function Write-Err {
    param([string]$Message)
    Write-Host "   $Message" -ForegroundColor Red
}

function Get-LatestVersionTag {
    # Get all version tags, sort by version, return the latest
    $tags = git tag -l "v*" | Where-Object { $_ -match '^v\d+\.\d+\.\d+' }
    if (-not $tags) {
        return $null
    }
    
    $sorted = $tags | Sort-Object {
        if ($_ -match '^v(\d+)\.(\d+)\.(\d+)') {
            [int]$matches[1] * 10000 + [int]$matches[2] * 100 + [int]$matches[3]
        }
        else { 0 }
    } -Descending
    
    return $sorted | Select-Object -First 1
}

function Get-NextPatchVersion {
    param([string]$CurrentTag)
    
    if ($CurrentTag -match '^v(\d+)\.(\d+)\.(\d+)(-.*)?$') {
        $major = [int]$matches[1]
        $minor = [int]$matches[2]
        $patch = [int]$matches[3] + 1
        $prerelease = $matches[4]
        
        if ($prerelease) {
            # If current is prerelease, keep same version without prerelease
            return "$major.$minor.$($patch - 1)"
        }
        return "$major.$minor.$patch"
    }
    return $null
}

# Header
Write-Host "`n========================================" -ForegroundColor Magenta
Write-Host "  CivitaiSharp Release Preparation" -ForegroundColor Magenta
Write-Host "========================================`n" -ForegroundColor Magenta

if ($DryRun) {
    Write-Warn "DRY RUN MODE - No changes will be made"
}

# Step 1: Get version
Write-Step "Determining version..."

if (-not $Version) {
    $latestTag = Get-LatestVersionTag
    if ($latestTag) {
        $suggestedVersion = Get-NextPatchVersion $latestTag
        Write-Host "   Latest tag: $latestTag" -ForegroundColor Gray
        Write-Host "   Suggested next version: $suggestedVersion" -ForegroundColor Gray
        
        if ($PSCmdlet.ShouldProcess("Version $suggestedVersion", "Use suggested version")) {
            $Version = $suggestedVersion
            Write-Success "Using version: $Version"
        }
        else {
            $Version = Read-Host "Enter version to release (or press Enter for $suggestedVersion)"
            if (-not $Version) {
                $Version = $suggestedVersion
            }
        }
    }
    else {
        Write-Warn "No existing version tags found"
        $Version = Read-Host "Enter version to release (e.g., 0.1.0)"
    }
}

if (-not $Version) {
    Write-Err "Version is required"
    exit 1
}

# Validate version format
if ($Version -notmatch '^\d+\.\d+\.\d+(-[a-zA-Z0-9.]+)?$') {
    Write-Err "Invalid version format: $Version"
    Write-Err "Expected format: MAJOR.MINOR.PATCH or MAJOR.MINOR.PATCH-prerelease"
    exit 1
}

$tag = "v$Version"
Write-Success "Version: $Version (tag: $tag)"

# Step 2: Check for existing tag
Write-Step "Checking for existing tag..."
$existingTag = git tag -l $tag
if ($existingTag) {
    Write-Err "Tag $tag already exists"
    exit 1
}
Write-Success "Tag $tag is available"

# Step 3: Check working directory
Write-Step "Checking working directory status..."
$status = git status --porcelain
if ($status) {
    Write-Warn "Working directory has uncommitted changes:"
    $status | ForEach-Object { Write-Host "   $_" -ForegroundColor Magenta }
    if (-not $Force) {
        Write-Err "Commit or stash changes before releasing, or use -Force to bypass."
        exit 1
    }
    Write-Warn "Continuing due to -Force flag..."
}
else {
    Write-Success "Working directory is clean"
}

# Step 4: Run tests
if ($SkipTests) {
    Write-Step "Skipping tests (not recommended)..."
}
else {
    Write-Step "Running tests..."
    if (-not $DryRun) {
        $testResult = dotnet test --configuration Release --verbosity minimal
        if ($LASTEXITCODE -ne 0) {
            Write-Err "Tests failed. Aborting release."
            exit 1
        }
        Write-Success "All tests passed"
    }
    else {
        Write-Success "(dry run) Would run: dotnet test --configuration Release"
    }
}

# Step 5: Generate NuGet README
Write-Step "Generating NuGet README..."
$generateScript = Join-Path $scriptDir "GenerateNuGetReadme.ps1"
if (-not $DryRun) {
    & $generateScript
    Write-Success "NUGET.md generated"
}
else {
    Write-Success "(dry run) Would run: $generateScript"
}

# Step 6: Check if NUGET.md changed
Write-Step "Checking for changes to commit..."
$nugetStatus = git status --porcelain "NUGET.md"
if ($nugetStatus) {
    Write-Warn "NUGET.md has changes"
    
    $shouldCommit = $true
    if (-not $PSCmdlet.ShouldProcess("NUGET.md", "Commit changes")) {
        $confirm = Read-Host "Commit NUGET.md changes? (Y/n)"
        $shouldCommit = ($confirm -ne 'n' -and $confirm -ne 'N')
    }
    
    if ($shouldCommit -and -not $DryRun) {
        git add "NUGET.md"
        git commit -m "Update NUGET.md for $tag release"
        Write-Success "Committed NUGET.md changes"
    }
    elseif ($DryRun) {
        Write-Success "(dry run) Would commit NUGET.md changes"
    }
}
else {
    Write-Success "No changes to commit"
}

# Step 7: Create tag
Write-Step "Creating tag $tag..."

$shouldCreateTag = $true
if (-not $PSCmdlet.ShouldProcess("Tag $tag", "Create tag")) {
    $confirm = Read-Host "Create tag $tag? (Y/n)"
    $shouldCreateTag = ($confirm -ne 'n' -and $confirm -ne 'N')
}

if ($shouldCreateTag) {
    if (-not $DryRun) {
        git tag -a $tag -m "Release $Version"
        Write-Success "Tag $tag created"
    }
    else {
        Write-Success "(dry run) Would create tag: $tag"
    }
}
else {
    Write-Warn "Tag creation skipped"
    exit 0
}

# Step 8: Push (ALWAYS requires confirmation)
Write-Step "Pushing to remote..."
Write-Host ""
Write-Host "   ============================================" -ForegroundColor White
Write-Host "   FINAL STEP: This will trigger the CI release" -ForegroundColor White
Write-Host "   ============================================" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "Push tag $tag to origin? (y/N)"
if ($confirm -eq 'y' -or $confirm -eq 'Y') {
    if (-not $DryRun) {
        git push origin $tag
        Write-Success "Tag $tag pushed to origin"
        Write-Host "`n   GitHub Actions will now build and publish the release." -ForegroundColor Cyan
    }
    else {
        Write-Success "(dry run) Would push tag $tag to origin"
    }
}
else {
    if (-not $DryRun) {
        Write-Warn "Tag created locally but not pushed."
        Write-Warn "Run 'git push origin $tag' to trigger the release workflow."
    }
    else {
        Write-Warn "(dry run) Push cancelled."
    }
}

# Done
Write-Host "`n========================================" -ForegroundColor Magenta
Write-Host "  Release preparation complete" -ForegroundColor Magenta
Write-Host "========================================`n" -ForegroundColor Magenta
