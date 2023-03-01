using Azure.Communication.JobRouter;
using JasonShave.Azure.Communication.Service.JobRouter.Sdk.Contracts.V2021_10_20_preview.Events;
using JobRouterTelemetry.Models;
using Newtonsoft.Json;

namespace JobRouterTelemetry.Models
{
    public class JobStatistics
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public State State { get; set; }
        public string ChannelReference { get; set; }
        public string Channel { get; set; }
        public string QueueId { get; set; }
        public int? Priority { get; set; }
        public string ClassificationPolicyId { get; set; }
        public string DispositionCode { get; set; }
        public string CurrentAssignedWorker { get; set; }
        public Dictionary<DateTimeOffset, string> Notes { get; set; } = new Dictionary<DateTimeOffset, string>();
        public Dictionary<string, object> JobMetaData { get; set; } = new Dictionary<string, object>();
        public DateTimeOffset? InitialQueueTime { get; set; } = null;
        public DateTimeOffset? InitialAsignmentTime { get; set; } = null;
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset JobCompletedTime { get; set; }
        public DateTimeOffset JobClosedTime { get; set; }
        public DateTimeOffset JobCancelledTime { get; set; }
        public List<RouterJobQueued>? QueuedEvents { get; set; } = new List<RouterJobQueued>();
        public List<RouterJobClassified>? ClassifiedEvents { get; set; } = new List<RouterJobClassified>();
        public List<RouterWorkerOfferAccepted> OfferAccepted { get; set; } = new List<RouterWorkerOfferAccepted> { };
        public List<RouterWorkerOfferDeclined> OfferDeclined { get; set; } = new List<RouterWorkerOfferDeclined> { };
        public List<RouterWorkerOfferExpired> OfferExpired { get; set; } = new List<RouterWorkerOfferExpired> { };
        public List<RouterWorkerOfferIssued> OfferIssued { get; set; } = new List<RouterWorkerOfferIssued> { };
        public List<RouterWorkerOfferRevoked> OfferRevoked { get; set; } = new List<RouterWorkerOfferRevoked> { };
        public List<RouterJobExceptionTriggered> ExceptionEvents { get; set; } = new List<RouterJobExceptionTriggered> { };
    }
    public enum State
    {
        ClassificationFailed,
        Queued,
        Created,
        Assigned,
        Completed,
        Closed,
        Cancelled
    }

    public enum AssignmentEventType
    {
        Assaign,
        Unassign
    }

    public enum OfferEventType
    {
        OfferIssued,
        OfferRevoked,
        OfferExpired,
        OfferAccepted,
        OfferDeclined
    }
    public enum ExceptionEventType
    {
        Action,
        OfferRevoked,
        OfferExpired,
        OfferAccepted,
        OfferDeclined
    }

    public class JobQueuedEvents
    {
        public string id { get; set; }
        public string Channel { get; set; }
        public string Priority { get; set; }
        public string ClassificationPolicy { get; set; }
        public Dictionary<string, int> RequiredAbilities { get; set; }
        public Dictionary<string, LabelValue> JobMetaData { get; set; }
        public DateTimeOffset QueuedTime { get; set; }
        public DateTimeOffset DeQueuedTime { get; set; }
    }

    public class JobAssignmentEvents
    {
        public AssignmentEventType Type { get; set; }
        public string WorkerId { get; set; }
        public DateTimeOffset AssignmentTime { get; set; }
    }

    public class JobOfferEvents
    {
        public OfferEventType Type { get; set; }
        public string WorkerId { get; set; }
        public DateTimeOffset OfferTime { get; set; }
    }
}

    public class JobExceptionEvents
    {
    public ExceptionEventType EventType { get; set; }
    public string TriggerOrActionName { get; set; }
    public string WorkerId { get; set; }
    public DateTimeOffset OfferTime { get; set; }
}