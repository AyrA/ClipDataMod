using Plugin;
using Plugin.MenuItems;
using System.Reflection;
using System.Text;

namespace ClipDataMod.BuiltinFunctions
{
    internal class BinaryDecoder : BasePlugin
    {
        public override string Name => "Internal binary decoder plugin";

        public override string Author => "AyrA";

        public override Version Version =>
            Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

        public override Uri? Url => new("https://github.com/AyrA/ClipDataMod");

        public BinaryDecoder() : base(new BaseMenuDescriptor("Binary Decoder"))
        {
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("To UTF8", null, ToUtf8));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("To ANSI", null, ToAnsi));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("To Image", null, ToImage));
        }

        private void ToImage(IMenuDescriptor menuDescriptor)
        {
            if (!PluginHelper.Clipboard.HasBinary() && !PluginHelper.Clipboard.HasImage())
            {
                PluginHelper.Dialog.Warn("Clipboard does not contains image data", "Binary Decoder");
                return;
            }
            try
            {
                var data = PluginHelper.Clipboard.GetBinary();
                if (data == null || data.Length == 0)
                {
                    throw new Exception("Clipboard does not contain binary data");
                }
                using var ms = new MemoryStream(data, false);
                using var bmp = Image.FromStream(ms);
                var img = new WindowsImage(bmp);
                PluginHelper.Clipboard.SetImage(img);
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error(ex.Message, "Binary Decoder");
            }
        }

        private void ToAnsi(IMenuDescriptor menuDescriptor)
        {
            try
            {
                var data = PluginHelper.Clipboard.GetBinary();
                if (data == null || data.Length == 0)
                {
                    throw new Exception("Clipboard does not contain binary data");
                }
                if (data.Contains((byte)0))
                {
                    throw new Exception("Binary data contains nullbytes, which cannot be stored as string in the clipboard");
                }
                var enc = Encoding.GetEncoding(Encoding.Latin1.CodePage,
                    EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                PluginHelper.Clipboard.SetString(enc.GetString(data));
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error(ex.Message, "Binary Decoder");
            }
        }

        private void ToUtf8(IMenuDescriptor menuDescriptor)
        {
            try
            {
                var data = PluginHelper.Clipboard.GetBinary();
                if (data == null || data.Length == 0)
                {
                    throw new Exception("Clipboard does not contain binary data");
                }
                if (data.Contains((byte)0))
                {
                    throw new Exception("Binary data contains nullbytes, which cannot be stored as string in the clipboard");
                }
                var enc = new UTF8Encoding(false, true);
                PluginHelper.Clipboard.SetString(enc.GetString(data));
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error(ex.Message, "Binary Decoder");
            }
        }
    }
}
