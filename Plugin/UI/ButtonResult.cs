namespace Plugin.UI
{
    /// <summary>
    /// Represents the clicked button on a system native dialog box
    /// </summary>
    public enum ButtonResult
    {
        /// <summary>
        /// "OK" button was clicked
        /// </summary>
        /// <remarks>
        /// If present, this button is always the default button
        /// and can be activated using ENTER.
        /// It is the button shown by default for dialog boxes that lack options.
        /// </remarks>
        OK = 1,
        /// <summary>
        /// "Cancel" button was clicked
        /// </summary>
        /// <remarks>
        /// If present, this button is always mapped to the ESC key.
        /// Showing this button also enables the [X] button of the window title bar.
        /// This code is also used when the user closes the dialog box with said [X] button.
        /// </remarks>
        Cancel = 2,
        /// <summary>
        /// "Abort" button was clicked
        /// </summary>
        /// <remarks>
        /// This button should abort the current action at the next safe abort point.
        /// The plugin should handle cleanup.
        /// This button should be offered in response to an operation that failed,
        /// and could be aborted in a safe manner.
        /// </remarks>
        Abort = 3,
        /// <summary>
        /// "Retry" button was clicked
        /// </summary>
        /// <remarks>
        /// This button should retry the failed operation.
        /// This button should be shown if retrying the operation might resolve the error,
        /// for example if a file cannot be deleted because it is open in another application.
        /// The user should be able to retry indefinitely.
        /// </remarks>
        Retry = 4,
        /// <summary>
        /// "Ignore" button was clicked
        /// </summary>
        /// <remarks>
        /// This button should skip the current operation.
        /// This button should be shown if it's possible to ignore the current error,
        /// and still have the possibility for a successful outcome at the end of the operation.
        /// Example: A plugin uploads the clipboard contents to multiple online services,
        /// but one of them is unavailable.
        /// </remarks>
        Ignore = 5,
        /// <summary>
        /// "Yes" button was clicked
        /// </summary>
        Yes = 6,
        /// <summary>
        /// "No" button was clicked
        /// </summary>
        No = 7
    }
}
