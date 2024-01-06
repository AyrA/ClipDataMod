namespace Plugin.UI
{
    /// <summary>
    /// Possible enumeration of dialog box button combinations
    /// </summary>
    /// <remarks>
    /// Button specific documentation can be found
    /// at the respective enum member of <see cref="ButtonResult"/>
    /// </remarks>
    public enum DialogButtons
    {
        /// <summary>
        /// Use "OK" button only
        /// </summary>
        OK = 0,
        /// <summary>
        /// "OK" and "Cancel" button
        /// </summary>
        /// <remarks>
        /// Consider using <see cref="YesNo"/> instead
        /// if you ask the user a Yes/No question
        /// </remarks>
        OKCancel = 1,
        /// <summary>
        /// "Abort" "Retry" "Igonre" buttons
        /// </summary>
        AbortRetryIgnore = 2,
        /// <summary>
        /// "Yes" "No" "Cancel" buttons
        /// </summary>
        YesNoCancel = 3,
        /// <summary>
        /// "Yes" "No" buttons
        /// </summary>
        YesNo = 4,
        /// <summary>
        /// "Retry" "Cancel" buttons
        /// </summary>
        RetryCancel = 5
    }
}
