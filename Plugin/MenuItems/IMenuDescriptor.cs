namespace Plugin.MenuItems
{
    /// <summary>
    /// Interface from which all menu descriptors inherit
    /// </summary>
    public interface IMenuDescriptor
    {
        /// <summary>
        /// Unique menu id
        /// </summary>
        /// <remarks>
        /// This id can be used by plugins to store menu related information.
        /// The id is only unique during the application lifetime
        /// and will differ on every application start.
        /// </remarks>
        Guid Id { get; }

        /// <summary>
        /// Gets the display string used for the menu item
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets if the menu item should be rendered as enabled or disabled
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Gets the item to be shown in front of <see cref="Title"/>
        /// </summary>
        byte[]? Icon { get; }

        /// <summary>
        /// Gets the function to be called
        /// </summary>
        /// <remarks>
        /// Clicking on the menu item will have no effect if this is not set.
        /// This is normally the case for main menu items if they contain child menu items.
        /// Windows ignores this if the menu has child items
        /// </remarks>
        MenuHandlerDelegate? OnClick { get; }

        /// <summary>
        /// Gets the subitems of this instance
        /// </summary>
        /// <remarks>
        /// Any level of subitems is supported,
        /// although it's a good idea to have at most one level
        /// because menu navigation gets increasingly more difficult with every additional level.
        /// If this has any items <see cref="OnClick"/> will have no effect anymore
        /// </remarks>
        List<IMenuDescriptor> Items { get; }

        /// <summary>
        /// Gets whether the menu item should be rendered in checked or unchecked state.
        /// </summary>
        /// <remarks>
        /// The default is the unchecked state.
        /// The check state can be used to facilitate simple settings
        /// </remarks>
        bool Checked { get; set; }
    }
}