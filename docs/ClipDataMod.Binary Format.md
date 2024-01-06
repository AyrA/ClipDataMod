# ClipDataMod.Binary Format

ClipDataMod comes with its own binary clipboard format as a universal way of performing clipboard actions.

## Purpose

The binary format is provided because the Windows clipboard lacks a universal binary format. Audio and image formats could both be abused for this purpose, but this may have unintended side effects in audio and video editor software. Because of this, ClipDataMod comes with its own binary format.

**Note!** `IClipboard` implementations on other systems may prefer to use a system native binary format if its available. Your Plugin should not be affected by this decision if it doesn't interacts with the clipboard directly, but only via `PluginHelper.Clipboard`

The binary format can be used to simplify actions that are agnostic to the type of data in the clipboard. The builtin "Security" plugin for example uses binary data as source for the hash functions.

## Behavior for Plugins

Plugins that create binary data (for example when creating a checksum) should refrain from encoding this data, unless the data is never used in a raw manner. `ClipDataMod` comes with a hex and base64 encoder that users can use to transform the data into the desired format.

## Behavior for IClipboard Implementations

See the IClipboard.md file for more information

## Detecting Binary

`IClipboard` provides two functions, `HasBinary` and `HasBinaryCompatible`.
The first one detecting only the binary format itself, the latter one detecting if there's any data in the clipboard, that `GetBinary()` will use as fallback if no binary data is available
