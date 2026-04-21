# push-secrets.ps1
# Reads secrets.json and pushes each key as a GitHub Actions Secret.
# Skips secrets that already exist on GitHub.
#
# Run this once after cloning, or whenever you add a NEW key:
#   powershell -ExecutionPolicy Bypass -File scripts/push-secrets.ps1
#
# Prerequisites:
#   - GitHub CLI installed  (https://cli.github.com)
#   - Authenticated:        gh auth login
#   - secrets.json present  (copy from secrets.example.json and fill in values)

# Auto-locate gh.exe if not on PATH
$ghCmd = Get-Command gh -ErrorAction SilentlyContinue
$gh    = if ($ghCmd) { $ghCmd.Source } else { $null }
if (-not $gh) {
    $fallback = "C:\Program Files\GitHub CLI\gh.exe"
    if (Test-Path $fallback) { $gh = $fallback }
    else {
        Write-Error "GitHub CLI (gh) not found. Install from https://cli.github.com"
        exit 1
    }
}

# Only the repo owner may push secrets
$OWNER = "APRILDAY23"
$whoami = & $gh api user --jq ".login" 2>$null
if ($whoami -ne $OWNER) {
    Write-Error "Access denied. Only the repo owner ($OWNER) can push secrets. You are logged in as: $whoami"
    exit 1
}

$root        = Split-Path $PSScriptRoot -Parent
$secretsFile = Join-Path $root "secrets.json"

if (-not (Test-Path $secretsFile)) {
    Write-Error "secrets.json not found at: $secretsFile"
    Write-Host  "Copy secrets.example.json -> secrets.json and fill in your keys first."
    exit 1
}

$secrets = Get-Content $secretsFile | ConvertFrom-Json

# Map JSON property names to GitHub Secret names
$keyMap = @{
    openAipApiKey     = "OPEN_AIP_API_KEY"
    ivaoApiKey        = "IVAO_API_KEY"
    mapboxToken       = "MAPBOX_TOKEN"
    oauthClientId     = "OAUTH_CLIENT_ID"
    oauthClientSecret = "OAUTH_CLIENT_SECRET"
    aeroDataBoxApiKey = "AERO_DATABOX_API_KEY"
}

# Fetch existing secret names from GitHub (values are never returned by the API)
$existingRaw = & $gh secret list --json name 2>$null
$existing    = @()
if ($existingRaw) {
    try { $existing = ($existingRaw | ConvertFrom-Json) | ForEach-Object { $_.name } }
    catch { }
}

$pushed  = 0
$skipped = 0
foreach ($prop in $keyMap.GetEnumerator()) {
    $value = $secrets.($prop.Key)
    if ([string]::IsNullOrWhiteSpace($value)) {
        Write-Host "  SKIP  $($prop.Value)  (empty in secrets.json)"
        $skipped++
        continue
    }
    if ($existing -contains $prop.Value) {
        Write-Host "  SKIP  $($prop.Value)  (already exists on GitHub)"
        $skipped++
        continue
    }
    $value | & $gh secret set $prop.Value
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  OK    $($prop.Value)"
        $pushed++
    } else {
        Write-Warning "  FAIL  $($prop.Value)"
    }
}

Write-Host ""
Write-Host "$pushed secret(s) pushed, $skipped skipped."
