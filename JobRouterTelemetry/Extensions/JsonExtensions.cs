using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JobRouterTelemetry.Extensions
{
    public static class JsonExtensions
    {
        public static void PopulateObject<T>(this JToken jt, T target)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Populate(jt.CreateReader(), target);
        }
    }
}
