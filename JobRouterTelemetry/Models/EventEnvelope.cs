using Microsoft.AspNetCore.Mvc.Diagnostics;
using Newtonsoft.Json;

namespace JobRouterTelemetry.Models
{
    public class EventEnvelope
    {       
        public string EventType { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty(PropertyName = "entityId")]
        public string EntityId { get; set; } 
        public DateTimeOffset EventTime { get; set; } 
        public object EventData { get; set; }
    }
}
