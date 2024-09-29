using Plugin;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ClipDataMod.Forms
{
    /// <summary>
    /// Shows exception data in a user friendly manner
    /// </summary>
    public partial class ExceptionDialog : Form
    {
        private record BugReport(string Name, string Author, string Version, ExceptionContainer[] Errors);

        private static readonly string[] permittedSchemes = [Uri.UriSchemeHttp, Uri.UriSchemeHttps, Uri.UriSchemeMailto];
        private static readonly JsonSerializerOptions jsonOpt = new(JsonSerializerDefaults.General) { WriteIndented = true };
        private int errorIndex = 0;
        private readonly List<Exception> errorList = [];
        private readonly string description;
        private readonly IPlugin? plugin;

        private Exception CurrentException => errorList[errorIndex];


        /// <summary>
        /// Creates a new dialog with the given exception
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="description">Error description</param>
        /// <param name="plugin">Plugin that caused the error, or null if not a plugin</param>
        public ExceptionDialog(Exception ex, string description, IPlugin? plugin)
        {
            ArgumentNullException.ThrowIfNull(ex);
            Stack<Exception> exceptions = new();
            exceptions.Push(ex);
            while (exceptions.Count > 0)
            {
                var current = exceptions.Pop();
                errorList.Add(current);
                if (current is AggregateException ae)
                {
                    if (ae.InnerException != null && !ae.InnerExceptions.Contains(ae.InnerException))
                    {
                        exceptions.Push(ae.InnerException);
                    }

                    foreach (var err in ae.InnerExceptions)
                    {
                        exceptions.Push(err);
                    }
                }
                else
                {
                    if (current.InnerException != null)
                    {
                        exceptions.Push(current.InnerException);
                    }
                }
            }
            this.description = description;
            this.plugin = plugin;
            InitializeComponent();
            RenderError();
            BtnExport.Enabled = plugin != null;
        }

        private void RenderError()
        {
            LblDesc.Text = description;
            LblType.Text = "Error type: " +
                (CurrentException.GetType().FullName ?? CurrentException.GetType().Name);
            LblMessage.Text = "Error message: " + CurrentException.Message;
            LblPlugin.Text = GetPluginName();
            TbStack.Text = GetStack();
            BtnPrev.Enabled = errorIndex > 0;
            BtnNext.Enabled = errorIndex < errorList.Count - 1;
        }

        private string GetPluginName()
        {
            if (plugin == null)
            {
                return "Plugin: <None>";
            }
            return $"Plugin: '{plugin.Name}' {plugin.Version} by {plugin.Author}";
        }

        private string GetStack()
        {
            var stack = CurrentException.StackTrace ?? "No stack information is available. This exception was likely created, but not thrown";
            if (CbFilter.Checked)
            {
                var r = LineFilter();
                var lines = stack
                    .Split('\n')
                    .Select(m => m.TrimEnd())
                    //Matches strings that end in ":Line 99" without enforcing "Line" to be english
                    .Where(m => r.IsMatch(m));
                return string.Join(Environment.NewLine, lines);
            }
            return stack;
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            if (errorIndex > 0)
            {
                --errorIndex;
                RenderError();
            }
            else
            {
                PluginHelper.Dialog.Info("You are already viewing the top most error", "Already at the start");
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (errorIndex < errorList.Count - 1)
            {
                ++errorIndex;
                RenderError();
            }
            else
            {
                PluginHelper.Dialog.Info("You are already viewing the bottom most error", "Already at the end");
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
            errorList.Clear();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (plugin == null)
            {
                BtnExport.Enabled = false;
                return;
            }

            var model = new BugReport(plugin.Name, plugin.Author, plugin.Version.ToString(), [.. errorList.Select(m => new ExceptionContainer(m))]);
            try
            {
                var errstr = JsonSerializer.Serialize(model, jsonOpt);
                PluginHelper.Clipboard.Clear();
                PluginHelper.Clipboard.SetString(errstr);
                if (plugin.Url != null && permittedSchemes.Contains(plugin.Url.Scheme, StringComparer.InvariantCultureIgnoreCase))
                {
                    if (PluginHelper.Dialog.Info($"Error details were exported to clipboard. Open '{plugin.Url}' now?", "Export complete", Plugin.UI.DialogButtons.YesNo) == Plugin.UI.ButtonResult.Yes)
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo(plugin.Url.ToString()) { UseShellExecute = true });
                        }
                        catch
                        {
                            PluginHelper.Dialog.Error($"Unable to open '{plugin.Url}'", "Unable to launch browser");
                        }
                    }
                }
                else
                {
                    PluginHelper.Dialog.Info($"Error details were exported to clipboard. No report URL is available in the plugin", "Export complete");
                }
            }
            catch (Exception ex)
            {
                PluginHelper.Dialog.Error($"Cannot create an automatic bug report. At least one exception contains unserializable data.\r\n{ex.Message}", "Unable to export data");
            }
        }

        private void CbFilter_CheckedChanged(object sender, EventArgs e)
        {
            RenderError();
        }

        [GeneratedRegex(@":[^:\s]+\s+\d+$")]
        private static partial Regex LineFilter();
    }
}
