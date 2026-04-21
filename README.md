# IVAO Sector File Creator

> **⚠ NOT FOR REAL WORLD USE** - simulation purposes only.
> This is an **unofficial**, community-made tool. It is not affiliated with or endorsed by IVAO.

A Windows desktop application for building IVAO sector files. Pulls real-world aviation data from free, publicly available APIs to generate airport data, FIR boundaries, SID/STAR procedures, flight schedules, and ground layouts — all in one place.

---

## Table of Contents

- [Features](#features)
- [For End Users](#for-end-users)
  - [Installation](#installation)
  - [Logging In](#logging-in)
  - [Using the App](#using-the-app)
  - [Updates](#updates)
- [For Developers](#for-developers)
  - [Prerequisites](#prerequisites)
  - [Getting Started](#getting-started)
  - [API Keys](#api-keys)
  - [Project Structure](#project-structure)
  - [How the App Works](#how-the-app-works)
  - [Branch Strategy](#branch-strategy)
  - [Contributing](#contributing)
  - [Creating a Release](#creating-a-release)
  - [Push Secrets to GitHub](#push-secrets-to-github-repo-owner-only)
- [Security](#security)
- [Contributors](#contributors)
- [License](#license)

---

## Features

| Page | What it does |
|---|---|
| **Airport Data** | Fetches airports, runways, frequencies, VOR/DME, and NDB for any FIR region |
| **FIR / Airspace** | Downloads FIR, TMA, CTR, and restricted airspace boundaries |
| **SID & STAR** | Generates SID/STAR procedure files for a given airport ICAO code |
| **Flight Schedules** | Real-world departures and arrivals with Excel export |
| **Ground Layout** | Imports KML ground layouts and converts them to sector file format |

---

## For End Users

### Installation

1. Go to the [Releases](https://github.com/APRILDAY23/IVAO-Sector-File-Creator/releases) page.
2. Download the latest `IVAO-Sector-File-Creator-Setup-vX.X.X.exe`.
3. Run the installer and follow the wizard. A Start Menu shortcut is created automatically. An optional Desktop shortcut can be added during setup.
4. Launch **IVAO Sector File Creator** from the Start Menu.

**Requirements:** Windows 10 or later (64-bit). No additional software needed.

> You do not need to configure any API keys. The app comes fully ready to use out of the box.

---

### Logging In

The app uses **IVAO SSO (OAuth 2.0)** for authentication — the same login you use on the IVAO website.

1. Click **Continue with IVAO SSO**.
2. Your browser opens the IVAO login page.
3. Log in with your IVAO credentials.
4. The browser closes automatically and you are taken to the dashboard.

Your credentials are never stored by this app. Only your IVAO VID and name are used to identify your session.

> **Access is currently restricted to IVAO staff.** If you believe you should have access, contact the maintainer.

---

### Using the App

After logging in you land on the **Home** dashboard. Use the left sidebar to navigate between tools:

#### Airport Data
- Enter a **FIR ICAO code** (e.g. `VOMF`) and click **Fetch**.
- The app retrieves all airports, runways, radio frequencies, VORs, DMEs, and NDBs within that FIR.
- Export the result to a sector file-compatible format using the **Export** button.

#### FIR / Airspace
- Enter a **FIR ICAO code** and select the airspace types you want (FIR, TMA, CTR, restricted areas).
- Click **Fetch** to download boundary coordinates.
- Export to sector file format.

#### SID & STAR
- Enter an **airport ICAO code** (e.g. `VOBL`).
- The app fetches all published SID and STAR procedures for that airport.
- Select individual procedures or export all at once.

#### Flight Schedules
- Enter an **airport ICAO code** to view real-world departures and arrivals.
- Filter by date, airline, or direction.
- Export to Excel with one click.

#### Ground Layout
- Click **Import KML** and select a `.kml` file exported from Google Earth or similar tools.
- The app converts the KML geometry into sector file format for ground layout use.

---

### Updates

The app checks for updates automatically each time it starts. When an update is available a notification appears in **Settings → Updates**.

#### Update Channels

| Channel | Description |
|---|---|
| **Stable** | Tested, production-ready releases from `main` |
| **Beta** | Development builds from `develop` - may contain new features or bugs |

To switch channels: go to **Settings → Updates → Channel**, select **Stable** or **Beta**. The app immediately checks for a newer version on the selected channel. If one is found, click **Install Update** - the installer downloads silently and the app restarts automatically.

---

## For Developers

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Visual Studio 2022 or any IDE with C# / WinForms support
- [GitHub CLI](https://cli.github.com) (only needed if you are the repo owner pushing secrets)

---

### Getting Started

```bash
git clone https://github.com/APRILDAY23/IVAO-Sector-File-Creator.git
cd IVAO-Sector-File-Creator

# Copy the secrets template and fill in your own free-tier API keys
cp secrets.example.json secrets.json

dotnet build
dotnet run
```

On the first Debug run the app reads `secrets.json`, encrypts the values into the Windows DPAPI store at `%AppData%\IVAOSectorFileCreator\`, then deletes `secrets.json` from the output folder. Your keys persist across builds — you only need the file once.

---

### API Keys

The app relies on several free-tier APIs. Get your own keys to develop locally:

| Key in `secrets.json` | Where to get it |
|---|---|
| `openAipApiKey` | [app.openaip.net](https://app.openaip.net) - Account → API Key |
| `ivaoApiKey` | [api.ivao.aero](https://api.ivao.aero) developer portal |
| `mapboxToken` | [account.mapbox.com](https://account.mapbox.com) |
| `oauthClientId` / `oauthClientSecret` | Register an OAuth app at [sso.ivao.aero](https://sso.ivao.aero) |
| `aeroDataBoxApiKey` | [rapidapi.com](https://rapidapi.com) - subscribe to AeroDataBox (free tier) |

Fill these into `secrets.json` (copied from `secrets.example.json`). This file is gitignored and must never be committed.

---

### Project Structure

```
IVAO-Sector-File-Creator/
├── .github/
│   ├── workflows/
│   │   ├── build.yml             # CI - builds and validates every PR and push
│   │   ├── release.yml           # Release - triggered by version tags (owner only)
│   │   └── contributors.yml      # Auto-updates CONTRIBUTORS section on PR merge
│   ├── CODEOWNERS                # Requires owner approval on all PRs
│   └── pull_request_template.md  # PR checklist
├── scripts/
│   └── push-secrets.ps1          # Syncs secrets.json to GitHub Actions Secrets
├── MainForm.cs                   # Main window shell and OAuth login flow
├── MainForm.Airport.cs           # Airport Data page logic
├── MainForm.Fir.cs               # FIR / Airspace page logic
├── MainForm.SidStar.cs           # SID & STAR page logic
├── MainForm.FlightSchedules.cs   # Flight Schedules page logic
├── MainForm.Ground.cs            # Ground Layout page logic
├── MainForm.Country.cs           # Country selector logic
├── MainForm.Settings.cs          # Settings page (theme, updates)
├── MainForm.Designer.cs          # WinForms designer-generated layout
├── ConfigManager.cs              # Encrypted key storage (DPAPI + AES-256)
├── SecretsEmbed.cs               # Compile-time AES-256 key store (CI only, empty in source)
├── UpdateManager.cs              # GitHub Releases auto-updater
├── Login.cs                      # Standalone login form
├── installer.iss                 # Inno Setup installer script
├── secrets.example.json          # Template for contributors - copy and fill in
├── secrets.json                  # Your local keys (gitignored, never committed)
└── LICENSE
```

---

### How the App Works

#### Authentication
The app uses IVAO's OAuth 2.0 SSO. On login it:
1. Generates a cryptographically random `state` parameter to prevent CSRF attacks.
2. Opens the IVAO SSO URL in the system browser.
3. Starts a local HTTP listener on `http://localhost:5000/callback`.
4. When IVAO redirects back, validates the `state` and exchanges the code for an access token.
5. Fetches the user profile from `api.ivao.aero/v2/users/me` and checks the `isStaff` flag.

#### Key Storage
Keys flow through three stages:

```
Development:   secrets.json  →  read on first run  →  DPAPI (%AppData%)  →  deleted
Release build: GitHub Secrets  →  CI encrypts (AES-256-CBC + PBKDF2 600k)  →  embedded in binary  →  decrypted to DPAPI on first launch
Runtime:       ConfigManager reads from DPAPI store for all API calls
```

#### Auto-Updater
On startup `UpdateManager` calls the GitHub Releases API, compares the latest release version on the user's channel (Stable or Beta) against the running assembly version, and sets the update badge in Settings if a newer version is found. When the user clicks Install, the installer is downloaded to `%TEMP%` and launched with `/SILENT /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS`. The app exits and restarts on the new version.

---

### Branch Strategy

| Branch | Purpose | Protected |
|---|---|---|
| `main` | Production - stable, fully tested | Yes - PR + owner approval + CI required |
| `develop` | Staging - active development | Yes - PR + owner approval + CI required |

- All contributor PRs target **`develop`**.
- The repo owner merges `develop` into `main` when ready for a stable release.
- No direct pushes to either branch - everything goes through a PR.
- All conversations on a PR must be resolved before merging.

---

### Contributing

1. Fork the repository.
2. Create a feature branch from `develop`:
   ```bash
   git checkout develop
   git checkout -b feature/your-feature-name
   ```
3. Copy `secrets.example.json` to `secrets.json` and fill in your own free-tier API keys.
4. Make your changes and run `dotnet build` to verify.
5. Open a PR targeting **`develop`** (not `main`).
6. The CI build must pass and all PR checklist items must be completed.
7. The repo owner will review and approve. Once approved and merged, you are automatically added to the Contributors list.

> **Do not commit `secrets.json` or any API keys.** The CI will reject the PR if secrets are detected in the diff.

---

### Creating a Release

Only the repo owner (`@APRILDAY23`) can create releases. Tags pushed by anyone else are ignored by the release workflow.

```bash
# Stable release from main
git checkout main
git tag v1.2.0
git push origin v1.2.0

# Beta / pre-release from develop
git checkout develop
git tag v1.2.0-beta.1
git push origin v1.2.0-beta.1
```

GitHub Actions then automatically:
1. Extracts the version number from the tag.
2. Fetches API keys from GitHub Secrets and encrypts them using AES-256-CBC with a PBKDF2-SHA256 derived key (600,000 iterations).
3. Patches `SecretsEmbed.cs` with the encrypted payload before compiling - no plaintext keys ever enter the binary.
4. Publishes a self-contained `win-x64` executable.
5. Packages it into an Inno Setup installer.
6. Creates a GitHub Release with the installer attached. Tags containing `-` (e.g. `-beta.1`) are published as pre-releases.

---

### Push Secrets to GitHub (repo owner only)

Contributors do **not** push secrets. The script is locked to the repo owner's GitHub account and will refuse to run if authenticated as anyone else.

Run this after approving and merging a PR that introduces a new API key:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/push-secrets.ps1
```

The script:
- Verifies your GitHub identity matches `@APRILDAY23`
- Skips secrets already present on GitHub (safe to re-run)
- Requires the GitHub CLI installed and authenticated (`gh auth login`)

---

## Security

| Area | Implementation |
|---|---|
| **API key storage** | Windows DPAPI-encrypted store at `%AppData%` - tied to your Windows account |
| **Release key embedding** | AES-256-CBC, PBKDF2-SHA256 key derivation with 600,000 iterations, random IV per release |
| **Source code** | No keys, no passphrase, no ciphertext in the repository - all empty in source |
| **OAuth flow** | Cryptographically random state per login, validated on callback to prevent CSRF |
| **Release workflow** | Restricted to `@APRILDAY23` via `if: github.actor` check |
| **Branch protection** | PR + owner approval + CI pass required on both `main` and `develop` |
| **Secrets file** | `secrets.json` is gitignored and auto-deleted from the output folder after first run |

---

## Contributors

<!-- CONTRIBUTORS-START -->
<a href="https://github.com/APRILDAY23"><img src="https://github.com/APRILDAY23.png" width="48" height="48" alt="APRILDAY23" title="APRILDAY23"/></a>
<!-- CONTRIBUTORS-END -->

---

## License

MIT - see [LICENSE](LICENSE).
Built by [Veda Moola](https://ivao.aero/Member.aspx?Id=656077) (IVAO VID 656077).
Not affiliated with IVAO.
