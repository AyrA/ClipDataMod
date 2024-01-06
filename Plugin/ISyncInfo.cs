namespace Plugin
{
    /// <summary>
    /// Interface declaring whether an application must sync or not
    /// </summary>
    public interface ISyncInfo
    {
        /// <summary>
        /// Gets whether <see cref="PluginHelper.Sync(Action)"/> must be called
        /// to safely access any functionality of this instance
        /// </summary>
        /// <remarks>
        /// Application generated events such as clipboard change events are always called in sync.
        /// Plugins are loaded in sync too.<br />
        /// If this property is "true",
        /// the behavior of methods in this instance may be undefined,
        /// potentially not performing any action or throwing exceptions
        /// </remarks>
        bool MustSync { get; }
    }
}
