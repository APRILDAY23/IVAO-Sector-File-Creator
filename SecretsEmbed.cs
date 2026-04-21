using System;
using System.Security.Cryptography;
using System.Text;

namespace Sector_File
{
    // ─────────────────────────────────────────────────────────────────────────
    //  SecretsEmbed  —  AES-256-CBC encrypted key store
    //
    //  Source build : _data is empty. No keys, no cipher text, nothing.
    //  Release build: GitHub Actions derives the AES key (matching _s + _p),
    //                 encrypts each API key with AES-256-CBC, and replaces
    //                 _data and _iv before compiling. Real key values never
    //                 appear as plaintext anywhere in the binary or installer.
    //
    //  Runtime      : SeedOnce() derives the same key, decrypts, writes to the
    //                 DPAPI-encrypted %AppData% store. After that the in-memory
    //                 constants are no longer needed.
    //
    //  Security     : Breaking this requires decompiling the binary, locating
    //                 _s, _p, _iv, and _data, reconstructing the PBKDF2 call
    //                 with the exact same parameters, and running AES decryption.
    //                 That is enough friction for free-tier API keys.
    // ─────────────────────────────────────────────────────────────────────────
    internal static class SecretsEmbed
    {
        // PBKDF2 salt  — 16 random bytes, fixed at build time
        private static readonly byte[] _s =
        {
            0x3E, 0x9A, 0x1C, 0x7F, 0x52, 0xB4, 0xD8, 0x2A,
            0x6B, 0xF3, 0x45, 0xC9, 0x18, 0x7E, 0xA2, 0x5D,
        };

        // PBKDF2 passphrase — combined with salt to derive the 256-bit AES key
        private const string _p = "IVXSec.24!kF@zR9";

        // AES IV — populated by release workflow, empty in source
        private static readonly byte[] _iv = { };

        // Encrypted payload: flat array [keyName, cipherB64, keyName, cipherB64, ...]
        // Populated by release workflow, empty in source
        private static readonly string[] _data = { };

        // ── Public API ───────────────────────────────────────────────────────

        internal static void SeedOnce()
        {
            if (_data.Length == 0 || _iv.Length == 0) return;
            try
            {
                byte[] key = DeriveKey();
                for (int i = 0; i + 1 < _data.Length; i += 2)
                {
                    string name = _data[i];
                    string val  = Decrypt(_data[i + 1], key);
                    if (!string.IsNullOrEmpty(val))
                        ConfigManager.SetKeyIfEmpty(name, val);
                }
            }
            catch { /* if decryption fails, user configures via Settings */ }
        }

        // ── Key derivation ───────────────────────────────────────────────────

        private static byte[] DeriveKey()
        {
            using var kdf = new Rfc2898DeriveBytes(
                _p, _s,
                iterations: 2000,
                hashAlgorithm: HashAlgorithmName.SHA256);
            return kdf.GetBytes(32); // 256-bit
        }

        // ── AES-256-CBC decryption ───────────────────────────────────────────

        private static string Decrypt(string cipherB64, byte[] key)
        {
            try
            {
                byte[] cipher = Convert.FromBase64String(cipherB64);
                using var aes = Aes.Create();
                aes.Key     = key;
                aes.IV      = _iv;
                aes.Mode    = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using var dec = aes.CreateDecryptor();
                byte[] plain = dec.TransformFinalBlock(cipher, 0, cipher.Length);
                return Encoding.UTF8.GetString(plain);
            }
            catch { return ""; }
        }
    }
}
