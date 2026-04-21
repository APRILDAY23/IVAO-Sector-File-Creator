# IVAO Sector File Creator

A community-made Windows desktop tool for building IVAO sector files.
Pulls real-world aviation data from free, publicly available APIs to generate
airport data, FIR boundaries, SID/STAR procedures, and flight schedules.

> **⚠ NOT FOR REAL WORLD USE** - simulation purposes only.
> This is an **unofficial** tool and is not affiliated with IVAO.

---

## Features

| Page | What it does |
|---|---|
| **Airport Data** | Fetches airports, runways, frequencies, VOR/DME, NDB for a FIR region |
| **FIR / Airspace** | Downloads FIR, TMA, CTR, restricted airspace boundaries |
| **SID & STAR** | Generates SID/STAR procedure files for a given airport ICAO |
| **Flight Schedules** | Real-world departures and arrivals with Excel export |
| **Ground Layout** | Imports KML ground layouts and converts to sector file format |

---

## Installation (End Users)

1. Download the latest installer from the [Releases](https://github.com/APRILDAY23/IVAO-Sector-File-Creator/releases) page.
2. Run `IVAO-Sector-File-Creator-Setup-vX.X.X.exe`.
3. Follow the wizard - Start Menu and optional Desktop shortcut are created.
4. Launch **IVAO Sector File Creator** and log in with your IVAO credentials.

No API keys to configure. The app comes ready to use.

**Requirements:** Windows 10 or later (64-bit)

---

## Update Channels

| Channel | Description | How to get |
|---|---|---|
| **Stable** | Tested releases from `main` | Default - install any release without `-beta` in the tag |
| **Beta** | Development builds from `develop` | Install a release tagged `v*-beta.*` |

Switch channel inside the app: **Settings → Updates → Channel**.
The app checks for updates automatically on startup and shows a notification when one is available.

---

## For Developers

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Visual Studio 2022 (or any IDE with C# support)
- [GitHub CLI](https://cli.github.com) (for pushing secrets)

### Quick Start

```bash
git clone https://github.com/APRILDAY23/IVAO-Sector-File-Creator.git
cd IVAO-Sector-File-Creator
cp secrets.example.json secrets.json   # fill in your own API keys
dotnet build
```

On first Debug run the app reads `secrets.json`, encrypts your keys to `%AppData%`,
then deletes `secrets.json` from the output folder. Keys persist across builds.

### API Keys

The app uses these free-tier APIs. Get your own keys to develop:

| Key | Where to get |
|---|---|
| `openAipApiKey` | [app.openaip.net](https://app.openaip.net) → Account → API Key |
| `ivaoApiKey` | [api.ivao.aero](https://api.ivao.aero) developer portal |
| `mapboxToken` | [account.mapbox.com](https://account.mapbox.com) |
| `oauthClientId` / `oauthClientSecret` | Register an OAuth app on [sso.ivao.aero](https://sso.ivao.aero) |
| `aeroDataBoxApiKey` | [rapidapi.com/aerodatabox](https://rapidapi.com) → subscribe to AeroDataBox |

Fill them into `secrets.json` (copied from `secrets.example.json`). Never commit this file.

### Push Secrets to GitHub (repo owner only)

Contributors do **not** push secrets. The script is locked to the repo owner's GitHub account - it will refuse to run if you are authenticated as anyone else.

The owner runs this after approving and merging a PR into `develop`, only if that PR introduced a new API key:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/push-secrets.ps1
```

The script verifies your GitHub identity, skips secrets already set, and requires the GitHub CLI (`gh auth login`).

### Branch Strategy

| Branch | Purpose | Releases |
|---|---|---|
| `main` | Production - stable, tested | Tagged `v1.0.0` → creates **Stable** release |
| `develop` | Active development / staging | Tagged `v1.0.0-beta.1` → creates **Beta** pre-release |

- All PRs target **`develop`**, not `main`.
- `develop` is merged to `main` when ready for a stable release.
- GitHub Actions builds and validates every PR on both branches.

### Creating a Release

```bash
# Stable release
git checkout main
git tag v1.0.0
git push origin v1.0.0

# Beta / pre-release from develop
git checkout develop
git tag v1.1.0-beta.1
git push origin v1.1.0-beta.1
```

GitHub Actions will:
1. Build a self-contained Release exe
2. Embed your API keys (from GitHub Secrets) using AES-256 encryption
3. Package it into an Inno Setup installer
4. Create a GitHub Release with the installer attached

### Project Structure

```
IVAO-Sector-File-Creator/
├── .github/
│   ├── workflows/
│   │   ├── build.yml          # CI - builds on every PR/push
│   │   └── release.yml        # Release - triggered by version tags
│   └── pull_request_template.md
├── scripts/
│   └── push-secrets.ps1       # Push secrets.json → GitHub Secrets
├── MainForm.*.cs               # MainForm partial classes (one per page)
├── ConfigManager.cs            # Encrypted key storage (DPAPI + AES-256)
├── SecretsEmbed.cs             # Compile-time AES-256 key store (CI only)
├── UpdateManager.cs            # GitHub Releases auto-updater
├── installer.iss               # Inno Setup installer script
├── secrets.example.json        # Template - copy to secrets.json, fill in keys
├── secrets.json                # Your local keys (gitignored, never committed)
└── LICENSE
```

### Contributing

1. Fork the repo and create a feature branch from `develop`.
2. Fill in `secrets.json` with your own keys.
3. Run `dotnet build` to verify.
4. Open a PR targeting **`develop`** (not `main`).
5. The PR template checklist must pass - especially: no secrets in the diff.

---

## Security

- API keys are **never** stored in source code.
- Release builds embed keys using **AES-256-CBC + PBKDF2** - no plaintext file ships with the installer.
- On first launch the app decrypts keys into the Windows **DPAPI**-encrypted `%AppData%` store.
- Contributors use their own free-tier keys via `secrets.json` (gitignored).

---

## License

MIT - see [LICENSE](LICENSE).
Built by [Veda Moola](https://ivao.aero/Member.aspx?Id=656077) (IVAO VID 656077).
Not affiliated with IVAO.
