using Plugin;
using System.Text.RegularExpressions;

namespace ClipDataMod.Forms
{
    /// <summary>
    /// Shows exception data in a user friendly manner
    /// </summary>
    public partial class ExceptionDialog : Form
    {
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
            while (ex != null)
            {
                errorList.Add(ex);
                ex = ex.InnerException!;
            }
            this.description = description;
            this.plugin = plugin;
            InitializeComponent();
            RenderError();
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
            //TODO
        }

        private void CbFilter_CheckedChanged(object sender, EventArgs e)
        {
            RenderError();
        }

        [GeneratedRegex(@":[^:\s]+\s+\d+$")]
        private static partial Regex LineFilter();
    }
}
