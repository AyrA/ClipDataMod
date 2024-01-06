using Plugin.Clipboard;

namespace ClipDataMod
{
    internal class WindowsImage : IImageWrapper<Image>
    {
        private readonly Image img;
        private readonly byte[] imageData;

        public Image Image => img;

        public int Width => img.Width;

        public int Height => img.Height;

        public ImageType DataType => ImageType.Raw;

        public byte[] Data => imageData;

        public WindowsImage(Image i)
        {
            ArgumentNullException.ThrowIfNull(i);
            img = i;
            using var ms = new MemoryStream();
            i.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            imageData = ms.ToArray();
        }
    }
}
