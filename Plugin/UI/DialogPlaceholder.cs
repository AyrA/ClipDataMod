namespace Plugin.UI
{
    internal class DialogPlaceholder : IDialog
    {
        private readonly NotImplementedException NI = new();

        public bool MustSync => throw NI;

        public ButtonResult Error(string _1, string _2, DialogButtons _3 = DialogButtons.OK) => throw NI;
        public ButtonResult Info(string _1, string _2, DialogButtons _3 = DialogButtons.OK) => throw NI;
        public ButtonResult Warn(string _1, string _2, DialogButtons _3 = DialogButtons.OK) => throw NI;
        public void Exception(Exception _1, string _2, IPlugin? _3) => throw NI;

        public string? OpenFile(string _1, FilterMask _2) => throw NI;
        public string? OpenFile(string _1, IEnumerable<FilterMask> _2) => throw NI;
        public string? OpenFile(string _1, IEnumerable<FilterMask> _2, string _3) => throw NI;
        public string? OpenFile(string _1, IEnumerable<FilterMask> _2, string _3, string _4) => throw NI;

        public string[] OpenFiles(string _1, FilterMask _2) => throw NI;
        public string[] OpenFiles(string _1, IEnumerable<FilterMask> _2) => throw NI;
        public string[] OpenFiles(string _1, IEnumerable<FilterMask> _2, string _3) => throw NI;
        public string[] OpenFiles(string _1, IEnumerable<FilterMask> _2, string _3, string _4) => throw NI;

        public string? SaveFile(string _1, FilterMask _2) => throw NI;
        public string? SaveFile(string _1, IEnumerable<FilterMask> _2) => throw NI;
        public string? SaveFile(string _1, IEnumerable<FilterMask> _2, string _3) => throw NI;
        public string? SaveFile(string _1, IEnumerable<FilterMask> _2, string? _3, string _4) => throw NI;

        public string? SelectDirectory(string _1) => throw NI;
        public string? SelectDirectory(string _1, string _2) => throw NI;

        public void NotifyInfo(string text, string title) => throw NI;
        public void NotifyWarn(string text, string title) => throw NI;
        public void NotifyError(string text, string title) => throw NI;
    }
}
