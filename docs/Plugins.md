# Plugins

ClipDataMod provides a few internal actions, but primarily is based on plugins that are loaded at runtime.

An assembly can contain multiple plugins.

## Creating a Plugin

Creating plugins is easy and fast.
Simply create a C# library project that targets .NET 8, then add a reference to the `Plguin.dll` file.

Create a class and implement the plugin interface by adding `: IPlugin` after the class name for a full plugin, or add `: BasePlugin` for a minimal plugin.

A minimal plugin will look like this:

```csharp
internal class Test : BasePlugin
{
	public override string Name => "Test plugin";

	public override string Author => "YourNameHere";

	public override Version Version =>
		Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

	public override Uri? Url => new("https://yoursite.example/");

	public Test() : base(new BaseMenuDescriptor("Test", null, OnClick))
	{
		//Use _menuDescriptor.Items.Add(...) to add subitems
	}

	private static void OnClick(IMenuDescriptor menuDescriptor)
	{
		try
		{
			//Do something with the clipboard here
			var str = PluginHelper.Clipboard.GetString();
			if(!string.IsNullOrEmpty(str))
			{
				PluginHelper.Dialog.Info(str, "Clipboard contents");
			}
		}
		catch (Exception ex)
		{
			PluginHelper.Dialog.Error(ex.Message, "Failed to get clipboard contents");
		}
	}
}
```

## Implementing BasePlugin

`BasePlugin` is an abstract class you can use as base for simple plugins.
It implements less used functions of `IPlugin` in a minimalistic manner:

- `Message` event is declared with an empty event handler
- `OnMessage` method is declated and empty
- `GetDescriptor` method is declared and returns the descriptor used in the base constructor
- `OnClipboardUpdate` method is declated and empty

Additionally, the readonly `_menuDescriptor` is field will be populated with the descriptor used in the base constructor. You can use this field to add subitems in your constructor

Methods are declared using the `virtual` keyword, which means you can declare the same method using the `override` keyword to add custom logic for that function.

The properties `Name`, `Author`, `Version`, `Url` are not implemented, and you must declare them using `override` yourself.

## Implementing IPlugin

For more complex plugins, you may prefer to directly implement `IPlugin` directly instead of `BasePlugin`. A few details are given below, but in general, all properties and methods contain detailed documentation comments.

### GetDescriptor()

This function returns the menu descriptor. It should return the same descriptor reference on every call. For this reason, it's recommended to build the descriptor in the constructor and cache it in a local field.

In most cases, the `BaseMenuDescriptor` from the plugin library is sufficient. You can however implement your own `IMenuDescriptor` if you need advanced features.

You can also implement the descriptor and plugin in the same class
by making the plugin also inherit from `IMenuDescriptor`. This way you can easily update the properties of the descriptor.
The code of `GetDescriptor()` is then just `return this;`

&rArr; See the menu descriptor help for more information about this.

### OnClipboardUpdate()

This function is called every time the clipboard updates. It should complete fast to not block clipboard updates for too long. Only one OnClipboardUpdate call is running across all loaded plugins, but the order of the plugins is not guaranteed to remain the same between calls.

For simple plugins, this function is usually empty, but more advanced plugins can among other things:

- Perform automatic actions based on the clipboard contents
- Update the menu descriptor to change title, icon and enabled state of its menu items.

Descriptor updates must be performed synchronously because the menu is immediately updated when this event completes.

**CAUTION!** Updating the clipboard while this handler is running may lead to it being triggered immediately again after completion. This can potentially result in an infinite loop if the action is not handled carefully. Replacing the clipboard contents with identical contents will trigger the update handler again.
Replacing the contents of the clipboard may trigger the handler twice because the clipboard is cleared first before new content is set.

### Message event

This event can be invoked by the plugin to send messages to other plugins.
Invoking this event causes ClipDataMod to invoke the `OnMessage` method of every loaded plugin, including the plugin that generated the event.

**Note!** Messages cannot contain the string "ClipDataMod" (case insensitive) anywhere in the type. This is reserved for internal messages.
It's recommended that the message type is prefixed with a unique string such as a domain name that the creator controls to avoid conflicts with message types from other plugins.
The type must consist of only ASCII characters in the range `0x21` to `0x7E` inclusive.

### OnMessage(IPlugin?, PluginEventArgs)

This method is invoked by ClipDataMod in response to a plugin raising the `Message` event. In the future it may also be called for messages created by ClipDataMod itself. In that case, the IPlugin parameter will be null.
The plugin should carefully evaluate the supplied arguments, because the custom object that can be passed can be altered by any plugin in the chain, and doesn't necessary has to match the expected object type for a given plugin. Always perform a null check and type check before trying to use the custom message payload.

## Sync vs Async

`PluginHelper.Clipboard` and `PluginHelper.Dialog` both contain a `MustSync` property. This property indicates whether or not you must synchronize with the main thread when accessing methods and properties of either type from a different thread.

You can run a method in sync using the `PluginHelper.Sync(action);` method. The calling thread will be blocked until the supplied action completes.
It's safe to call this function when your method already runs on the main thread. It's also safe to call this function if synchronization is not necessary in your current environment. In both cases, the supplied action will be called immediately on the current thread.

**Note!** ClipDataMod already calls `OnClipboardUpdate` on the main thread, and you don't have to sync manually from inside that method.

**CAUTION!** `OnMessage` will be called on whatever thread was active when a plugin raised the `Message` event.

You should avoid performing potentially long running actions in `OnClipboardUpdate`, because it blocks all other plugins as well as further clipboard updates. Run long tasks in a background thread instead.
Periodic actions that don't depend on a clipboard update should be run on a timer instead of the clipboard update function. Do not forget to synchronize back to the main thread if you plan on updating the clipboard or show a dialog.

## Automated Actions

`OnClipboardUpdate` permits for automatic clipboard based actions. This can be a powerful tool, but also a massive annoyance for the user.

All automated actions should be toggleable by the user.
Menu descriptors with their `Checked` poperty can be used for this purpose.
"Destructive" actions should not be enabled by default unless the state is restored from the settings.

## Settings

To get access to plugin settings, a plugin can inherit from the `PluginSettings` base class. This is only necessary when implementing `IPlugin` directly, `BasePlugin` already does this. This gets the plugin access to the `SetValue` and `GetValue` functions as well as the `ClearSettings` function.
Any data that is JSON serializable can be saved and loaded. Settings are kept on a per-plugin base in the same directory the plugin is in.

**Note** Settings are immediately written to disk on every call to `SetValue`. Do not call this function if the data did not change, because it leads to unnecessary disk activity.