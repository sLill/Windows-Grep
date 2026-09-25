<#
.SYNOPSIS
    Builds WindowsGrep in release mode for x64 and ARM64, then packages each build
    into its NSIS installer.

.DESCRIPTION
    Both architectures are compiled before either installer is packaged, so a failed
    build leaves the existing installers untouched. The installers are written next
    to WindowsGrep\installer.nsi, replacing any with the same version:

        WindowsGrep-<version>-Setup.exe
        WindowsGrep-<version>-arm64-Setup.exe

    Requires rustup, NSIS (makensis) and the Visual Studio C++ build tools for x64
    and ARM64. Missing Rust targets are added with rustup. Can be run from any
    directory.

.EXAMPLE
    .\scripts\build_installer.ps1
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$projectDir = Join-Path (Split-Path -Parent $PSScriptRoot) 'WindowsGrep'

$builds = @(
    @{ Name = 'x64';   Triple = 'x86_64-pc-windows-msvc';  Suffix = '' }
    @{ Name = 'arm64'; Triple = 'aarch64-pc-windows-msvc'; Suffix = '-arm64' }
)

function Write-Step([string]$Message) {
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

# Runs a native command and throws if it exits non-zero. 'Stop' is relaxed for the
# call itself because hosts without a console (the ISE, remote sessions) turn native
# stderr, where cargo prints its progress, into error records that 'Stop' aborts on.
function Invoke-Native([string]$Exe, [string[]]$Arguments) {
    $null = Get-Command $Exe
    $ErrorActionPreference = 'Continue'
    & $Exe @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "'$Exe $Arguments' failed with exit code $LASTEXITCODE."
    }
}

# NSIS doesn't add itself to PATH, so fall back to its default install folder.
$nsis = Get-Command makensis -ErrorAction SilentlyContinue
$makensis = if ($nsis) { $nsis.Source } else { "${env:ProgramFiles(x86)}\NSIS\makensis.exe" }
if (-not (Test-Path $makensis)) {
    throw 'makensis not found. Install NSIS (https://nsis.sourceforge.io) or add it to PATH.'
}

Push-Location $projectDir
try {
    $metadata = Invoke-Native 'cargo' @('metadata', '--format-version', '1', '--no-deps') | ConvertFrom-Json
    $version = $metadata.packages[0].version

    # The version is also hard-coded in these files. Stop rather than build installers
    # whose name or splash banner disagrees with the binary inside them.
    $escaped = [regex]::Escape($version)
    $versionChecks = @(
        @{ Label = 'APP_VERSION in installer.nsi';   File = 'installer.nsi';        Pattern = "(?m)^!define\s+APP_VERSION\s+`"$escaped`"" }
        @{ Label = 'APP_VERSION_4 in installer.nsi'; File = 'installer.nsi';        Pattern = "(?m)^!define\s+APP_VERSION_4\s+`"$escaped\.\d+`"" }
        @{ Label = 'resources\splash.txt';           File = 'resources\splash.txt'; Pattern = "v$escaped(?!\d)" }
    )
    $stale = @($versionChecks | Where-Object { (Get-Content -Raw $_.File) -notmatch $_.Pattern } | ForEach-Object { $_.Label })
    if ($stale) {
        throw "Cargo.toml is at version $version but these don't match it: $($stale -join ', ')."
    }

    $installedTargets = @(Invoke-Native 'rustup' @('target', 'list', '--installed'))
    foreach ($build in $builds) {
        if ($installedTargets -notcontains $build.Triple) {
            Write-Step "Adding Rust target $($build.Triple)"
            Invoke-Native 'rustup' @('target', 'add', $build.Triple)
        }
    }

    foreach ($build in $builds) {
        Write-Step "Building $($build.Name) release ($($build.Triple))"
        Invoke-Native 'cargo' @('build', '--release', '--target', $build.Triple)
    }

    $packagingStarted = Get-Date
    foreach ($build in $builds) {
        Write-Step "Packaging $($build.Name) installer"
        $binDir = Join-Path $metadata.target_directory "$($build.Triple)\release"
        Invoke-Native $makensis @('/V2', "/DTARGET_DIR=$binDir", "/DARCH_SUFFIX=$($build.Suffix)", 'installer.nsi')
    }

    Write-Step "Built WindowsGrep $version installers"
    Get-ChildItem -Filter '*-Setup.exe' | Where-Object { $_.LastWriteTime -ge $packagingStarted } | ForEach-Object {
        Write-Host ('    {0}  ({1:N1} MB)' -f $_.FullName, ($_.Length / 1MB))
    }
}
finally {
    Pop-Location
}
