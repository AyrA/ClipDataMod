using Plugin;
using Plugin.MenuItems;
using System.Reflection;
using System.Security.Cryptography;

namespace ClipDataMod.BuiltinFunctions
{
    internal class Security : BasePlugin
    {
        private static readonly Dictionary<string, Type> hashes = new()
        {
            { "SHA1",      typeof(SHA1)     },
            { "SHA2: 256", typeof(SHA256)   },
            { "SHA2: 384", typeof(SHA384)   },
            { "SHA2: 512", typeof(SHA512)   },
            { "SHA3: 256", typeof(SHA3_256) },
            { "SHA3: 384", typeof(SHA3_384) },
            { "SHA3: 512", typeof(SHA3_512) },
        };

        public override string Name => "Internal security plugin";

        public override string Author => "AyrA";

        public override Version Version =>
            Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");

        public override Uri? Url => new("https://github.com/AyrA/ClipDataMod");

        public Security() : base(new BaseMenuDescriptor("Security"))
        {
            var hasher = new BaseMenuDescriptor("Hashing");
            _menuDescriptor.Items.Add(hasher);
            _menuDescriptor.Items.Add(new BaseMenuDescriptor("Clear Data", null, (_) => Clipboard.Clear()));
            foreach (var kp in hashes)
            {
                hasher.Items.Add(new BaseMenuDescriptor(kp.Key, null, ComputeHash));
            }
        }

        private void ComputeHash(IMenuDescriptor menuDescriptor)
        {
            const BindingFlags pub = BindingFlags.Static | BindingFlags.Public;
            if (hashes.TryGetValue(menuDescriptor.Title, out var algType))
            {
                var data = PluginHelper.Clipboard.GetBinary();
                if (data == null)
                {
                    return;
                }
                var constructor = algType.GetMethod("Create", pub, Type.EmptyTypes)
                    ?? throw new InvalidOperationException($"Algorithm '{menuDescriptor.Title}' has no appropriate constructor");
                var alg = (HashAlgorithm)(constructor.Invoke(null, null) ?? throw null!);
                PluginHelper.Clipboard.SetBinary(alg.ComputeHash(data));
            }
        }
    }
}
