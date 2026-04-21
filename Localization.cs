using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Sector_File
{
    internal static class Localization
    {
        private static Dictionary<string, string> _strings = new();
        private static string _currentCode = "en";

        private static readonly string LangsDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "langs");

        // ── Load a language by code (e.g. "en", "fr") ────────────────────────
        internal static void Load(string code)
        {
            string path = Path.Combine(LangsDir, $"{code}.json");
            if (!File.Exists(path))
                path = Path.Combine(LangsDir, "en.json");

            try
            {
                var obj = JObject.Parse(File.ReadAllText(path));
                var strings = obj["strings"] as JObject;
                _strings.Clear();
                if (strings != null)
                    foreach (var prop in strings.Properties())
                        _strings[prop.Name] = prop.Value?.ToString() ?? prop.Name;
                _currentCode = code;
            }
            catch { }
        }

        // ── Get a translated string, fallback to the key itself ───────────────
        internal static string Get(string key) =>
            _strings.TryGetValue(key, out var val) ? val : key;

        internal static string CurrentCode => _currentCode;

        // ── List all available language packs in the langs folder ─────────────
        internal static List<(string Code, string Name)> Available()
        {
            var result = new List<(string Code, string Name)>();
            if (!Directory.Exists(LangsDir)) return result;

            foreach (var file in Directory.GetFiles(LangsDir, "*.json"))
            {
                try
                {
                    var obj = JObject.Parse(File.ReadAllText(file));
                    string code = Path.GetFileNameWithoutExtension(file);
                    string name = obj["language"]?.ToString() ?? code;
                    result.Add((code, name));
                }
                catch { }
            }

            result.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            return result;
        }
    }
}
