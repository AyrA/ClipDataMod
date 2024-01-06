using Plugin.Clipboard;
using Plugin.UI;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Plugin
{
    /// <summary>
    /// Provides plugin functionality
    /// </summary>
    public static class PluginHelper
    {
        /// <summary>
        /// The data format string used to internally represent binary data in the clipboard
        /// </summary>
        public const string BinaryFormatName = "ClipDataMod.Binary";

        private static Action<Action>? _syncFunc;
        private static IClipboard _clipboard = new ClipboardPlaceholder();
        private static IDialog _dialog = new DialogPlaceholder();
        private static readonly List<IPlugin> _plugins = [];
        private static readonly List<EventProcessor> _pluginEventHandlers = [];
        private static readonly TempFiles _tempFiles = new();
        private static readonly object _lock = new();

        /// <summary>
        /// Gets clipboard access
        /// </summary>
        public static IClipboard Clipboard => _clipboard;

        /// <summary>
        /// Gets UI access
        /// </summary>
        public static IDialog Dialog => _dialog;

        /// <summary>
        /// Gets access to a temporary file store with auto cleanup
        /// </summary>
        public static TempFiles Temp => _tempFiles;

        /// <summary>
        /// Runs the supplied action synchronously on the main thread,
        /// blocking the current thread until the action has been run
        /// </summary>
        /// <param name="callback">Callback to invoke synchronously</param>
        /// <remarks>
        /// If the current thread is the main thread,
        /// the action will be called immediately
        /// </remarks>
        public static void Sync(Action callback)
        {
            ArgumentNullException.ThrowIfNull(callback);
            var sync = _syncFunc
                ?? throw new InvalidOperationException("Sync function has not been set");
            sync(callback);
        }

        /// <summary>
        /// Initializes ClipDataMod to work with the current environment
        /// </summary>
        /// <param name="syncFunc">Function used as backend to <see cref="Sync(Action)"/></param>
        /// <param name="clipboard">Clipboard handler with operating system specific functionality</param>
        /// <param name="dialog">Dialog handler with operating system native dialog support</param>
        /// <remarks>
        /// This method can only be called once.
        /// Subsequent calls will throw an exception
        /// </remarks>
        public static void InitHelper(Action<Action> syncFunc, IClipboard clipboard, IDialog dialog)
        {
            ArgumentNullException.ThrowIfNull(syncFunc);
            ArgumentNullException.ThrowIfNull(clipboard);
            ArgumentNullException.ThrowIfNull(dialog);
            lock (_lock)
            {
                if (_syncFunc != null)
                {
                    throw new InvalidOperationException("Plugin helper has already been initialized");
                }
                _syncFunc = syncFunc;
                _clipboard = clipboard;
                _dialog = dialog;
            }
        }

        /// <summary>
        /// Searches for plugins in the standard plugin folder
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PluginInfo> SearchPlugins()
        {
            var pluginDir = Path.Combine(AppContext.BaseDirectory, "Plugins");
            try
            {
                Directory.CreateDirectory(pluginDir);
            }
            catch
            {
                //NOOP
            }
            if (Directory.Exists(pluginDir))
            {
                foreach (var dir in Directory.GetDirectories(pluginDir))
                {
                    var infoFile = Path.Combine(dir, "info.json");
                    if (File.Exists(infoFile))
                    {
                        PluginInfo? info;
                        try
                        {
                            info = JsonSerializer.Deserialize<PluginInfo>(File.ReadAllText(infoFile));
                            if (info == null)
                            {
                                throw new Exception("Failed to deserialize JSON into valid PluginInfo structure");
                            }
                            info.ToFullPath(dir);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print($"Unable to load plugin info from {infoFile}.");
                            Debug.Print(ex.ToString());
                            continue;
                        }
                        yield return info;
                    }
                }
            }
        }

        /// <summary>
        /// Loads plugins from the given assembly
        /// </summary>
        /// <param name="a">Assembly to load plugins from</param>
        /// <param name="excludes">Full type names to not load</param>
        /// <returns>Loaded plugins</returns>
        /// <remarks>
        /// This will instantiate every plugin instance
        /// and call <see cref="AddPlugin(IPlugin)"/> for each one
        /// </remarks>
        public static IPlugin[] LoadPlugins(Assembly a, string[]? excludes = null)
        {
            excludes ??= [];
            var ret = new List<IPlugin>();
            var pluginInterfaceType = typeof(IPlugin);
            var pluginInterfaceName = pluginInterfaceType.FullName ?? pluginInterfaceType.Name;
            foreach (var t in a.GetTypes())
            {
                if (excludes.Contains(t.FullName))
                {
                    Debug.Print(
                        "Plugin {0} is skipped because it's contained in the exclusion list",
                        t.FullName);
                    continue;
                }

                if (t.GetInterface(pluginInterfaceName) != null)
                {
                    Debug.Print("Loading plugin from {0}", t);
                    var c = t.GetConstructor(Type.EmptyTypes)
                        ?? throw new InvalidOperationException($"Plugin {t} has no parameterless constructor");
                    var p = (IPlugin)c.Invoke([]);
                    ret.Add(p);
                }
            }
            //Only add plugins if they all loaded successfully
            ret.ForEach((p) => AddPlugin(p));
            return [.. ret];
        }

        /// <summary>
        /// Loads plugins from the given assembly file (usually a DLL)
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="excludes">Full type names to not load</param>
        /// <returns>Loaded plugins</returns>
        public static IPlugin[] LoadPlugins(string fileName, string[]? excludes = null)
        {
            return LoadPlugins(Assembly.LoadFrom(fileName), excludes);
        }

        /// <summary>
        /// Adds a plugin instance to the plugin list
        /// </summary>
        /// <param name="p">Plugin instance</param>
        /// <returns>true, if the plugin was added, false if it's already loaded</returns>
        public static bool AddPlugin(IPlugin p)
        {
            if (!_plugins.Contains(p))
            {
                _plugins.Add(p);
                _pluginEventHandlers.Add(new EventProcessor(p));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a plugin from the plugin list
        /// </summary>
        /// <param name="p">Plugin instance</param>
        /// <returns>true, if removed</returns>
        /// <remarks>
        /// The plugin is not unloaded from memory,
        /// it just no longer gets a menu entry or event notifications
        /// </remarks>
        public static bool RemovePlugin(IPlugin p)
        {
            return _plugins.Remove(p);
        }

        /// <summary>
        /// Gets an array of all loaded plugins
        /// </summary>
        /// <returns>Array of plugins. Empty array if none are loaded</returns>
        public static IPlugin[] GetPlugins()
        {
            return [.. _plugins];
        }

        /// <summary>
        /// Sends a clipboard update event to all loaded plugins
        /// </summary>
        /// <remarks>
        /// Plugins that throw an exception will be removed from the event notification list,
        /// but can be added again using <see cref="AddPlugin(IPlugin)"/>
        /// </remarks>
        public static void OnClipboardUpdate()
        {
            if (_clipboard.MustSync)
            {
                var cache = _plugins.ToArray();
                foreach (var p in cache)
                {
                    try
                    {
                        p.OnClipboardUpdate();
                    }
                    catch (Exception ex)
                    {
                        _plugins.Remove(p);
                        var name = p.GetType().FullName ?? "Plugin";
                        try
                        {
                            name = p.GetDescriptor().Title ?? name;
                        }
                        catch
                        {
                            //NOOP
                        }
                        Dialog.Exception(ex,
                            "Plugin threw an exception when handling a clipboard update event and will be excluded from future events", p);
                    }
                }
            }
        }

        /// <summary>
        /// Forwards an event to all plugins
        /// </summary>
        /// <param name="sender">Plugin source</param>
        /// <param name="eventArgs">Event arguments</param>
        internal static void HandlePluginMessage(IPlugin sender, PluginEventArgs eventArgs)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(eventArgs);
            foreach (var p in _pluginEventHandlers)
            {
                if (!eventArgs.Cancel)
                {
                    p.SendMessage(sender, eventArgs);
                }
            }
        }

        private class EventProcessor
        {
            private readonly IPlugin _plugin;
            private bool enabled = true;

            public EventProcessor(IPlugin plugin)
            {
                _plugin = plugin;
                plugin.Message += PluginMessageHandler;
            }

            public void Reset() => enabled = false;

            public bool IsFaulted() => !enabled;

            public void SendMessage(IPlugin? sender, PluginEventArgs eventArgs)
            {
                if (!enabled)
                {
                    return;
                }

                try
                {
                    _plugin.OnMessage(sender, eventArgs);
                    //TODO
                }
                catch (Exception ex)
                {
                    enabled = false;
                    Dialog.Exception(ex, "Plugin threw an exception when processing a message and will be locked out from receiving future messages", _plugin);
                }
            }

            private void PluginMessageHandler(string messageType, object? messageData)
            {
                if (!enabled)
                {
                    return;
                }
                try
                {
                    ArgumentException.ThrowIfNullOrEmpty(messageType);
                    if (messageType.Contains("ClipDataMod", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new ArgumentException("Message type contains forbidden string 'ClipDataMod'");
                    }
                    if (Encoding.UTF8.GetBytes(messageType).Any(m => m < 0x20 || m >= 0x7F))
                    {
                        throw new ArgumentException("Message type contains forbidden characters");
                    }
                }
                catch (ArgumentException ex)
                {
                    enabled = false;
                    Dialog.Exception(ex, "Plugin threw an exception when generating a message and will be locked out from message generation.", _plugin);
                    return;
                }
                HandlePluginMessage(_plugin, new PluginEventArgs(messageType, messageData));
            }
        }
    }
}
