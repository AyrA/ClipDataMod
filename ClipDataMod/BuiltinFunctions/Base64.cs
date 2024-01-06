using Plugin;
using Plugin.MenuItems;
using System.Reflection;

namespace ClipDataMod.BuiltinFunctions
{
    internal class Base64 : BasePlugin
    {
        public override string Name => "Internal Base64 plugin";

        public override string Author => "AyrA";

        public override Version Version =>
            Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

        public override Uri? Url => new("https://github.com/AyrA/ClipDataMod");

        public Base64() : base(new BaseMenuDescriptor("Base64"))
        {
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Decode", null, Decode));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Encode", null, Encode));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Encode with CRLF", null, EncodeWithCRLF));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Encode URL safe", null, EncodeUrlSafe));
        }

        private void Decode(IMenuDescriptor menuDescriptor)
        {
            try
            {
                var data = PluginHelper.Clipboard.GetString();
                if (string.IsNullOrEmpty(data))
                {
                    throw new Exception("Clipboard does not contain text");
                }
                //Convert URL B64 variant to regular variant
                data = data.Replace('-', '+').Replace('_', '/');
                //Add padding if it's missing
                var extra = data.Length % 4;
                if (extra == 1)
                {
                    throw new Exception("Invalid Base64 data. Padding is impossible");
                }
                if (extra != 0)
                {
                    data = data.PadRight(data.Length + 4 - extra, '=');
                }
                var bytes = Convert.FromBase64String(data);
                PluginHelper.Clipboard.SetBinary(bytes);
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error(ex.Message, "Unable to decode base64 data");
            }
        }

        private void Encode(IMenuDescriptor menuDescriptor)
        {
            var str = B64(Base64FormattingOptions.None);
            if (!string.IsNullOrEmpty(str))
            {
                Clipboard.SetText(str);
            }
        }

        private void EncodeUrlSafe(IMenuDescriptor menuDescriptor)
        {
            var str = B64(Base64FormattingOptions.None);
            if (!string.IsNullOrEmpty(str))
            {
                str = str
                    .TrimEnd('=')
                    .Replace('/', '_')
                    .Replace('+', '-');
                Clipboard.SetText(str);
            }
        }

        private void EncodeWithCRLF(IMenuDescriptor menuDescriptor)
        {
            var str = B64(Base64FormattingOptions.InsertLineBreaks);
            if (!string.IsNullOrEmpty(str))
            {
                Clipboard.SetText(str);
            }
        }

        private static string? B64(Base64FormattingOptions opt)
        {
            try
            {
                var data = PluginHelper.Clipboard.GetBinary();
                if (data == null || data.Length == 0)
                {
                    throw new Exception("Clipboard does not contain binary compatible data");
                }
                return Convert.ToBase64String(data, opt);
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error(ex.Message, "Unable to encode base64 data");
            }
            return null;
        }
    }
}
