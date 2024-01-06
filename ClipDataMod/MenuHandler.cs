using ClipDataMod.Properties;
using Plugin;
using Plugin.MenuItems;

namespace ClipDataMod
{
    internal class MenuHandler : IDisposable
    {
        private readonly List<PluginMetadata> _plugins = [];
        private readonly List<ToolStripMenuItem> _items = [];
        private readonly NotifyIcon icon;
        private bool disposed = false;

        internal NotifyIcon NotifyIcon
        {
            get
            {
                ObjectDisposedException.ThrowIf(disposed, this);
                return icon;
            }
        }

        public MenuHandler()
        {
            var CMS = new ContextMenuStrip();
            icon = new NotifyIcon
            {
                Icon = Resources.ApplicationIcon,
                Visible = true,
                Text = "Clipboard operations",
                ContextMenuStrip = CMS
            };
            CMS.Items.Add(new ToolStripSeparator());
            var item = CMS.Items.Add("Exit");
            item.Click += delegate { Application.Exit(); };
        }

        public void AddMenuItem(IPlugin plugin)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            PluginMetadata meta;
            try
            {
                meta = new PluginMetadata(plugin);
                meta.Validate();
                CheckDuplicates(meta.MenuDescriptor);
                var cms = GetCMS();
                var item = ToItem(meta.MenuDescriptor);
                cms.Items.Insert(0, item);
                _plugins.Add(meta);
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Exception(ex,
                    "Plugin threw an exception when attempting to add it to the menu", plugin);
                return;
            }
        }

        public void UpdateMenu()
        {
            if (disposed)
            {
                return;
            }

            foreach (var item in _items)
            {
                UpdateMenuItem(item);
            }
        }

        public void Dispose()
        {
            lock (icon)
            {
                if (!disposed)
                {
                    icon.Visible = false;
                    disposed = true;
                    icon.ContextMenuStrip?.Dispose();
                    icon.Dispose();
                }
            }
        }

        private IPlugin? FindPlugin(IMenuDescriptor descriptor)
        {
            return _plugins.FirstOrDefault(m => m.HasDescriptor(descriptor))?.Plugin;
        }

        private static void CheckDuplicates(IMenuDescriptor descriptor)
        {
            List<IMenuDescriptor> descriptors = [];
            Stack<IMenuDescriptor> pending = new();
            pending.Push(descriptor);

            while (pending.Count > 0)
            {
                var current = pending.Pop();
                if (descriptors.Contains(current))
                {
                    throw new ArgumentException($"Descriptor contains duplicate entries. Menu item '{current.Title}' exists multiple times");
                }
                descriptors.Add(current);
                current.Items?.ForEach(pending.Push);
            }
        }

        private ToolStripMenuItem ToItem(IMenuDescriptor descriptor)
        {
            var items = GetSubItems(descriptor);
            var item = new ToolStripMenuItem(descriptor.Title, ToImage(descriptor.Icon), items);
            _items.Add(item);
            if (descriptor.OnClick != null)
            {
                item.Click += MenuItemClick;
            }
            item.Tag = descriptor;
            item.Enabled = descriptor.Enabled;
            item.Checked = descriptor.Checked;
            return item;
        }

        private ToolStripMenuItem[] GetSubItems(IMenuDescriptor descriptor)
        {
            if (descriptor.Items == null || descriptor.Items.Count == 0)
            {
                return [];
            }
            return descriptor.Items.Select(ToItem).ToArray();
        }

        private void UpdateMenuItem(ToolStripMenuItem item)
        {
            if (item.Tag is IMenuDescriptor descriptor)
            {
                try
                {
                    item.Image?.Dispose();
                    item.Image = descriptor.Icon != null ? ToImage(descriptor.Icon) : null;
                    item.Text = descriptor.Title;
                    item.Enabled = descriptor.Enabled;
                    item.Checked = descriptor.Checked;
                }
                catch (Exception ex)
                {
                    PluginHelper.Dialog.Exception(ex,
                        "Menu update threw an exception", FindPlugin(descriptor));
                }
            }
        }

        private static Image? ToImage(byte[]? data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }
            using var ms = new MemoryStream(data, false);
            return Image.FromStream(ms);
        }

        private void MenuItemClick(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem item)
            {
                return;
            }
            if (item.Tag is not IMenuDescriptor desc || desc.OnClick == null)
            {
                return;
            }
            try
            {
                desc.OnClick(desc);
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Exception(ex,
                    "Menu action of a plugin threw an exception", FindPlugin(desc));
            }
            UpdateMenuItem(item);
        }

        private ContextMenuStrip GetCMS()
        {
            return icon.ContextMenuStrip
                ?? throw new InvalidOperationException("Context menu has not been set");
        }

        private class PluginMetadata(IPlugin plugin)
        {
            public IPlugin Plugin { get; } = plugin;
            public IMenuDescriptor MenuDescriptor { get; } = plugin?.GetDescriptor()!;

            public bool HasDescriptor(IMenuDescriptor descriptor)
            {
                List<IMenuDescriptor> processed = [];
                Stack<IMenuDescriptor> pending = [];

                pending.Push(descriptor);

                while (pending.Count > 0)
                {
                    var current = pending.Pop();
                    if (current == descriptor)
                    {
                        return true;
                    }
                    if (processed.Contains(current))
                    {
                        continue;
                    }
                    processed.Add(current);
                    if (current.Items != null && current.Items.Count > 0)
                    {
                        foreach (var item in current.Items)
                        {
                            pending.Push(item);
                        }
                    }
                }
                return false;
            }

            public void Validate()
            {
                if (Plugin == null)
                {
                    throw new Exception("Plugin cannot be null");
                }
                if (MenuDescriptor == null)
                {
                    throw new Exception($"Menu descriptor of {Plugin.GetType().FullName} is null");
                }
            }
        }
    }
}
