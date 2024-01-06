# Linux and other OS

ClipDataMod is only available for Windows as of now. I have no intention on creating binaries for other systems. However, ClipDataMod has been carefully designed to be trivially extendable to work with other operating systems.

This document is intended to serve as guideline on how to implement ClipDataMod for a different operating system.

## Short Version

1. Download the repository contents and open the solution
2. Create a new project that targets your current operating system
3. Add a reference to the "Plugin" project in your project
4. Implement `IClipboard` and `IDialog` according to the intellisense comments for each method and property.
5. Use `PluginHelper.InitHelper` to initialize the system
6. Use `PluginHelper.LoadPlugins` to load all plugins from a given assembly or file

**Note!** ClipDataMod is a Windows project and may not build if you develop for another platform. You can right click on the project file and select "Unload Project" to disable it.

## Implementing IClipboard

The `IClipboard` provides the interface which plugins use to access clipboard functionality. The interface should be implemented in a way that it most closely matches the method descriptions provided in the interface.

### Binary data

The primary method to exchange data should be binary. This simplifies plugin development. The builtin "Security" plugin for example hashes any data that can be converted to binary, and stores the result as binary. The user can then use another plugin to convert the binary data into the appropriate text format (hexadecimal, base64, etc).

Unless your system has a native binary type, you can use `PluginHelper.BinaryFormatName` if the clipboard needs a type name.

## Clipboard monitoring

Clipboard monitoring should be implemented within your project in accordance with the recommendations for your operating system. When a clipboard update is detected, you should call `PluginHelper.OnClipboardUpdate()`, which will forward the event to all loaded plugins. If your clipboard requires main thread synchronization, you should call the event on the main thread.

## Implementing IDialog

The `IDialog` provides the interface which plugins use to create dialog boxes without having to be aware of the system architecture. The interface should be implemented in a way that it most closely matches the method descriptions provided in the interface.

Special care and attention has to be given to the `IDialog.Exception` method. This method should always work, even when the current action is not running on the main thread in an environment where main thread synchronization is required.
