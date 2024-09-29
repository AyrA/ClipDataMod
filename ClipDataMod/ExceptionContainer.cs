using System.Text.Json;

namespace ClipDataMod
{
    internal class ExceptionContainer
    {
        public string Message { get; }

        public string? Source { get; }

        public string Type { get; }

        public string? StackTrace { get; }

        public Dictionary<string, object?> ExtraData { get; }

        public ExceptionContainer(Exception ex)
        {
            Source = ex.Source;
            Message = ex.Message;
            Type = ex.GetType().FullName ?? ex.GetType().Name;
            StackTrace = ex.StackTrace;
            ExtraData = [];
            foreach (var k in ex.Data.Keys)
            {
                if (k == null || !IsSerializable(ex.Data[k]))
                {
                    continue;
                }
                ExtraData[k.ToString() ?? "null"] = ex.Data[k];
            }
        }

        private static bool IsSerializable(object? o)
        {
            try
            {
                JsonSerializer.Serialize(o);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
