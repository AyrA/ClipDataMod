using Plugin;
using System.Diagnostics;
using System.Reflection;

namespace ClipDataMod
{
    internal static class Program
    {
        private static ClipboardEventFilter? _eventFilter;
        private static MenuHandler? _menuHandler;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Idle += Init;
            Application.Run();
            //Perform cleanup
            _eventFilter?.Dispose();
            _menuHandler?.Dispose();
            PluginHelper.Temp.Dispose();
        }

        private static void Init(object? sender, EventArgs e)
        {
            Application.Idle -= Init;

            //Register event filter
            _eventFilter = new ClipboardEventFilter();
            _eventFilter.Register();
            Application.AddMessageFilter(_eventFilter);
            //Create context menu
            _menuHandler = new MenuHandler();
            _eventFilter.ClipboardChanged += (_) => _menuHandler.UpdateMenu();
            //Register clipboard helper functions
            PluginHelper.InitHelper(
                _eventFilter.RunOnUiThread,
                new WindowsClipboard(),
                new WindowsDialog(_menuHandler.NotifyIcon));
            SearchPlugins();
        }

        private static void SearchPlugins()
        {
            //load internal plugins first
            var plugins = PluginHelper.LoadPlugins(Assembly.GetExecutingAssembly());
            foreach (var plugin in plugins)
            {
                _menuHandler?.AddMenuItem(plugin);
            }
            //Load external plugins
            var pluginDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath)!, "Plugins");
            if (Directory.Exists(pluginDir))
            {
                foreach (var dir in Directory.GetDirectories(pluginDir))
                {
                    foreach (var file in Directory.GetFiles(dir, "*.dll"))
                    {
                        Assembly a;
                        try
                        {
                            a = Assembly.LoadFile(file);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print($"Unable to load {file} as assembly. {ex.Message}");
                            continue;
                        }
                        try
                        {
                            plugins = PluginHelper.LoadPlugins(a);
                        }
                        catch (Exception ex)
                        {
                            PluginHelper.Dialog.Exception(ex, "Failed to load plugins", null);
                        }
                    }
                }
            }
        }
    }
}