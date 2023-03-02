using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using JasonShave.Azure.Communication.Service.JobRouter.Sdk.Contracts.V2021_10_20_preview.Models;
using JasonShave.Azure.Communication.Service.JobRouter.Sdk.Contracts.V2021_10_20_preview.Events;

namespace JobRouterTelemetry.Models
{
    public class WorkerStatistics
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public WorkerState State { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;
        public ICollection<QueueInfo> RegisteredQueues { get; set; } = new List<QueueInfo>();
        public ICollection<ChannelConfiguration> RegisteredChannels { get; set; }
        public Dictionary<string, int> RegisteredAbilities { get; set; }
        public Dictionary<string, object> WorkerMetaData { get; set; }
        public decimal VacantCapacity { get; set; }
        public string AssignedJob { get; set; }
        public List<RouterWorkerOfferAccepted> OfferAccepted { get; set; } = new List<RouterWorkerOfferAccepted> { };
        public List<RouterWorkerOfferDeclined> OfferDeclined { get; set; } = new List<RouterWorkerOfferDeclined> { };
        public List<RouterWorkerOfferExpired> OfferExpired { get; set; } = new List<RouterWorkerOfferExpired> { };
        public List<RouterWorkerOfferIssued> OfferIssued { get; set; } = new List<RouterWorkerOfferIssued> { };
        public List<RouterWorkerOfferRevoked> OfferRevoked { get; set; } = new List<RouterWorkerOfferRevoked> { };
        public List<RouterJobCompleted> JobsCompleted { get; set; } = new List<RouterJobCompleted> { };

        public enum WorkerState
        {
            Registered,
            DeRegistered,
            Draining
        }
    }
}
