namespace Plugin
{
    /// <summary>
    /// Event arguments for plugin messaging event
    /// </summary>
    public class PluginEventArgs
    {
        /// <summary>
        /// Gets or sets whether the event was handled
        /// </summary>
        /// <remarks>
        /// This propery has no effect and is merely used
        /// to communicate to plugins that a plugin already handled the event.
        /// It is left to the plugins how to interpret this
        /// </remarks>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets whether to cancel the event or not
        /// </summary>
        /// <remarks>
        /// If set to true by a plugin, the event chain will be immediately cancelled.
        /// Setting this to true asynchronously will not cancel the event chain.
        /// </remarks>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the message type as sent by the <see cref="IPlugin.Message"/> event
        /// </summary>
        /// <remarks>
        /// For restrictions,
        /// see parameter documentation of <see cref="MessageEventHandler"/>
        /// </remarks>
        public string MessageType { get; }

        /// <summary>
        /// Gets the message payload as sent by the <see cref="IPlugin.Message"/> event
        /// </summary>
        /// <remarks>
        /// ClipDataMod makes no attempt in protecting this value.
        /// Provided the supplied object has settable properties, plugins can modify them.
        /// The object is not automatically disposed at the end of the event,
        /// even if it implements the IDisposable interface.
        /// Cleanup is the responsibility of the plugin that generated the event.
        /// Events are processed synchronously, which means it's safe
        /// for the generating plugin to call the event from within a using block.
        /// </remarks>
        public object? MessageData { get; }

        internal PluginEventArgs(string messageType, object? messageData)
        {
            MessageType = messageType;
            MessageData = messageData;
        }
    }
}