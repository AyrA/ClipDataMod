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

            //Register event filter to catch clipboard change event
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

            //Load all plugins
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
            foreach (var info in PluginHelper.SearchPlugins())
            {
                Assembly a;
                try
                {
                    a = Assembly.LoadFile(info.FileName);
                }
                catch (Exception ex)
                {
                    Debug.Print($"Unable to load {info.FileName} as assembly. {ex.Message}");
                    continue;
                }
                try
                {
                    plugins = PluginHelper.LoadPlugins(a, info.Excludes);
                }
                catch (Exception ex)
                {
                    PluginHelper.Dialog.Exception(ex, "Failed to load plugins", null);
                }
            }
        }
    }
}