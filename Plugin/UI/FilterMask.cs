namespace Plugin.UI
{
    /// <summary>
    /// File filter mask for the <see cref="IDialog" /> functions
    /// </summary>
    public class FilterMask
    {
        /// <summary>
        /// Provides easy access to a mask that specifies all files
        /// </summary>
        /// <remarks>
        /// The default mask is "All files (*.*)" with a mask of "*.*"
        /// but can be internally overridden to accomodate for other languages and operating systems.
        /// "*.*" has a special meaning on Windows of "All files" instead of "All files with a dot"
        /// </remarks>
        public static FilterMask[] AllFiles { get; internal set; } = [new("All Files (*.*)", "*.*")];

        /// <summary>
        /// Gets the mask title
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// Gets the masks
        /// </summary>
        public string[] Mask { get; }

        /// <summary>
        /// Creates a new instance using the given name and mask
        /// </summary>
        /// <param name="title">Mask title</param>
        /// <param name="mask">File name masks</param>
        public FilterMask(string title, params string[] mask)
        {
            ArgumentException.ThrowIfNullOrEmpty(title);
            ArgumentNullException.ThrowIfNull(mask);
            if (mask.Length == 0)
            {
                throw new ArgumentException("Masks cannot be empty", nameof(mask));
            }
            Title = title;
            Mask = mask;
        }
    }
}
