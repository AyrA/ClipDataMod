using Plugin.MenuItems;

namespace Plugin
{
    /// <summary>
    /// The base interface for all plugins
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// User friendly name of the plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Creator name of the plugin
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Version number of the plugin
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// URL of the plugin
        /// </summary>
        Uri? Url { get; }

        /// <summary>
        /// Event that a plugin can invoke to send a message to other plugins
        /// </summary>
        /// <remarks>
        /// See documentation of <see cref="MessageEventHandler"/> for parameter details
        /// </remarks>
        event MessageEventHandler Message;

        /// <summary>
        /// Method that is called on every plugin in response
        /// to a <see cref="Message"/> event firing
        /// </summary>
        /// <param name="sender">
        /// Plugin that sent the message.
        /// This will be null for internally generated events
        /// </param>
        /// <param name="eventArgs">Event arguments</param>
        /// <remarks>
        /// ClipDataMod makes no guarantee that the method will be called on the main thread.
        /// If the plugin throws an exception, it will be excluded from the messaging system
        /// and can no longer send or receive messages until the application is restarted.
        /// </remarks>
        void OnMessage(IPlugin? sender, PluginEventArgs eventArgs);

        /// <summary>
        /// Gets the menu descriptor for the current plugin
        /// </summary>
        /// <returns>Menu descriptor</returns>
        /// <remarks>
        /// ClipDataMod may call this function multiple times.
        /// The function should return the same descriptor instance on every call
        /// </remarks>
        IMenuDescriptor GetDescriptor();

        /// <summary>
        /// Called by the application every time the clipboard updates
        /// </summary>
        /// <remarks>
        /// This function can be used for automatic actions,
        /// but caution should be taken because this can lead to endless loops
        /// if the callee is not careful.<br />
        /// The call will be performed synchronously,
        /// and because of that should be quick to complete or defer to a background thread.<br />
        /// Menu descriptor properties can be modified in this function,
        /// because after this function completes, all menu items will be rendered again.<br />
        /// If the plugin throws an exception, it will be excluded from further updates
        /// until the application restarts
        /// </remarks>
        void OnClipboardUpdate();
    }
}