﻿using Newtonsoft.Json;

namespace JobRouterTelemetry.Models
{
    public class QueueStatistics
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public string JobId { get; set; }

        public string? ChannelReference { get; set; }

        public DateTimeOffset? CreateTime { get; set; }

        public string ChannelId { get; set; }

        public string? QueueId { get; set; }

        public int? Priority { get; set; }

        public Dictionary<string, object> Labels { get; set; }

        public Dictionary<string, object> Tags { get; set; }

        //public List<WorkerSelector>? RequestedWorkerSelectors { get; set; }

        //public List<WorkerSelector>? AttachedWorkerSelectors { get; set; }
    }
}
