namespace Plugin.Clipboard
{
    /// <summary>
    /// A non functional placeholder used as default
    /// until the real clipboard is loaded
    /// </summary>
    /// <remarks>
    /// All accesses to this instance will
    /// unconditionally throw <see cref="NotImplementedException"/>
    /// </remarks>
    internal class ClipboardPlaceholder : IClipboard
    {
        private readonly NotImplementedException NI = new("This is a placeholder implementation");

        public bool MustSync => throw NI;
        public void Clear() => throw NI;
        public bool RegisterFormat<T>(string _) => throw NI;
        public T GetCustomData<T>(string _) => throw NI;
        public byte[] GetBinary() => throw NI;
        public string[] GetFileList() => throw NI;
        public PreferredFileOperation GetFileOperation() => throw NI;
        public IImageWrapper? GetImage() => throw NI;
        public string GetString() => throw NI;
        public string[] GetFormats() => throw NI;
        public byte[] GetAudio() => throw NI;
        public bool HasBinary() => throw NI;
        public bool HasBinaryCompatible() => throw NI;
        public bool HasFileList() => throw NI;
        public bool HasImage() => throw NI;
        public bool HasString() => throw NI;
        public bool HasAudio() => throw NI;
        public bool HasFormat(string _) => throw NI;
        public void SetCustomData(string _1, object _2) => throw NI;
        public void SetBinary(byte[] _) => throw NI;
        public void SetFileList(string[] _1, PreferredFileOperation _2) => throw NI;
        public void SetImage(IImageWrapper _) => throw NI;
        public void SetString(string _) => throw NI;
    }
}
