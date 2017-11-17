using Newtonsoft.Json;

namespace OS.Core.Queues
{
    internal static class Serializer
    {
        public static string Serialize<T>(T @object)
            where T : class
        {
            return JsonConvert.SerializeObject(@object);
        }
    }
}