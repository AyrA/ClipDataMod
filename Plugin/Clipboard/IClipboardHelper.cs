namespace Plugin.Clipboard
{
    /// <summary>
    /// Represents a system independent interface for a clipboard
    /// </summary>
    /// <remarks>
    /// The main application process should implement this operating system specific interface
    /// </remarks>
    public interface IClipboard : ISyncInfo
    {
        /// <summary>
        /// Clears all data from the clipboard
        /// </summary>
        void Clear();
        /// <summary>
        /// Registers a custom format in the clipboard
        /// </summary>
        /// <typeparam name="T">Type to register with the given format</typeparam>
        /// <param name="formatName">Format</param>
        /// <returns>true, if the format was registered, false otherwise</returns>
        /// <remarks>
        /// This function will return true if registration is not necessary on the current system.
        /// Multiple calls using the same name are permitted.
        /// The format is automatically deregistered on application shutdown.
        /// </remarks>
        bool RegisterFormat<T>(string formatName);
        /// <summary>
        /// Gets binary data from the clipboard
        /// </summary>
        /// <remarks>
        /// If no binary data is in the clipboard,
        /// it returns another format if possible in this order:<br />
        /// Image<br />
        /// Audio<br />
        /// String<br />
        /// <br />
        /// Use <see cref="HasBinary"/> to check if the data is actual binary,
        /// or was converted from a proxy format
        /// </remarks>
        /// <returns>Binary data, or null if no data in the clipboard</returns>
        byte[]? GetBinary();
        /// <summary>
        /// Gets audio samples from the clipboard
        /// </summary>
        /// <returns>Audio data, or null if no audio is in the clipboard</returns>
        /// <remarks>
        /// The audio format of this remains unspecified.
        /// On Windows, "audio" means a RIFF wave file.
        /// A plugin that works with audio should not assume a specific format,
        /// and should test whether the audio data is compatible or not.
        /// </remarks>
        byte[] GetAudio();
        /// <summary>
        /// Gets a list of copied files from the clipboard
        /// </summary>
        /// <returns>File list, null if no files</returns>
        string[]? GetFileList();
        /// <summary>
        /// Gets the preferred file operation from the clipboard
        /// </summary>
        /// <remarks>
        /// The value of this function is meaningless
        /// if <see cref="GetFileList"/> is empty.
        /// </remarks>
        /// <returns>
        /// Preferred operation,
        /// or <see cref="PreferredFileOperation.Undefined"/> if none found
        /// </returns>
        PreferredFileOperation GetFileOperation();
        /// <summary>
        /// Gets an image from the clipboard
        /// </summary>
        /// <returns>Image, or null if no image is in the clipboard</returns>
        IImageWrapper? GetImage();
        /// <summary>
        /// Gets a string from the clipboard
        /// </summary>
        /// <returns>String, or null if no string found</returns>
        /// <remarks>
        /// The string will be encoded using default encoding
        /// </remarks>
        string? GetString();
        /// <summary>
        /// Returns a list of formats currently stored in the clipboard,
        /// including formats the implementing instance can convert to.
        /// </summary>
        /// <returns>List of clipboard formats, or an empty list if clipboard is empty</returns>
        /// <remarks>
        /// The format strings are operating system specific,
        /// but common formats are guaranteed to additionally be represented using .NET types
        /// with full type name, for example "System.String"
        /// </remarks>
        string[] GetFormats();
        /// <summary>
        /// Gets custom data with the given format
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="formatName">Data format name</param>
        /// <returns>Data format, or null if not found</returns>
        /// <remarks>
        /// ClipDataMod makes no assumptions about types and formats.
        /// Plugins that use this method likely need to implement an operating system specific handler.
        /// Callee can use <see cref="object"/> as the type of <typeparamref name="T"/>
        /// to handle the type by itself. ClipDataMod makes no guarantee that this will succeed.
        /// </remarks>
        T? GetCustomData<T>(string formatName);
        /// <summary>
        /// Set binary data
        /// </summary>
        /// <param name="data">Binary data</param>
        void SetBinary(byte[] data);
        /// <summary>
        /// Set file list
        /// </summary>
        /// <param name="f">File list</param>
        /// <param name="preferredOperation">
        /// Preferred file operation for the shell<br />
        /// <br />
        /// <see cref="PreferredFileOperation.Undefined"/> can be used
        /// if the caller has no preference.
        /// This value is mostly used by the shell to determine the correct operation
        /// when pasting files
        /// </param>
        void SetFileList(string[] f, PreferredFileOperation preferredOperation);
        /// <summary>
        /// Set an image
        /// </summary>
        /// <param name="img">Image</param>
        void SetImage(IImageWrapper img);
        /// <summary>
        /// Set a string
        /// </summary>
        /// <param name="s">String</param>
        /// <remarks>
        /// The string will be encoded using default encoding
        /// </remarks>
        void SetString(string s);
        /// <summary>
        /// Sets custom data into the clipboard
        /// </summary>
        /// <param name="formatName">Format name</param>
        /// <param name="data">Data to store</param>
        /// <remarks>
        /// ClipDataMod makes no attempt to convert <paramref name="data"/>
        /// into a clipboard capable format.
        /// Custom formats have to be registered first using <see cref="RegisterFormat{T}(string)"/>
        /// </remarks>
        void SetCustomData(string formatName, object data);
        /// <summary>
        /// Gets whether the clipboard contains a string or not
        /// </summary>
        /// <returns>true, if <see cref="GetString"/> will return data</returns>
        bool HasString();
        /// <summary>
        /// Gets whether the clipboard contains an image or not
        /// </summary>
        /// <returns>true, if <see cref="GetImage"/> will return data</returns>
        bool HasImage();
        /// <summary>
        /// Gets whether the clipboard contains data
        /// in the <see cref="PluginHelper.BinaryFormatName"/> format
        /// </summary>
        /// <returns>true, if <see cref="GetBinary"/> will return raw binary data</returns>
        bool HasBinary();
        /// <summary>
        /// Gets whether the clipboard contains data
        /// that <see cref="GetBinary"/> can convert to binary
        /// as a fallback to no binary data being present
        /// </summary>
        /// <returns>true, if binary compatible data present</returns>
        bool HasBinaryCompatible();
        /// <summary>
        /// Gets whether a file list is present or not
        /// </summary>
        /// <returns>true, if <see cref="GetFileList"/> will return data</returns>
        bool HasFileList();
        /// <summary>
        /// Gets whether audio samples are contained in the clipboard
        /// </summary>
        /// <returns>true, if <see cref="GetAudio"/> will return data</returns>
        bool HasAudio();
        /// <summary>
        /// Gets whether the given format is contained in the clipboard
        /// </summary>
        /// <param name="formatName">Format name to check for</param>
        /// <returns>
        /// true, if <see cref="GetCustomData{T}(string)"/> will return data using the given format
        /// </returns>
        bool HasFormat(string formatName);
    }
}
