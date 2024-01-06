using Plugin.MenuItems;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Plugin
{
    /// <summary>
    /// Provides access to plugin settings
    /// </summary>
    public abstract class PluginSettings : IPlugin
    {
        private readonly Dictionary<string, string> settings;
        private readonly string settingsFile;

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract string Author { get; }

        /// <inheritdoc />
        public abstract Version Version { get; }

        /// <inheritdoc />
        public abstract Uri? Url { get; }

        /// <inheritdoc />
        public abstract event MessageEventHandler Message;

        /// <inheritdoc />
        public abstract IMenuDescriptor GetDescriptor();

        /// <inheritdoc />
        public abstract void OnClipboardUpdate();

        /// <inheritdoc />
        public abstract void OnMessage(IPlugin? sender, PluginEventArgs eventArgs);

        /// <summary>
        /// Initializes settings
        /// </summary>
        protected PluginSettings()
        {
            var name = $"{GetType().FullName}.json".Replace('<', '{').Replace('>', '}');
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            settingsFile =
                Path.Combine(
                Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)!,
                name);
            try
            {
                var contents = File.ReadAllText(settingsFile);
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(contents);
                settings = data ?? [];
            }
            catch (Exception ex)
            {
                Debug.Print($"Unable to load settings. {ex.Message}");
                settings = [];
            }
        }

        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="key">Setting key</param>
        /// <param name="defaultValue">Default value in case <paramref name="key"/> is not found</param>
        /// <returns>Value</returns>
        /// <remarks>This call is thread safe</remarks>
        protected T? GetValue<T>(string key, T? defaultValue = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            lock (settings)
            {
                if (settings.TryGetValue(key, out var value))
                {
                    return JsonSerializer.Deserialize<T>(value);
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Sets a setting value
        /// </summary>
        /// <param name="key">Key to set</param>
        /// <param name="value">Value to set. null deletes the setting</param>
        /// <remarks>This call is thread safe</remarks>
        protected void SetValue<T>(string key, T? value)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            lock (settings)
            {
                settings[key] = JsonSerializer.Serialize(value);
                SaveSettings();
            }
        }

        /// <summary>
        /// Clears all settings
        /// </summary>
        /// <remarks>This call is thread safe</remarks>
        protected void ClearSettings()
        {
            lock (settings)
            {
                settings.Clear();
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            var data = JsonSerializer.Serialize(settings);
            File.WriteAllText(settingsFile, data);
        }
    }
}
