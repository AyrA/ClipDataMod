# User Interface

The primary way a plugin interacts with the user is via the application context menu. However, it is sometimes necessary to show dialog boxes or interact in other ways.

To facilitate this need, and to keep plugins platform agnostic, UI functionality is provided via `PluginHelper.Dialog`

## Behavior

This chapter serves as a guide on how to interact with the user. Plugins should be careful when using UI functions. Too many dialog boxes will disrupt the workflow of the user, too little will leave them confused as to whether an action took place or not.

In general, don't show success messages for user initiated menu options that complete immediately, but do show them when an automatic action changes the clipboard content.

For warning and error messages it is the other way round, show them on manually initiated action, but do not show them if automated actions fail.

## Message Boxes

Message boxes can be shown with an "Info","Warn","Error" icon.
The buttons can be chosen from a list, and the clicked button is returned in the call, allowing for simple interaction with the user.

Message boxes should be used for important messages that you want the user to confirm. An example would be if the user tries to perform a clipboard action on a string that is in an invalid format.
If the mentioned action was performed automatically in response to a clipboard update, a passive notification should be used instead, or preferrably, no message should be shown at all unless it's blatantly obvious that the data is supposed to be in the expected format, but is not.

## Notifications

Notifications work in a similar manner to message boxes, but they disappear again after a few seconds or when the user clicks on them anywhere. They're useful for messages you want to show to the user, but don't need confirmation. ClipDataMod provides no mechanism to interact with them or detect when a user clicked on the message.

Expect the popup to be only shown briefly. Keep the message short.

## Saving Files

Save dialog windows can be opened using one of the `SaveFile` functions.
They return the fully qualified file name on success, and null on failure or cancellation.

**Note** The dialog box will show a warning if the user tries to use an existing file.

### Filter Mask

The files shown in the dialog can be filtered using one or many `FilterMask` instances. `FilterMask.AllFiles` is provided to add the "All files" filter.
It's common behavior to list explicit types first, and provide the "all files" filter as a fallback. If multiple masks are supplied, the first one is preselected.
When showing a dialog, the following masks should be supplied for maximum user comfort:

1. Mask entry containing "All supported types" with the list of all file extensions the user may want to use
2. One or many mask entries containing individual types to facilitate filter functions.
3. Fallback "All files" type

1 or 2 may be skipped if only one type is available. Both may be skipped if the extension is not known or cannot be reliably guessed.

## Opening Files

Similar to `SaveFile`, a dialog to open files can also be shown. `OpenFile` functions are for one file, `OpenFiles` for multiple files.

For one file, they return the fully qualified file name on success, and null on failure or cancellation. For multiple files, an array with all selected files is returned. The array is empty on cancellation or error.

The same mask rules as for saving apply.

**CAUTION!** Do not rely on the dialog boxes checking if the files actually exist. Not only may this not be done on all operating systems, but the file can be deleted by another process between the dialog box checking if the file exists and your plugin opening the file.

## Browsing directories

`SelectDirectory` is provided to make the user select a directory.
Returns the fully qualified file name on success, and null on failure or cancellation.