//This plugin is to test the exception handling and should not be included in release builds
#if DEBUG
using Plugin;
using Plugin.MenuItems;
using System.Reflection;

namespace ClipDataMod.BuiltinFunctions
{
    internal class TestPlugin : BasePlugin
    {
        public override event MessageEventHandler Message = delegate { };

        private bool throwOnClipboardChange = false;
        private bool throwOnMessage = false;
        private bool throwOnDescriptorCrash = false;

        public override string Name => nameof(TestPlugin);

        public override string Author => "AyrA";

        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

        public override Uri? Url => new("https://github.com/AyrA/ClipDataMod");

        public TestPlugin() : base(new BaseMenuDescriptor("Test plugin"))
        {
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Throw an exception", null, ThrowNow));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Crash on clipboard change", null, ToggleChangeCrash));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Crash on message event", null, ToggleEventCrash));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Send event", null, SendEvent));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Crash on descriptor retrieval", null, ToggleDescriptorCrash));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Access settings", null, (_) =>
            {
                GetValue<string>("test");
            }));
        }

        private void ToggleDescriptorCrash(IMenuDescriptor menuDescriptor)
        {
            throwOnDescriptorCrash = !throwOnDescriptorCrash;
            menuDescriptor.Checked = throwOnDescriptorCrash;
        }

        private void SendEvent(IMenuDescriptor menuDescriptor)
        {
            Message("TestMessage", "Test data");
        }

        private void ToggleEventCrash(IMenuDescriptor menuDescriptor)
        {
            throwOnMessage = !throwOnMessage;
            menuDescriptor.Checked = throwOnMessage;
        }

        private void ToggleChangeCrash(IMenuDescriptor menuDescriptor)
        {
            throwOnClipboardChange = !throwOnClipboardChange;
            menuDescriptor.Checked = throwOnClipboardChange;
        }

        private void ThrowNow(IMenuDescriptor menuDescriptor)
        {
            throw new Exception("User thrown test error");
        }

        public override IMenuDescriptor GetDescriptor()
        {
            if (throwOnDescriptorCrash)
            {
                throw new Exception("User generated descriptor exception");
            }
            return base.GetDescriptor();
        }

        public override void OnClipboardUpdate()
        {
            if (throwOnClipboardChange)
            {
                throw new Exception("User generated clipboard update exception");
            }
            base.OnClipboardUpdate();
        }

        public override void OnMessage(IPlugin? sender, PluginEventArgs eventArgs)
        {
            if (throwOnMessage)
            {
                throw new Exception("User generated exception in plugin event");
            }
            base.OnMessage(sender, eventArgs);
        }
    }
}
#endif
