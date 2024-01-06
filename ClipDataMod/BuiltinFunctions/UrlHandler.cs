using Plugin;
using Plugin.MenuItems;
using Plugin.UI;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ClipDataMod.BuiltinFunctions
{
    internal partial class UrlHandler : BasePlugin
    {
        public override string Name => "Internal URL handler plugin";

        public override string Author => "AyrA";

        public override Version Version =>
            Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

        public override Uri? Url => new("https://github.com/AyrA/ClipDataMod");

        private bool autoTrackerRemove;
        private readonly string[] schemes = ["https", "http"];

        public UrlHandler() : base(new BaseMenuDescriptor("URL"))
        {
            autoTrackerRemove = false;

            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Open in default application", null, OpenUrl));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Save target as file...", null, DownloadUrlFile));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Save target to clipboard", null, DownloadUrlCb));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Remove 'UTM' tracker", null, (_) => RemoveTracker(false)));
            var autoMenu = new BaseMenuDescriptor("Automatically remove 'UTM' tracker", null, ToggleAutoTrackerRemove);
            _menuDescriptor.Items.Add(autoMenu);
            //Load settings
            autoTrackerRemove = autoMenu.Checked = GetValue("AutoRemove", false);
        }

        private void OpenUrl(IMenuDescriptor menuDescriptor)
        {
            if (Uri.TryCreate(PluginHelper.Clipboard.GetString(), UriKind.Absolute, out var url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(url.ToString()) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    PluginHelper.Dialog.Exception(ex, $"Failed to open {url}", this);
                }
            }
            else
            {
                PluginHelper.Dialog.Error("Clipboard does not contain valid URL", Name);
            }
        }

        private async void DownloadUrlCb(IMenuDescriptor menuDescriptor)
        {
            if (!Uri.TryCreate(PluginHelper.Clipboard.GetString(), UriKind.Absolute, out var url))
            {
                PluginHelper.Dialog.Error("Clipboard does not contain valid URL", Name);
                return;
            }
            if (!schemes.Contains(url.Scheme.ToLower()))
            {
                PluginHelper.Dialog.Error($"URL type {url.Scheme} is not supported", Name);
                return;
            }
            try
            {
                using var cli = new HttpClient();
                using var response = await cli.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var btn = PluginHelper.Dialog.Warn(
                        $"Server responded with non-success code {(int)response.StatusCode} {response.ReasonPhrase}\r\nDownload anyways?", Name, DialogButtons.YesNo);
                    if (btn != ButtonResult.Yes)
                    {
                        return;
                    }
                }
                var fileName = PluginHelper.Temp.CreateTempFile("temp.bin");
                long size = 0;
                using (var fs = File.Create(fileName))
                {
                    await response.Content.CopyToAsync(fs);
                    size = fs.Length;
                }
                PluginHelper.Sync(() =>
                {
                    if (size == 0)
                    {
                        PluginHelper.Dialog.Warn("Server response was empty. Clipboard not updated", Name);
                    }
                    if (size > 1024 * 1024 * 100)
                    {
                        var btn = PluginHelper.Dialog.Warn(
                            @"Data exceeds 100 MB. Copy as binary anyways?
[Yes] Copy as binary
[No] Copy as file
[Cancel] Do not copy", Name, DialogButtons.YesNoCancel);
                        if (btn == ButtonResult.Cancel)
                        {
                            try
                            {
                                File.Delete(fileName);
                            }
                            catch { /*NOOP*/ }
                            return;
                        }
                        if (btn == ButtonResult.Yes)
                        {
                            PluginHelper.Clipboard.SetBinary(File.ReadAllBytes(fileName));
                        }
                        else
                        {
                            PluginHelper.Clipboard.SetFileList([fileName], Plugin.Clipboard.PreferredFileOperation.Move);
                        }
                    }
                    else
                    {
                        PluginHelper.Clipboard.SetBinary(File.ReadAllBytes(fileName));
                    }
                });
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error($"Failed to download {url}\r\n{ex.Message}", Name);
            }
        }

        private async void DownloadUrlFile(IMenuDescriptor menuDescriptor)
        {
            if (!Uri.TryCreate(PluginHelper.Clipboard.GetString(), UriKind.Absolute, out var url))
            {
                PluginHelper.Dialog.Error("Clipboard does not contain valid URL", Name);
                return;
            }
            if (!schemes.Contains(url.Scheme.ToLower()))
            {
                PluginHelper.Dialog.Error($"URL type {url.Scheme} is not supported", Name);
                return;
            }
            try
            {
                using var cli = new HttpClient();
                using var response = await cli.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var btn = PluginHelper.Dialog.Warn(
                        $"Server responded with non-success code {(int)response.StatusCode} {response.ReasonPhrase}\r\nDownload anyways?", Name, DialogButtons.YesNo);
                    if (btn != ButtonResult.Yes)
                    {
                        return;
                    }
                }
                string? fileName = null;
                //Try to get the correct file name. First by looking at the appropriate header
                if (response.Headers.TryGetValues("Content-Disposition", out var fileNameValues))
                {
                    foreach (var fileNameValue in fileNameValues)
                    {
                        var temp = MakeValidFileName(ExtractFileName(fileNameValue));
                        if (!string.IsNullOrEmpty(temp))
                        {
                            fileName = temp;
                        }
                    }
                }
                //If no header given, try URL path (first from final URL, then from original URL
                if (response.RequestMessage?.RequestUri != null)
                {
                    fileName ??= MakeValidFileName(response.RequestMessage.RequestUri.AbsolutePath);
                }
                fileName ??= MakeValidFileName(url.AbsolutePath);

                //Generate a file name based on the Content-Type header
                if (response.Headers.TryGetValues("Content-Type", out var contentTypeValues))
                {
                    fileName ??= MakeValidFileName(GenerateNameByHeader(
                        url.DnsSafeHost, contentTypeValues.FirstOrDefault()));
                }

                var finalName = fileName ?? "unknown.bin";

                PluginHelper.Sync(() =>
                {
                    finalName = PluginHelper.Dialog.SaveFile("Download file", FilterMask.AllFiles, null, finalName);
                });
                if (string.IsNullOrEmpty(finalName))
                {
                    return;
                }
                using var fs = File.Create(finalName);
                await response.Content.CopyToAsync(fs);
                PluginHelper.Sync(() =>
                {
                    PluginHelper.Dialog.NotifyInfo("File download complete", Name);
                });
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error($"Failed to download {url}\r\n{ex.Message}", Name);
            }
        }

        private void ToggleAutoTrackerRemove(IMenuDescriptor menuDescriptor)
        {
            menuDescriptor.Checked = !menuDescriptor.Checked;
            autoTrackerRemove = menuDescriptor.Checked;
            SetValue("AutoRemove", autoTrackerRemove);
        }

        public override void OnClipboardUpdate()
        {
            if (autoTrackerRemove && PluginHelper.Clipboard.HasString())
            {
                RemoveTracker(true);
            }
            base.OnClipboardUpdate();
        }

        private void RemoveTracker(bool notify)
        {
            var s = PluginHelper.Clipboard.GetString();
            if (string.IsNullOrEmpty(s))
            {
                return;
            }
            if (Uri.TryCreate(s, UriKind.Absolute, out var uri))
            {
                if (schemes.Contains(uri.Scheme.ToLower()))
                {
                    //Filter parts that start with "utm_"
                    var query = uri.Query;
                    if (string.IsNullOrEmpty(query) || query == "?")
                    {
                        return;
                    }
                    var parts = query[1..].Split('&');
                    var keep = new List<string>();
                    int count = 0;
                    foreach (var part in parts)
                    {
                        if (!part.StartsWith("utm_", StringComparison.OrdinalIgnoreCase))
                        {
                            keep.Add(part);
                        }
                        else
                        {
                            ++count;
                        }
                    }
                    if (count == 0) //Do not update URL if nothing was removed
                    {
                        return;
                    }

                    var baseUrl = uri.GetLeftPart(UriPartial.Path);

                    if (keep.Count > 0)
                    {
                        baseUrl += "?" + string.Join("&", keep);
                    }
                    //Keep existing fragment if any
                    if (!string.IsNullOrEmpty(uri.Fragment) && uri.Fragment.Length > 1)
                    {
                        baseUrl += uri.Fragment;
                    }
                    //Only set the URL if it differs
                    if (uri.OriginalString != baseUrl)
                    {
                        PluginHelper.Clipboard.SetString(baseUrl);
                        if (notify)
                        {
                            PluginHelper.Dialog.NotifyInfo(
                                $"{count} tracker parameters were removed from the URL", Name);
                        }
                    }
                }
            }
        }

        private static string GenerateNameByHeader(string hostName, string? contentTypeValue)
        {
            if (string.IsNullOrWhiteSpace(contentTypeValue))
            {
                return $"{hostName}.bin";
            }
            //Use lowercase and cut off extra params
            var contentType = contentTypeValue.ToLowerInvariant().Split(';')[0];
            var fallbackExt = contentType.Split('/').Last();
            return contentType switch
            {
                "text/plain" => $"{hostName}.txt",
                "text/javascript" => $"{hostName}.js",
                "audio/mpeg" => $"{hostName}.mp3",
                "application/octet-stream" => $"{hostName}.bin",
                _ => $"{hostName}.{fallbackExt}"
            };
        }

        private static string? MakeValidFileName(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }
            //If an URL path ends in "/", it does not end in a file name
            if (fileName.EndsWith('/'))
            {
                return null;
            }

            var fn = Path.GetFileName(fileName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
            if (fn == null)
            {
                return null;
            }
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fn = fn.Replace(c, '_');
            }
            return fn;
        }

        private static string? ExtractFileName(string contentDispositionValue)
        {
            if (string.IsNullOrWhiteSpace(contentDispositionValue))
            {
                return null;
            }
            var r = ContentDispositionNameExtractor();
            var m = r.Match(contentDispositionValue);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return null;
        }

        [GeneratedRegex(@"\bfilename\s*=\s*""([^""]+)""", RegexOptions.IgnoreCase, "de-CH")]
        private static partial Regex ContentDispositionNameExtractor();
    }
}
