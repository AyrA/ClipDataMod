using Plugin;
using Plugin.MenuItems;
using System.Reflection;
using System.Text;

namespace ClipDataMod.BuiltinFunctions
{
    internal class Info : BasePlugin
    {
        public override string Name => "Internal info plugin";

        public override string Author => "AyrA";

        public override Version Version =>
            Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

        public override Uri? Url => new("https://github.com/AyrA/ClipDataMod");

        public Info() : base(new BaseMenuDescriptor("Info", null, ShowInfo))
        {
        }

        private static void ShowInfo(IMenuDescriptor menuDescriptor)
        {
            var sb = new StringBuilder();
            var formats = PluginHelper.Clipboard.GetFormats();
            if (formats == null || formats.Length == 0)
            {
                sb.AppendLine("Clipboard is empty");
            }
            else
            {
                sb.AppendLine("The following clipboard formats are present:");
                foreach (var chunk in formats.Chunk(3))
                {
                    sb.AppendLine(string.Join(", ", chunk));
                }
            }
            PluginHelper.Dialog.Info(sb.ToString(), "Clipboard information");
        }
    }
}
