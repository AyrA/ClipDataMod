namespace Plugin
{
    /// <summary>
    /// Message event declaration
    /// </summary>
    /// <param name="messageType">
    /// Type of the message.
    /// The following restrictions apply:<br />
    /// - The substring "ClipDataMod" (case insensitive) must not be present anywhere<br />
    /// - The value must not be null or an empty string<br />
    /// - The value must consist of ASCII characters between 0x21 and 0x7E (both inclusive)
    /// </param>
    /// <param name="messageData">
    /// Optional message payload. Can be null if no payload is needed
    /// </param>
    public delegate void MessageEventHandler(string messageType, object? messageData);
}
