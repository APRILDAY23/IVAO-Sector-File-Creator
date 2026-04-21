# push-secrets.ps1
# Reads secrets.json from the project root and pushes each key as a
# GitHub Actions Secret using the GitHub CLI (gh).
#
# Run this once after cloning, or whenever you update your keys:
#   pwsh scripts/push-secrets.ps1
#
# Prerequisites:
#   - GitHub CLI installed  (https://cli.github.com)
#   - Authenticated:        gh auth login
#   - secrets.json present  (copy from secrets.example.json and fill in values)

$root        = Split-Path $PSScriptRoot -Parent
$secretsFile = Join-Path $root "secrets.json"

if (-not (Test-Path $secretsFile)) {
    Write-Error "secrets.json not found at: $secretsFile"
    Write-Host  "Copy secrets.example.json → secrets.json and fill in your keys first."
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

$pushed = 0
foreach ($prop in $keyMap.GetEnumerator()) {
    $value = $secrets.($prop.Key)
    if ([string]::IsNullOrWhiteSpace($value)) {
        Write-Host "  SKIP  $($prop.Value)  (empty in secrets.json)"
        continue
    }
    $value | gh secret set $prop.Value
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  OK    $($prop.Value)"
        $pushed++
    } else {
        Write-Warning "  FAIL  $($prop.Value)"
    }
}

Write-Host ""
Write-Host "$pushed secret(s) pushed to GitHub."
Write-Host "Run 'git tag v1.0.0 && git push origin v1.0.0' to trigger a release."
