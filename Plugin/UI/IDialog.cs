namespace Plugin.UI
{
    /// <summary>
    /// Represents a common interface to display system native dialog boxes
    /// </summary>
    /// <remarks>
    /// Plugins should use these functions instead of coding up their own dialog boxes
    /// to ensure that shown dialog boxes look the same across all plugins.<br />
    /// Plugins should only render their own dialog boxes
    /// if the functionality provided in this interface
    /// is not suitable for the requirements of the plugin.
    /// </remarks>
    public interface IDialog : ISyncInfo
    {
        #region Dialog boxes

        /// <summary>
        /// Shows a system native dialog box using the given properties and an "info" icon
        /// </summary>
        /// <param name="text">Message text</param>
        /// <param name="title">Dialog box title</param>
        /// <param name="buttons">Shown buttons</param>
        /// <returns>Pressed button</returns>
        ButtonResult Info(string text, string title, DialogButtons buttons = DialogButtons.OK);
        /// <summary>
        /// Shows a system native dialog box using the given properties and a "warning" icon
        /// </summary>
        /// <param name="text">Message text</param>
        /// <param name="title">Dialog box title</param>
        /// <param name="buttons">Shown buttons</param>
        /// <returns>Pressed button</returns>
        ButtonResult Warn(string text, string title, DialogButtons buttons = DialogButtons.OK);
        /// <summary>
        /// Shows a system native dialog box using the given properties and an "error" icon
        /// </summary>
        /// <param name="text">Message text</param>
        /// <param name="title">Dialog box title</param>
        /// <param name="buttons">Shown buttons</param>
        /// <returns>Pressed button</returns>
        ButtonResult Error(string text, string title, DialogButtons buttons = DialogButtons.OK);
        /// <summary>
        /// Shows a system native window with error details
        /// </summary>
        /// <param name="ex">Exception that was ungaught</param>
        /// <param name="description">User friendly description</param>
        /// <param name="source">Plugin that caused the error (if any)</param>
        /// <remarks>
        /// Plugins should generally not call this directly, and throw exceptions instead.
        /// It should be called by whatever calls into the plugin.<br />
        /// This function automatically synchronizes with the main thread if necessary
        /// </remarks>
        void Exception(Exception ex, string description, IPlugin? source);

        #endregion
        #region Notifications

        /// <summary>
        /// Shows a notification with informational content for a few seconds
        /// </summary>
        /// <param name="text">Notification text</param>
        /// <param name="title">Notification title</param>
        /// <remarks>
        /// On systems where this is not supported, a temporary self-closing dialog window will be shown
        /// </remarks>
        void NotifyInfo(string text, string title);
        /// <summary>
        /// Shows a notification with a warning for a few seconds
        /// </summary>
        /// <param name="text">Notification text</param>
        /// <param name="title">Notification title</param>
        /// <remarks>
        /// On systems where this is not supported, a temporary self-closing dialog window will be shown
        /// </remarks>
        void NotifyWarn(string text, string title);
        /// <summary>
        /// Shows a notification with an error message for a few seconds
        /// </summary>
        /// <param name="text">Notification text</param>
        /// <param name="title">Notification title</param>
        /// <remarks>
        /// On systems where this is not supported, a temporary self-closing dialog window will be shown
        /// </remarks>
        void NotifyError(string text, string title);

        #endregion
        #region Save/Open Windows

        /// <summary>
        /// Shows a system native dialog used to pick a file name for saving
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="mask">File name mask</param>
        /// <returns>string of selected file, or null if the dialog was cancelled</returns>
        string? SaveFile(string title, FilterMask mask);
        /// <summary>
        /// Shows a system native dialog used to pick a file name for saving
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="masks">File name masks</param>
        /// <returns>string of selected file, or null if the dialog was cancelled</returns>
        string? SaveFile(string title, IEnumerable<FilterMask> masks);
        /// <summary>
        /// Shows a system native dialog used to pick a file name for saving
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="masks">File name masks</param>
        /// <param name="defaultDir">Default directory the dialog should open up in</param>
        /// <returns>string of selected file, or null if the dialog was cancelled</returns>
        string? SaveFile(string title, IEnumerable<FilterMask> masks, string defaultDir);
        /// <summary>
        /// Shows a system native dialog used to pick a file name for saving
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="masks">File name masks</param>
        /// <param name="defaultDir">
        /// Default directory the dialog should open up in.
        /// If set to null or empty, defaults to the dialog box initial value
        /// </param>
        /// <param name="defaultFileName">
        /// Default file name prefilled into the file name box.<br />
        /// <br />
        /// The path component will be trimmed off if present.
        /// Use <paramref name="defaultDir"/> to specify the directory instead.
        /// </param>
        /// <returns>string of selected file, or null if the dialog was cancelled</returns>
        string? SaveFile(string title, IEnumerable<FilterMask> masks, string? defaultDir, string defaultFileName);

        /// <summary>
        /// Shows a system native dialog used to pick a file name for opening.
        /// The dialog performs a basic check to ensure the file exists
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="mask">File name mask</param>
        /// <returns>string of selected file, or null if the dialog was cancelled</returns>
        string? OpenFile(string title, FilterMask mask);
        /// <summary>
        /// Shows a system native dialog used to pick a file name for opening.
        /// The dialog performs a basic check to ensure the file exists
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="masks">File name mask</param>
        /// <returns>string of selected file, or null if the dialog was cancelled</returns>
        string? OpenFile(string title, IEnumerable<FilterMask> masks);
        /// <summary>
        /// Shows a system native dialog used to pick a file name for opening.
        /// The dialog performs a basic check to ensure the file exists
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="masks">File name mask</param>
        /// <param name="defaultDir">Initial directory to open the dialog in</param>
        /// <returns>string of selected file, or null if the dialog was cancelled</returns>
        string? OpenFile(string title, IEnumerable<FilterMask> masks, string defaultDir);
        /// <summary>
        /// Shows a system native dialog used to pick a file name for opening.
        /// The dialog performs a basic check to ensure the file exists
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="masks">File name mask</param>
        /// <param name="defaultDir">Initial directory to open the dialog in</param>
        /// <param name="defaultFileName">
        /// Default file name prefilled into the file name box.<br />
        /// <br />
        /// The path component will be trimmed off if present.
        /// Use <paramref name="defaultDir"/> to specify the directory instead.
        /// </param>
        /// <returns>string of selected file, or null if the dialog was cancelled</returns>
        string? OpenFile(string title, IEnumerable<FilterMask> masks, string defaultDir, string defaultFileName);

        /// <summary>
        /// Shows a system native dialog used to pick multiple files for opening.
        /// The dialog performs a basic check to ensure the file exists
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="mask">File name mask</param>
        /// <returns>array of selected files, empty array if the dialog was cancelled</returns>
        string[] OpenFiles(string title, FilterMask mask);
        /// <summary>
        /// Shows a system native dialog used to pick multiple files for opening.
        /// The dialog performs a basic check to ensure the file exists
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="masks">File name mask</param>
        /// <returns>array of selected files, empty array if the dialog was cancelled</returns>
        string[] OpenFiles(string title, IEnumerable<FilterMask> masks);
        /// <summary>
        /// Shows a system native dialog used to pick multiple files for opening.
        /// The dialog performs a basic check to ensure the file exists
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="masks">File name mask</param>
        /// <param name="defaultDir">Initial directory to open the dialog in</param>
        /// <returns>array of selected files, empty array if the dialog was cancelled</returns>
        string[] OpenFiles(string title, IEnumerable<FilterMask> masks, string defaultDir);
        /// <summary>
        /// Shows a system native dialog used to pick multiple files for opening.
        /// The dialog performs a basic check to ensure the file exists
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="masks">File name mask</param>
        /// <param name="defaultDir">Initial directory to open the dialog in</param>
        /// <param name="defaultFileName">
        /// Default file name prefilled into the file name box.<br />
        /// <br />
        /// The path component will be trimmed off if present.
        /// Use <paramref name="defaultDir"/> to specify the directory instead.
        /// </param>
        /// <returns>array of selected files, empty array if the dialog was cancelled</returns>
        string[] OpenFiles(string title, IEnumerable<FilterMask> masks, string defaultDir, string defaultFileName);

        /// <summary>
        /// Shows a system native dialog used to pick a directory
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <returns>Selected directory, null if cancelled</returns>
        string? SelectDirectory(string title);
        /// <summary>
        /// Shows a system native dialog used to pick a directory
        /// </summary>
        /// <param name="title">Dialog box title</param>
        /// <param name="selectedDir">Preselected directory</param>
        /// <returns>Selected directory, null if cancelled</returns>
        string? SelectDirectory(string title, string selectedDir);

        #endregion
    }
}
