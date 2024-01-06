using Plugin.MenuItems;

namespace Plugin
{
    /// <summary>
    /// Provides a base plugin implementation
    /// to reduce the amount of boilerplate code needed to get started
    /// </summary>
    public abstract class BasePlugin : PluginSettings
    {
        /// <summary>
        /// Menu descriptor that is returned by <see cref="GetDescriptor"/>
        /// </summary>
        protected readonly IMenuDescriptor _menuDescriptor;

        /// <inheritdoc />
        public override event MessageEventHandler Message = delegate { };

        /// <summary>
        /// Initializes <see cref="_menuDescriptor"/>
        /// </summary>
        /// <param name="menuDescriptor">
        /// Value assigned to <see cref="_menuDescriptor"/>.
        /// Must not be null
        /// </param>
        protected BasePlugin(IMenuDescriptor menuDescriptor)
        {
            ArgumentNullException.ThrowIfNull(menuDescriptor);
            _menuDescriptor = menuDescriptor;
        }

        /// <inheritdoc />
        public override IMenuDescriptor GetDescriptor()
        {
            return _menuDescriptor ?? throw new InvalidOperationException("Plugin has not been set up correctly. menu descriptor is not set");
        }

        /// <inheritdoc />
        public override void OnClipboardUpdate()
        {
            //NOOP
        }

        /// <inheritdoc />
        public override void OnMessage(IPlugin? sender, PluginEventArgs eventArgs)
        {
            //NOOP
        }
    }
}