namespace Plugin
{
    /// <summary>
    /// Provides information about a plugin
    /// </summary>
    /// <param name="fileName">Plugin file name</param>
    /// <param name="excludes">Full type names of plugins that should not be loaded</param>
    public class PluginInfo(string? fileName, string[]? excludes)
    {
        private readonly string _fileName = fileName
            ?? throw new ArgumentNullException("plugin file name is not specified");
        /// <summary>
        /// Plugin file name
        /// </summary>
        public string FileName { get; private set; } = fileName;
        /// <summary>
        /// Full type names of plugins that should not be loaded
        /// </summary>
        public string[]? Excludes { get; private set; } = excludes;

        /// <summary>
        /// Convert supplied file name in info json into full path
        /// </summary>
        /// <param name="dir"></param>
        public void ToFullPath(string dir)
        {
            FileName = Path.Combine(dir, _fileName);
        }
    }
}
