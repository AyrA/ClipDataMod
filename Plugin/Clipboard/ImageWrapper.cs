namespace Plugin.Clipboard
{
    /// <summary>
    /// Represents the base interface of an image wrapper
    /// </summary>
    /// <remarks>
    /// Types that implement <see cref="IClipboard"/>
    /// should implement <see cref="IImageWrapper{T}"/> instead of this type
    /// </remarks>
    public interface IImageWrapper
    {
        /// <summary>
        /// Gets the width of the image
        /// </summary>
        /// <remarks>
        /// For raster graphics, the unit of this value is in pixels,
        /// for vector graphics, the unit of this value is undefined
        /// </remarks>
        int Width { get; }
        /// <summary>
        /// Gets the height of the image
        /// </summary>
        /// <remarks>
        /// For raster graphics, the unit of this value is in pixels,
        /// for vector graphics, the unit of this value is undefined
        /// </remarks>
        int Height { get; }
        /// <summary>
        /// Gets the data type stored in <see cref="Data"/>
        /// </summary>
        ImageType DataType { get; }
        /// <summary>
        /// Gets the raw image data
        /// according to <see cref="DataType"/>
        /// </summary>
        /// <remarks>
        /// This value is always a complete and valid container
        /// of the type specified by <see cref="DataType"/>
        /// </remarks>
        byte[] Data { get; }
    }

    /// <summary>
    /// Represents image data that is retrieved and stored in the clipboard
    /// </summary>
    /// <typeparam name="T">Underlying image type</typeparam>
    /// <remarks>
    /// The <typeparamref name="T"/> type is usually platform specific.
    /// Plugins that want to work in a platform indepentent manner
    /// should only work with the properties from the base type <see cref="IImageWrapper"/>,
    /// or offer platform specific binaries
    /// </remarks>
    public interface IImageWrapper<T> : IImageWrapper
    {
        /// <summary>
        /// Image data
        /// </summary>
        T Image { get; }
    }

    /// <summary>
    /// Image type
    /// </summary>
    /// <remarks>
    /// Positive integer values are reserved for official, predefined types.
    /// Negatibe integer values are reserved for private use,
    /// custom formats, and formats not defined in this enumeration
    /// </remarks>
    public enum ImageType
    {
        /// <summary>
        /// The image data consists of raw pixel data without any header information
        /// </summary>
        Raw = 0,
        /// <summary>
        /// The image data consists of a device independent bitmap,
        /// commonly represented using a file with a ".bmp" extension.
        /// </summary>
        /// <remarks>
        /// Format specification:
        /// <a href="https://learn.microsoft.com/en-us/windows/win32/gdi/device-independent-bitmaps">
        /// Microsoft: Device Independent Bitmap
        /// </a>
        /// </remarks>
        Bmp = 1,
        /// <summary>
        /// The image data consists of a portable network graphics image,
        /// commonly represented using a file with a ".png" extension.
        /// </summary>
        /// <remarks>
        /// Format specification:
        /// <a href="http://www.libpng.org/pub/png/spec/1.2/PNG-Contents.html">
        /// PNG (Portable Network Graphics) Specification, Version 1.2
        /// </a>
        /// </remarks>
        Png = 2,
        /// <summary>
        /// The image data consists of a Joint Photographic Experts Group image,
        /// commonly represented using a file with a ".jpg" or ".jpeg" extension.
        /// </summary>
        /// <remarks>
        /// There's no assumption made as to whether the image is represented in JFIF or Exif format.
        /// Exif format is more common<br />
        /// Exif format specification:
        /// <a href="https://web.archive.org/web/20131111073619/http://www.exif.org/Exif2-1.PDF">
        /// Digital Still Camera Image File Format Standard
        /// </a>
        /// </remarks>
        Jpg = 3,
        /// <summary>
        /// The image data consists of a Tag Image File Format image,
        /// commonly represented using a file with a ".tiff" or ".tif" extension.
        /// </summary>
        /// <remarks>
        /// Format specification:
        /// <a href="https://developer.adobe.com/content/dam/udp/en/open/standards/tiff/TIFF6.pdf">
        /// TIFF6.final.9509 - TIFF6.pdf
        /// </a>
        /// </remarks>
        Tiff = 4,
        /// <summary>
        /// The image data consists of an AV1 Image File Format image,
        /// commonly represented using a file with a ".avif" extension.
        /// </summary>
        /// <remarks>
        /// Format specification:
        /// <a href="https://aomediacodec.github.io/av1-avif/">
        /// AV1 Image File Format (AVIF)
        /// </a>
        /// </remarks>
        Av1 = 5,
        /// <summary>
        /// The image data consists of an WebP image,
        /// commonly represented using a file with a ".webp" extension.
        /// </summary>
        /// <remarks>
        /// Format specification:
        /// <a href="https://developers.google.com/speed/webp">
        /// An image format for the Web
        /// </a>
        /// </remarks>
        Webp = 6,
        /// <summary>
        /// The image data consists of a Scalable Vector Graphics image,
        /// commonly represented using a file with a ".svg" extension.
        /// </summary>
        /// <remarks>
        /// <a href="https://www.w3.org/Graphics/SVG/">
        /// Scalable Vector Graphics
        /// </a>
        /// </remarks>
        Svg = 7
    }
}
