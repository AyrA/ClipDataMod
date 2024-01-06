using ClipDataMod.Forms;
using Plugin;
using Plugin.UI;

namespace ClipDataMod
{
    internal class WindowsDialog : IDialog
    {
        private readonly NotifyIcon notifyIcon;

        public bool MustSync => true;

        internal WindowsDialog(NotifyIcon notifyIcon)
        {
            this.notifyIcon = notifyIcon;
        }

        private static string Filter(string s) => s.Replace('|', '¦');

        private static string ConvertMask(FilterMask mask)
        {
            var masks = string.Join('|', mask.Mask.Select(Filter));
            return $"{Filter(mask.Title)}|{masks}";
        }

        private static string ConvertMask(IEnumerable<FilterMask> masks)
        {
            return string.Join('|', masks.Select(ConvertMask));
        }

        private static OpenFileDialog OpenDlg(string title, string mask,
            string? dir = null, string? name = null)
        {
            var ofd = new OpenFileDialog()
            {
                Title = title,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true,
                Filter = mask,
                ValidateNames = true
            };
            if (!string.IsNullOrEmpty(dir))
            {
                ofd.InitialDirectory = dir;
            }
            if (!string.IsNullOrEmpty(name))
            {
                ofd.FileName = Path.GetFileName(name);
            }
            return ofd;
        }

        private static SaveFileDialog SaveDlg(string title, string mask,
            string? dir = null, string? name = null)
        {
            var ofd = new SaveFileDialog()
            {
                Title = title,
                AutoUpgradeEnabled = true,
                CheckFileExists = false,
                CheckPathExists = true,
                DereferenceLinks = true,
                Filter = mask,
                ValidateNames = true
            };
            if (!string.IsNullOrEmpty(dir))
            {
                ofd.InitialDirectory = dir;
            }
            if (!string.IsNullOrEmpty(name))
            {
                ofd.FileName = Path.GetFileName(name);
            }
            return ofd;
        }

        private static ButtonResult Box(string text, string title, DialogButtons buttons, MessageBoxIcon icon)
        {
            return (ButtonResult)(int)MessageBox.Show(text, title, (MessageBoxButtons)(int)buttons, icon);
        }

        public ButtonResult Error(string text, string title, DialogButtons buttons = DialogButtons.OK)
        {
            return Box(text, title, buttons, MessageBoxIcon.Error);
        }

        public ButtonResult Warn(string text, string title, DialogButtons buttons = DialogButtons.OK)
        {
            return Box(text, title, buttons, MessageBoxIcon.Warning);
        }

        public ButtonResult Info(string text, string title, DialogButtons buttons = DialogButtons.OK)
        {
            return Box(text, title, buttons, MessageBoxIcon.Information);
        }

        public void Exception(Exception error, string description, IPlugin? source)
        {
            PluginHelper.Sync(() =>
            {
                var ed = new ExceptionDialog(error, description, source);
                ed.FormClosed += delegate { ed.Dispose(); };
                ed.Show();
            });
        }

        public string? OpenFile(string title, FilterMask mask)
        {
            using var ofd = OpenDlg(title, ConvertMask(mask));
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileName;
            }
            return null;
        }

        public string? OpenFile(string title, IEnumerable<FilterMask> masks)
        {
            using var ofd = OpenDlg(title, ConvertMask(masks));
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileName;
            }
            return null;
        }

        public string? OpenFile(string title, IEnumerable<FilterMask> masks, string defaultDir)
        {
            using var ofd = OpenDlg(title, ConvertMask(masks), defaultDir);
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileName;
            }
            return null;
        }

        public string? OpenFile(string title, IEnumerable<FilterMask> masks, string defaultDir, string defaultFileName)
        {
            using var ofd = OpenDlg(title, ConvertMask(masks), defaultDir, defaultFileName);
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileName;
            }
            return null;
        }

        public string[] OpenFiles(string title, FilterMask mask)
        {
            using var ofd = OpenDlg(title, ConvertMask(mask));
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileNames;
            }
            return [];
        }

        public string[] OpenFiles(string title, IEnumerable<FilterMask> masks)
        {
            using var ofd = OpenDlg(title, ConvertMask(masks));
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileNames;
            }
            return [];
        }

        public string[] OpenFiles(string title, IEnumerable<FilterMask> masks, string defaultDir)
        {
            using var ofd = OpenDlg(title, ConvertMask(masks), defaultDir);
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileNames;
            }
            return [];
        }

        public string[] OpenFiles(string title, IEnumerable<FilterMask> masks, string defaultDir, string defaultFileName)
        {
            using var ofd = OpenDlg(title, ConvertMask(masks), defaultDir, defaultFileName);
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileNames;
            }
            return [];
        }

        public string? SaveFile(string title, FilterMask mask)
        {
            using var sfd = SaveDlg(title, ConvertMask(mask));
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                return sfd.FileName;
            }
            return null;
        }

        public string? SaveFile(string title, IEnumerable<FilterMask> masks)
        {
            using var sfd = SaveDlg(title, ConvertMask(masks));
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                return sfd.FileName;
            }
            return null;
        }

        public string? SaveFile(string title, IEnumerable<FilterMask> masks, string defaultDir)
        {
            using var sfd = SaveDlg(title, ConvertMask(masks), defaultDir);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                return sfd.FileName;
            }
            return null;
        }

        public string? SaveFile(string title, IEnumerable<FilterMask> masks, string? defaultDir, string defaultFileName)
        {
            using var sfd = SaveDlg(title, ConvertMask(masks), defaultDir, defaultFileName);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                return sfd.FileName;
            }
            return null;
        }

        public string? SelectDirectory(string title)
        {
            using var fbd = new FolderBrowserDialog()
            {
                Description = title,
                AutoUpgradeEnabled = false,
                ShowNewFolderButton = true
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                return fbd.SelectedPath;
            }
            return null;
        }

        public string? SelectDirectory(string title, string selectedDir)
        {
            using var fbd = new FolderBrowserDialog()
            {
                Description = title,
                AutoUpgradeEnabled = false,
                ShowNewFolderButton = true,
                InitialDirectory = selectedDir
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                return fbd.SelectedPath;
            }
            return null;
        }

        public void NotifyInfo(string text, string title)
        {
            notifyIcon.ShowBalloonTip(5000, title, text, ToolTipIcon.Info);
        }

        public void NotifyWarn(string text, string title)
        {
            notifyIcon.ShowBalloonTip(5000, title, text, ToolTipIcon.Warning);
        }

        public void NotifyError(string text, string title)
        {
            notifyIcon.ShowBalloonTip(5000, title, text, ToolTipIcon.Error);
        }
    }
}
