namespace Sector_File
{
    internal static class AppInfo
    {
        public const string Name = "IVAO Sector File Creator";

        public static string Version
        {
            get
            {
                var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return v != null ? $"v{v.Major}.{v.Minor}.{v.Build}" : "v1.0.0";
            }
        }

        public static System.Version AssemblyVersion =>
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version
            ?? new System.Version(1, 0, 0);
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Load persisted config (API keys etc.) before any form opens
            ConfigManager.Load();
            SecretsEmbed.SeedOnce();         // decode compile-time keys into %AppData% (release builds)
            ConfigManager.LoadSecretsFile(); // overlay keys from secrets.json if present (dev only)
            ConfigManager.SeedDefaults();    // no-op in open-source builds

            // Load saved language pack
            Localization.Load(ConfigManager.Language);

            // Show splash (blocks until startup checks complete)
            using (var splash = new SplashForm())
                splash.ShowDialog();

            // Single main window for the entire application
            Application.Run(new MainForm());
        }
    }
}
