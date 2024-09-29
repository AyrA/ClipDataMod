using Plugin;
using Plugin.MenuItems;
using System.Reflection;
using System.Text.Json;

namespace ClipDataMod.BuiltinFunctions
{
    internal class Json : BasePlugin
    {
        public override string Name => "JSON Tools";

        public override string Author => "AyrA";

        public override Version Version =>
            Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

        public override Uri? Url => new("https://github.com/AyrA/ClipDataMod");

        private readonly JsonSerializerOptions optStrict = new()
        {
            AllowTrailingCommas = false,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.Strict,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = null,
            ReadCommentHandling = JsonCommentHandling.Disallow
        };

        private readonly JsonSerializerOptions optLooseFormat = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true
        };

        private readonly JsonSerializerOptions optLooseCompact = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = false
        };

        public Json() : base(new BaseMenuDescriptor("JSON"))
        {
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Validate", null, Validate));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Format", null, Format));
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Compact", null, Compact));
        }

        private void Validate(IMenuDescriptor menuDescriptor)
        {
            var data = PluginHelper.Clipboard.GetString();
            if (!string.IsNullOrWhiteSpace(data))
            {
                bool isStrict = false;
                bool isLoose = false;
                try
                {
                    JsonSerializer.Deserialize<JsonDocument>(data, optStrict);
                    isStrict = true;
                    isLoose = true;
                }
                catch
                {
                    try
                    {
                        JsonSerializer.Deserialize<JsonDocument>(data, optLooseCompact);
                        isStrict = false;
                        isLoose = true;
                    }
                    catch
                    {
                        //NOOP
                    }
                }
                if (isStrict)
                {
                    PluginHelper.Dialog.Info("JSON is valid", "JSON check");
                }
                else if (isLoose)
                {
                    PluginHelper.Dialog.Warn("JSON is valid only if trailing comas or comments are permitted", "JSON check");
                }
                else
                {
                    PluginHelper.Dialog.Error("JSON is invalid", "JSON check");
                }
            }
        }

        private void Format(IMenuDescriptor menuDescriptor)
        {
            var data = PluginHelper.Clipboard.GetString();
            if (!string.IsNullOrWhiteSpace(data))
            {
                try
                {
                    var doc = JsonSerializer.Deserialize<JsonDocument>(data, optLooseFormat);
                    data = JsonSerializer.Serialize(doc, optLooseFormat);
                    if (!string.IsNullOrEmpty(data))
                    {
                        PluginHelper.Clipboard.Clear();
                        PluginHelper.Clipboard.SetString(data);
                    }
                }
                catch (Exception ex)
                {
                    PluginHelper.Dialog.Error($"Failed to deserialize JSON.\r\n{ex.Message}", "Invalid JSON");
                }
            }
        }

        private void Compact(IMenuDescriptor menuDescriptor)
        {
            var data = PluginHelper.Clipboard.GetString();
            if (!string.IsNullOrWhiteSpace(data))
            {
                try
                {
                    var doc = JsonSerializer.Deserialize<JsonDocument>(data, optLooseCompact);
                    data = JsonSerializer.Serialize(doc, optLooseCompact);
                    if (!string.IsNullOrEmpty(data))
                    {
                        PluginHelper.Clipboard.Clear();
                        PluginHelper.Clipboard.SetString(data);
                    }
                }
                catch (Exception ex)
                {
                    PluginHelper.Dialog.Error($"Failed to deserialize JSON.\r\n{ex.Message}", "Invalid JSON");
                }
            }
        }
    }
}
