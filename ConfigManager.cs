using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    // ─────────────────────────────────────────────────────────────────────────
    //  ConfigManager  -  loads / saves application configuration
    //
    //  Encrypted store: %AppData%\IVAOSectorFileCreator\config.json
    //  Developer keys : secrets.json  (project root, gitignored)
    //
    //  On startup call order:
    //    1. Load()             - read encrypted store from %AppData%
    //    2. LoadSecretsFile()  - overlay keys from secrets.json (dev only)
    //    3. SeedDefaults()     - no-op in open-source builds
    //
    //  Sensitive values are encrypted with Windows DPAPI so they are only
    //  readable by the current Windows user on this machine.
    // ─────────────────────────────────────────────────────────────────────────
    internal static class ConfigManager
    {
        // ── Paths ────────────────────────────────────────────────────────────
        private static readonly string ConfigDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IVAOSectorFileCreator");

        private static readonly string ConfigFile = Path.Combine(ConfigDir, "config.json");

        // secrets.json lives next to the .exe (copied from project root via .csproj)
        private static readonly string SecretsFile = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "secrets.json");

        // ── In-memory cache ──────────────────────────────────────────────────
        private static JObject _cache = new();

        // ── Keys ─────────────────────────────────────────────────────────────
        public static string OpenAipApiKey
        {
            get => GetSecure("openAipApiKey");
            set => SetSecure("openAipApiKey", value);
        }

        public static string IvaoApiKey
        {
            get => GetSecure("ivaoApiKey");
            set => SetSecure("ivaoApiKey", value);
        }

        public static string MapboxToken
        {
            get => GetSecure("mapboxToken");
            set => SetSecure("mapboxToken", value);
        }

        public static string AviationStackApiKey
        {
            get => GetSecure("aviationStackApiKey");
            set => SetSecure("aviationStackApiKey", value);
        }

        public static string AeroDataBoxApiKey
        {
            get => GetSecure("aeroDataBoxApiKey");
            set => SetSecure("aeroDataBoxApiKey", value);
        }

        // ── Flight request daily limit ────────────────────────────────────────
        private static string FlightRequestDate
        {
            get => GetSecure("flightReqDate");
            set => SetSecure("flightReqDate", value);
        }

        private static int FlightRequestCount
        {
            get => int.TryParse(GetSecure("flightReqCount"), out int n) ? n : 0;
            set => SetSecure("flightReqCount", value.ToString());
        }

        public static int GetTodayFlightRequests()
        {
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (FlightRequestDate != today) { FlightRequestDate = today; FlightRequestCount = 0; }
            return FlightRequestCount;
        }

        public static bool TryIncrementFlightRequest()
        {
            if (GetTodayFlightRequests() >= 10) return false;
            FlightRequestCount++;
            return true;
        }

        public static string UpdateChannel
        {
            get => GetSecure("updateChannel", "stable");
            set => SetSecure("updateChannel", value);
        }

        public static string Language
        {
            get => GetSecure("language", "en");
            set => SetSecure("language", value);
        }

        public static string OAuthClientId
        {
            get => GetSecure("oauthClientId");
            set => SetSecure("oauthClientId", value);
        }

        public static string OAuthClientSecret
        {
            get => GetSecure("oauthClientSecret");
            set => SetSecure("oauthClientSecret", value);
        }

        // ── Lifecycle ────────────────────────────────────────────────────────

        /// <summary>Load encrypted config from %AppData% (call once at startup).</summary>
        public static void Load()
        {
            try
            {
                if (File.Exists(ConfigFile))
                    _cache = JObject.Parse(File.ReadAllText(ConfigFile));
            }
            catch
            {
                _cache = new JObject();
            }
        }

        /// <summary>
        /// Load developer keys from secrets.json next to the .exe.
        /// Keys present in the file always overwrite the encrypted store so
        /// editing secrets.json and restarting is enough to update any key.
        /// This file must never be committed - it is listed in .gitignore.
        /// </summary>
        public static void LoadSecretsFile()
        {
            if (!File.Exists(SecretsFile)) return;
            try
            {
                var obj = JObject.Parse(File.ReadAllText(SecretsFile));
                bool dirty = false;
                foreach (var prop in obj.Properties())
                {
                    string val = prop.Value?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(val)) continue;
                    _cache[prop.Name] = Protect(val);
                    dirty = true;
                }
                if (dirty) Save();
                // Delete from output directory after seeding so the file is
                // never present in a distributed build.
                try { File.Delete(SecretsFile); } catch { }
            }
            catch { /* malformed secrets.json - silently skip */ }
        }

        /// <summary>No default secrets are bundled. Configure keys via Settings or secrets.json.</summary>
        public static void SeedDefaults() { }

        /// <summary>Write a key only if it is not already set - used by SecretsEmbed.</summary>
        public static void SetKeyIfEmpty(string key, string value)
        {
            if (string.IsNullOrEmpty(GetSecure(key)))
                SetSecure(key, value);
        }

        /// <summary>Persist the in-memory cache to disk.</summary>
        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(ConfigDir);
                File.WriteAllText(ConfigFile,
                    _cache.ToString(Formatting.Indented),
                    Encoding.UTF8);
            }
            catch { /* non-fatal */ }
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static string GetSecure(string key, string defaultValue = "")
        {
            if (_cache[key] is JValue jv && jv.Value is string enc && enc.Length > 0)
            {
                try { return Unprotect(enc); }
                catch { /* fall through to default */ }
            }
            return defaultValue;
        }

        private static void SetSecure(string key, string value)
        {
            _cache[key] = string.IsNullOrEmpty(value) ? "" : Protect(value);
            Save();
        }

        // DPAPI encrypt/decrypt - tied to current user on this machine
        private static string Protect(string plaintext)
        {
            byte[] encrypted = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(plaintext),
                null,
                DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        private static string Unprotect(string ciphertext)
        {
            byte[] decrypted = ProtectedData.Unprotect(
                Convert.FromBase64String(ciphertext),
                null,
                DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
