namespace Plugin.MenuItems
{
    /// <summary>
    /// Base implementation of <see cref="IMenuDescriptor"/> that describes a menu item
    /// </summary>
    /// <param name="menuTitle">Menu item text</param>
    /// <param name="icon">
    /// Optional icon to be shown in front of the menu item.
    /// </param>
    /// <param name="onClick">
    /// The function to be invoked when the user clicks on a menu item
    /// </param>
    /// <param name="children">
    /// Child items of the current menu.
    /// These are optional, and can later be added and removed from <see cref="Items"/>.
    /// Using null is the same as supplying an empty enumeration
    /// </param>
    /// <remarks>
    /// This implementation is suitable for simple use cases that don't need special features
    /// </remarks>
    public class BaseMenuDescriptor(string menuTitle, byte[]? icon = null, MenuHandlerDelegate? onClick = null, IEnumerable<IMenuDescriptor>? children = null) : IMenuDescriptor
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public string Title { get; } = menuTitle;

        /// <inheritdoc/>
        public byte[]? Icon { get; } = icon;

        /// <inheritdoc/>
        public MenuHandlerDelegate? OnClick { get; } = onClick;

        /// <inheritdoc/>
        public List<IMenuDescriptor> Items { get; } = children?.ToList() ?? [];

        /// <inheritdoc/>
        public bool Enabled { get; set; } = true;

        /// <inheritdoc/>
        public bool Checked { get; set; } = false;
    }
}
