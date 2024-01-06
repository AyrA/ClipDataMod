namespace Plugin.Clipboard
{
    /// <summary>
    /// Flags for preferred file operations in a file list
    /// </summary>
    [Flags]
    public enum PreferredFileOperation
    {
        /// <summary>
        /// Operation is undefined
        /// </summary>
        /// <remarks>
        /// This value should be treated defensively.
        /// Do not move or delete files with this state, only copy or link them
        /// </remarks>
        Undefined = 0,
        /// <summary>
        /// Files should be copied, the source files left unmodified
        /// </summary>
        /// <remarks>
        /// This is the recommended value to use
        /// if the plugin doesn't knows the user preferences,
        /// or if the user may be interested in pasting the same file multiple times
        /// </remarks>
        Copy = 1,
        /// <summary>
        /// Files should be moved, the source files deleted
        /// </summary>
        /// <remarks>
        /// This is the recommended action for temporary files created by plugins
        /// that are intended for the user to paste into a file explorer window.
        /// Plugins should use <see cref="TempFiles"/> for ephemeral files
        /// </remarks>
        Move = 2,
        /// <summary>
        /// Files should be linked, the source files left unmodified
        /// </summary>
        /// <remarks>
        /// In Windows, a link is a shortcut file, not a symbolic link.
        /// Symbolic links require administrative permissions to be created by default.
        /// Because of this, this code should be ignored by most plugins
        /// </remarks>
        Link = 4,
        /// <summary>
        /// The mask applied to the enumeration to filter possible future values.
        /// </summary>
        /// <remarks>
        /// Copy and Move are usually not found together
        /// </remarks>
        Any = Copy | Move | Link,
    }
}
