using Plugin;
using Plugin.Clipboard;
using System.Collections.Specialized;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace ClipDataMod
{
    internal partial class WindowsClipboard : IClipboard
    {
        [LibraryImport("User32.dll")]
        private static partial uint RegisterClipboardFormatW([MarshalAs(UnmanagedType.LPWStr)] string name);

        public bool MustSync => true;

        public void Clear() => Clipboard.Clear();

        public bool RegisterFormat<T>(string formatName)
        {
            return RegisterClipboardFormatW(formatName) != 0;
        }

        public void SetString(string s) => Clipboard.SetText(s);

        public void SetFileList(string[] f, PreferredFileOperation preferredOperation)
        {
            var l = new StringCollection();
            l.AddRange(f);
            Clipboard.SetFileDropList(l);
            //TODO: Use PreferredFileOperation
        }

        public void SetImage(IImageWrapper img)
        {
            if (img is IImageWrapper<Image> imgWrapper)
            {
                Clipboard.SetImage(imgWrapper.Image);
            }
            else
            {
                throw new ArgumentException($"Argument must be {nameof(IImageWrapper<Image>)} type", nameof(img));
            }
        }

        public void SetBinary(byte[] data) => Clipboard.SetData(PluginHelper.BinaryFormatName, data);

        public void SetCustomData(string formatName, object data)
        {
            Clipboard.SetData(formatName, data);
        }

        public string? GetString()
        {
            var str = Clipboard.GetText();
            //Windows uses empty string instead of null for an empty clipboard
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return str;
        }

        public string[]? GetFileList()
        {
            var entries = Clipboard.GetFileDropList()?.OfType<string>().ToArray();
            return entries != null && entries.Length > 0 ? entries : null;
        }

        public PreferredFileOperation GetFileOperation()
        {
            var data = Clipboard.GetDataObject();
            if (data?.GetData("Preferred DropEffect", true) is MemoryStream ms)
            {
                using var br = new BinaryReader(ms);
                var effect = (PreferredFileOperation)br.ReadInt32();
                return effect & PreferredFileOperation.Any;
            }
            return PreferredFileOperation.Undefined;
        }

        public IImageWrapper? GetImage()
        {
            var img = Clipboard.GetImage()
                ?? throw new InvalidOperationException("No image in clipboard");
            return new WindowsImage(img);
        }

        public byte[] GetAudio()
        {
            using var audio = Clipboard.GetAudioStream()
                ?? throw new InvalidOperationException("No audio in clipboard");
            using var ms = new MemoryStream();
            audio.CopyTo(ms);
            return ms.ToArray();
        }

        public byte[]? GetBinary()
        {
            var data = Clipboard.GetData(PluginHelper.BinaryFormatName);
            if (data is byte[] b)
            {
                return b;
            }
            var img = Clipboard.GetImage();
            if (img != null)
            {
                using var ms = new MemoryStream();
                img.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
            var str = Clipboard.GetText();
            if (str != null)
            {
                return Encoding.UTF8.GetBytes(str);
            }
            return null;
        }

        public string[] GetFormats()
        {
            var obj = Clipboard.GetDataObject();
            if (obj == null)
            {
                return [];
            }
            return obj.GetFormats(true);
        }

        public T? GetCustomData<T>(string formatName)
        {
            var obj = Clipboard.GetDataObject();
            if (obj == null)
            {
                return default;
            }
            var data = obj.GetData(formatName, true);
            return (T?)data;
        }

        public bool HasString() => Clipboard.ContainsText();

        public bool HasImage() => Clipboard.ContainsImage();

        public bool HasAudio() => Clipboard.ContainsAudio();

        public bool HasBinary() => Clipboard.ContainsData(PluginHelper.BinaryFormatName);

        public bool HasBinaryCompatible()
            => HasString() || HasImage() || Clipboard.ContainsAudio() || HasBinary();

        public bool HasFileList() => Clipboard.ContainsFileDropList();

        public bool HasFormat(string formatName) => Clipboard.ContainsData(formatName);
    }
}
