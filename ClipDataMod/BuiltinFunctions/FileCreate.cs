using Plugin;
using Plugin.MenuItems;
using Plugin.UI;
using System.Reflection;

namespace ClipDataMod.BuiltinFunctions
{
    internal class FileCreate : BasePlugin
    {
        public override string Name => "Internal file creator plugin";

        public override string Author => "AyrA";

        public override Version Version =>
            Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

        public override Uri? Url => new("https://github.com/AyrA/ClipDataMod");

        public FileCreate() : base(new BaseMenuDescriptor("Create File"))
        {
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Using 'Save As...' dialog", null, SaveAs));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Convert to clipboard file", null, FileList));
        }

        private void FileList(IMenuDescriptor menuDescriptor)
        {
            try
            {
                var ext = ".bin";
                if (PluginHelper.Clipboard.HasImage())
                {
                    //Special case for images
                    var img = (WindowsImage?)PluginHelper.Clipboard.GetImage();
                    if (img != null)
                    {
                        using (img.Image)
                        {
                            //Windows automatically picks an extension appropriate format.
                            //If a format cannot be found, PNG is used.
                            img.Image.Save(PluginHelper.Temp.CreateTempMoveFile("clipboard.png", []));
                        }
                        return;
                    }
                }
                if (PluginHelper.Clipboard.HasAudio())
                {
                    ext = ".wav";
                }
                else if (PluginHelper.Clipboard.HasString())
                {
                    ext = ".txt";
                }
                var data = PluginHelper.Clipboard.GetBinary() ??
                    throw new Exception("Clipboard is empty");
                PluginHelper.Temp.CreateTempMoveFile($"clipboard{ext}", data);
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error(ex.Message, "Failed to create file");
            }
        }

        private void SaveAs(IMenuDescriptor menuDescriptor)
        {
            var str = PluginHelper.Dialog.SaveFile("Save clipboard to file", GetExt());
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            try
            {
                //Special case for images
                var img = (WindowsImage?)PluginHelper.Clipboard.GetImage();
                if (img != null)
                {
                    using (img.Image)
                    {
                        //Windows automatically picks an extension appropriate format.
                        //If a format cannot be found, PNG is used.
                        img.Image.Save(str);
                    }
                    return;
                }
                var data = PluginHelper.Clipboard.GetBinary()
                    ?? throw new Exception("Clipboard does not contain binary compatible data");
                File.WriteAllBytes(str, data);
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error(ex.Message, "Failed to create file");
            }
        }

        private IEnumerable<FilterMask> GetExt()
        {
            if (PluginHelper.Clipboard.HasImage())
            {
                return [
                    new FilterMask("Image files (png, jpg, bmp)", "*.png", "*.jpg", "*.bmp"),
                    .. FilterMask.AllFiles
                ];
            }
            else if (PluginHelper.Clipboard.HasAudio())
            {
                return [
                    new FilterMask("WAVE audio (wav)", "*.wav"),
                    .. FilterMask.AllFiles
                ];
            }
            else if (PluginHelper.Clipboard.HasString())
            {
                return [
                    new FilterMask("Text (txt)", "*.txt"),
                    .. FilterMask.AllFiles
                ];
            }
            return [
                new FilterMask("Raw binary (bin)", "*.bin"),
                .. FilterMask.AllFiles
            ];
        }
    }
}
