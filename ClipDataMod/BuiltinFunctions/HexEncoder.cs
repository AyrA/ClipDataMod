using Plugin;
using Plugin.MenuItems;
using System.Reflection;
using System.Text;

namespace ClipDataMod.BuiltinFunctions
{
    internal class HexEncoder : BasePlugin
    {
        private const string Codepage = ".☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼ !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~⌂ÇüéâäàåçêëèïîìÄÅÉæÆôöòûùÿÖÜ¢£¥₧ƒáíóúñÑªº¿⌐¬½¼¡«»░▒▓│┤╡╢╖╕╣║╗╝╜╛┐└┴┬├─┼╞╟╚╔╩╦╠═╬╧╨╤╥╙╘╒╓╫╪┘┌█▄▌▐▀αßΓπΣσµτΦΘΩδ∞φε∩≡±≥≤⌠⌡÷≈°∙·√ⁿ²■ ";

        public override string Name => "Internal hex encoder plugin";

        public override string Author => "AyrA";

        public override Version Version =>
            Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

        public override Uri? Url => new("https://github.com/AyrA/ClipDataMod");

        public HexEncoder() : base(new BaseMenuDescriptor("Hex Encoder"))
        {
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Format: AA-BB", null, (x) => ToHex("-")));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Format: AA:BB", null, (x) => ToHex(":")));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Format: AA BB", null, (x) => ToHex(" ")));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Format: AABB", null, (x) => ToHex("")));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Format: Hex Dump", null, HexDump));
        }

        private void HexDump(IMenuDescriptor menuDescriptor)
        {
            try
            {
                var data = PluginHelper.Clipboard.GetBinary();
                if (data == null || data.Length == 0)
                {
                    throw new Exception("Clipboard does not contain binary compatible data");
                }
                int offset = 0;
                var sb = new StringBuilder();

                foreach (var chunk in data.Chunk(16))
                {
                    var hex = string.Join(" ", chunk.Select(m => m.ToString("X2")));
                    var text = string.Concat(chunk.Select(m => Codepage[m]));
                    sb.AppendFormat("{0:X8}\t{1,-47}\t{2}\r\n", offset, hex, text);
                    offset += chunk.Length;
                }
                PluginHelper.Clipboard.SetString(sb.ToString());
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error(ex.Message, "Hex encoder");
            }
        }

        private static void ToHex(string? delimiter)
        {
            try
            {
                var parts = ToHex();
                if (parts == null || parts.Length == 0)
                {
                    throw new Exception("Clipboard does not contain binary compatible data");
                }
                if (string.IsNullOrEmpty(delimiter))
                {
                    PluginHelper.Clipboard.SetString(string.Concat(parts));
                }
                else
                {
                    PluginHelper.Clipboard.SetString(string.Join(delimiter, parts));
                }
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error(ex.Message, "Hex encoder");
            }
        }

        private static string[]? ToHex()
        {
            var bin = PluginHelper.Clipboard.GetBinary();
            if (bin == null || bin.Length == 0)
            {
                return null;
            }
            return bin.Select(m => m.ToString("X2")).ToArray();
        }
    }
}
