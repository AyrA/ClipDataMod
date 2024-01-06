# Menu Descriptor

The menu descriptor is the only way you have to expose your plugin to the user.

A menu descriptor must implement `IMenuDescriptor`. An implementation is provided in `BaseMenuDescriptor`. This implementation has no custom functionality, and is sufficient for most plugins.

Menu descriptors may be updated at any time by the plugin, but those updates will not have any effect until the clipboard content changes. Because of this, it's usually a good idea to update the descriptor only when the clipboard update function of your plugin is called, unless said update operation may require a lot of time.

For very small plugins, you may wish to implement `IMenuDescriptor` and `IPlugin` in the same file.

## Writable Descriptor

`IMenuDescriptor` only declares the "get" operation for descriptor plugins.
To make these properties writable, you can create a custom interface that inherits from `IMenuDescriptor` and redeclares the necessary properties as writable.

As an alternative, you can create backing fields in your descriptor that are marked as either `internal` (separate descriptor) or `private` (descriptor and plugin combo in single class) to avoid creating an interface.

## Submenus

A descriptor can contain more descriptors in the "Items" property. This is to facilitate a menu structure.
Please be aware that navigation gets increasingly more difficult with every extra menu layer.

## The "Image" Property

The `Image` property of the descriptor is a byte array and not an actual image. Reasoning behind this is that .NET lacks a system independent image type.

In general, it should be safe to use a 16x16 PNG as data.

## "OnClick" vs. "Items"

A menu descriptor can have an "OnClick" action assigned to it. This action is called when the user clicks on the menu item. A descriptor can also have subitems in the "Items" property.

"OnClick" and "Items" are mutually exclusive on Windows. Click handlers are ignored if at least one submenu item is present.

Because of this behavior, you should not register important click handler on menu items that contain submenu items.