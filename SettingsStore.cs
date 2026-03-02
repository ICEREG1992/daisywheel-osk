using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DaisywheelOsk
{

    //<!--Align Dropdown-->
    //<!--Wheel Opacity Slider-->
    //<!--Background Opacity Slider-->
    //<!--Theme dropdown-->
    //<!--Layout dropdown-->
    //<!--Show Hotkey-->
    //<!--Close Hotkey-->
    //<!--Backspace Hotkey-->
    //<!--Space Hotkey-->
    //<!--Enter Hotkey-->
    //<!--Tab Hotkey-->
    //<!--Launch on Boot Tickbox-->
    public class AppSettings
    {
        public bool StartOnBoot { get; set; } = false;
        public float Opacity { get; set; } = 100;
        public float BackgroundOpacity { get; set; } = 100;
        public float Size { get; set; } = 75;
        public string Theme { get; set; } = "default_theme";
        public string Layout { get; set; } = "default_layout";
        // Add more settings here...
    }

    public class SettingsStore
    {
        // ---------------------------------------------------------------
        // Singleton plumbing
        // ---------------------------------------------------------------
        private static readonly Lazy<SettingsStore> _instance =
            new(() => new SettingsStore());

        /// <summary>Access settings from anywhere: SettingsStore.Instance.Settings.Theme</summary>
        public static SettingsStore Instance => _instance.Value;

        private SettingsStore()
        {
            Load();
        }

        // ---------------------------------------------------------------
        // Config
        // ---------------------------------------------------------------
        private static readonly string SettingsPath =
            @"%appdata%\daisywheel-osk\settings.yaml";

        private static readonly IDeserializer Deserializer =
            new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()   // forward-compat: unknown keys won't throw
                .Build();

        private static readonly ISerializer Serializer =
            new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

        // ---------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------
        public AppSettings Settings { get; private set; } = new();

        /// <summary>Re-reads the YAML file and replaces the in-memory settings.</summary>
        public void Load()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                {
                    Settings = new AppSettings();
                    Save(); // write defaults so the file exists
                    return;
                }

                var yaml = File.ReadAllText(SettingsPath);
                Settings = Deserializer.Deserialize<AppSettings>(yaml) ?? new AppSettings();
            }
            catch (Exception ex)
            {
                // Log and fall back to defaults rather than crashing
                Console.Error.WriteLine($"[SettingsStore] Load failed: {ex.Message}");
                Settings = new AppSettings();
            }
        }

        /// <summary>Writes the current in-memory settings back to disk.</summary>
        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(SettingsPath)!;
                Directory.CreateDirectory(dir); // no-op if it already exists

                var yaml = Serializer.Serialize(Settings);
                File.WriteAllText(SettingsPath, yaml);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SettingsStore] Save failed: {ex.Message}");
            }
        }
    }
}