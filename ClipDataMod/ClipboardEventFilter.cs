using Plugin;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClipDataMod
{
    internal partial class ClipboardEventFilter : IMessageFilter, IDisposable
    {
        /// <summary>
        /// Message id for a clipboard change event
        /// </summary>
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        [LibraryImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool AddClipboardFormatListener(IntPtr hWnd);

        [LibraryImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool RemoveClipboardFormatListener(IntPtr hWnd);

        [LibraryImport("User32.dll")]
        private static partial uint RegisterClipboardFormatW([MarshalAs(UnmanagedType.LPWStr)] string name);

        private readonly Form clipForm;
        private readonly IntPtr hWnd;

        public readonly uint CustomClipboardFormat;

        /// <summary>
        /// Event that is triggered when the clipboard changes
        /// </summary>
        /// <remarks>
        /// All plugins have been updated at that point already
        /// </remarks>
        public event Action<object> ClipboardChanged = delegate { };

        public ClipboardEventFilter()
        {
            clipForm = new Form();
            hWnd = clipForm.Handle;
            CustomClipboardFormat = RegisterClipboardFormatW(PluginHelper.BinaryFormatName);
        }

        public void RunOnUiThread(Action action)
        {
            if (clipForm.InvokeRequired)
            {
                clipForm.Invoke(action);
            }
            else
            {
                action();
            }
        }

        public void Register()
        {
            AddClipboardFormatListener(hWnd);
        }

        public void Deregister()
        {
            RemoveClipboardFormatListener(hWnd);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                PluginHelper.OnClipboardUpdate();
                try
                {
                    ClipboardChanged(this);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Deregister();
            clipForm.Dispose();
        }
    }
}
