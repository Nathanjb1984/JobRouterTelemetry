using JasonShave.Azure.Communication.Service.EventHandler.JobRouter;
using JasonShave.Azure.Communication.Service.JobRouter.Sdk.Contracts.V2021_10_20_preview.Events;
using JobRouterTelemetry.Interfaces;
using JobRouterTelemetry.Models;
using Microsoft.Azure.Cosmos;
using static JobRouterTelemetry.Models.WorkerStatistics;

namespace JobRouterTelemetry
{
    public class EventService : BackgroundService
    {
        private IJobStatisticsRepository _jobStatisticsRepository;
        private IWorkerStatisticsRepository _workerStatisticsRepository;
        private IWorkerEventStore _workerEventStore;
        public EventService(IJobRouterEventSubscriber jobRouterEventSubscriber, IJobStatisticsRepository jobStatisticsRepository, IWorkerStatisticsRepository workerStatisticsRepository, IWorkerEventStore workerEventStore)
        {
            jobRouterEventSubscriber.OnJobReceived += HandleJobReceived;
            jobRouterEventSubscriber.OnJobQueued += HandleJobQueued;
            jobRouterEventSubscriber.OnWorkerOfferIssued += HandleWorkerOfferIssued;
            jobRouterEventSubscriber.OnWorkerOfferRevoked += HandleWorkerOfferRevoked;
            jobRouterEventSubscriber.OnWorkerOfferExpired += HandleWorkerOFferExpired;
            jobRouterEventSubscriber.OnWorkerOfferAccepted += HandleWorkerOfferAccepted;
            jobRouterEventSubscriber.OnWorkerOfferDeclined += HandleWorkerOfferDeclined;
            jobRouterEventSubscriber.OnJobCompleted += HandleJobCompleted;
            jobRouterEventSubscriber.OnJobClosed += HandleJobClosed;
            jobRouterEventSubscriber.OnJobCancelled += HandleJobCancelled;
            jobRouterEventSubscriber.OnJobClassified += HandleJobClassified;
            jobRouterEventSubscriber.OnJobClassificationFailed += HandleJobClassificationFailed;
            jobRouterEventSubscriber.OnJobExceptionTriggered += HandleJobExceptionTriggered;
            jobRouterEventSubscriber.OnJobWorkerSelectorsExpired += HandleJobWorkerSelectorsExpired;
            jobRouterEventSubscriber.OnWorkerRegistered += HandleWorkerRegistered;
            jobRouterEventSubscriber.OnWorkerDeregistered += HandleWorkerDeregistered;

            _jobStatisticsRepository = jobStatisticsRepository;
            _workerStatisticsRepository = workerStatisticsRepository;
            _workerEventStore = workerEventStore;
        }

        private async ValueTask HandleWorkerDeregistered(RouterWorkerDeregistered arg1, string? arg2)
        {
            try
            {
                var envelope = new List<EventEnvelope> { new EventEnvelope { EntityId = arg1.WorkerId, EventTime = DateTimeOffset.UtcNow, EventType = arg1.GetType().ToString(), EventData = new RouterWorkerDeregistered { WorkerId = arg1.WorkerId } } };
                await _workerEventStore.ApplyNewEventsAsync(arg1.WorkerId, envelope);

                var workerEntity = await _workerStatisticsRepository.Get(arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
                workerEntity.State = WorkerState.DeRegistered;
                workerEntity.LastUpdatedTime = DateTimeOffset.UtcNow;
                await _workerStatisticsRepository.Replace(workerEntity, arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }

        private async ValueTask HandleWorkerRegistered(RouterWorkerRegistered arg1, string? arg2)
        {
            try
            {
                var envelope = new List<EventEnvelope> { new EventEnvelope { EntityId = arg1.WorkerId, EventTime = DateTimeOffset.UtcNow, EventType = arg1.GetType().ToString(),
                    EventData = new RouterWorkerRegistered { WorkerId = arg1.WorkerId, ChannelConfigurations = arg1.ChannelConfigurations, Labels = arg1.Labels, QueueAssignments = arg1.QueueAssignments, TotalCapacity = arg1.TotalCapacity } }};
                await _workerEventStore.ApplyNewEventsAsync(arg1.WorkerId, envelope);

                var workerEntity = await _workerStatisticsRepository.Get(arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
                workerEntity.State = WorkerState.Registered;
                workerEntity.RegisteredQueues = arg1.QueueAssignments;
                workerEntity.RegisteredChannels = arg1.ChannelConfigurations;
                workerEntity.WorkerMetaData = arg1.Labels;
                workerEntity.LastUpdatedTime = DateTimeOffset.UtcNow;
                await _workerStatisticsRepository.Replace(workerEntity, arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var workerEntity = await _workerStatisticsRepository.Create(
                        new WorkerStatistics
                        {
                            id = arg1.WorkerId,
                            CreationTime = DateTimeOffset.UtcNow,
                            LastUpdatedTime = DateTimeOffset.UtcNow,
                            State = WorkerState.Registered,
                            RegisteredQueues = arg1.QueueAssignments,
                            RegisteredChannels = arg1.ChannelConfigurations,
                            WorkerMetaData = arg1.Labels
                        }, arg1.WorkerId).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private ValueTask HandleJobWorkerSelectorsExpired(RouterJobWorkerSelectorsExpired arg1, string? arg2)
        {
            throw new NotImplementedException();
        }

        private ValueTask HandleJobExceptionTriggered(RouterJobExceptionTriggered arg1, string? arg2)
        {
            throw new NotImplementedException();
        }

        private async ValueTask HandleJobClassificationFailed(RouterJobClassificationFailed arg1, string? arg2)
        {
            try
            {
                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.State = State.ClassificationFailed;
                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleJobClassified(RouterJobClassified arg1, string? arg2)
        {
            try
            {
                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.ClassifiedEvents.Add(arg1);
                jobEntity.ClassificationPolicyId = arg1.ClassificationPolicyId;
                if (arg1.QueueId != null)
                    jobEntity.QueueId = arg1.QueueId;
                if (arg1.Priority != null)
                    jobEntity.Priority = arg1.Priority;
                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleJobCancelled(RouterJobCancelled arg1, string? arg2)
        {
            try
            {
                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.State = State.Cancelled;
                jobEntity.JobCancelledTime = DateTimeOffset.UtcNow;
                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleJobClosed(RouterJobClosed arg1, string? arg2)
        {
            try
            {
                var envelope = new List<EventEnvelope> { new EventEnvelope { EntityId = arg1.WorkerId, EventTime = DateTimeOffset.UtcNow, EventType = arg1.GetType().ToString(),
                    EventData = new RouterJobClosed { WorkerId = arg1.WorkerId, AssignmentId = arg1.AssignmentId, Labels = arg1.Labels, ChannelId = arg1.ChannelId, ChannelReference = arg1.ChannelReference, JobId = arg1.JobId, QueueId = arg1.QueueId, Tags = arg1.Tags, DispositionCode = arg1.DispositionCode } }};
                await _workerEventStore.ApplyNewEventsAsync(arg1.WorkerId, envelope);

                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.State = State.Closed;
                jobEntity.JobClosedTime = DateTimeOffset.UtcNow;
                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleJobCompleted(RouterJobCompleted arg1, string? arg2)
        {
            try
            {
                var envelope = new List<EventEnvelope> { new EventEnvelope { EntityId = arg1.WorkerId, EventTime = DateTimeOffset.UtcNow, EventType = arg1.GetType().ToString(),
                    EventData = new RouterJobCompleted { WorkerId = arg1.WorkerId, AssignmentId = arg1.AssignmentId, Labels = arg1.Labels, ChannelId = arg1.ChannelId, ChannelReference = arg1.ChannelReference, JobId = arg1.JobId, QueueId = arg1.QueueId, Tags = arg1.Tags } } };

                var workerEntity = await _workerStatisticsRepository.Get(arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
                workerEntity.JobsCompleted.Add(arg1);
                await _workerStatisticsRepository.Replace(workerEntity, arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);

                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.State = State.Closed;
                jobEntity.JobCompletedTime = DateTimeOffset.UtcNow;
                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleWorkerOfferDeclined(RouterWorkerOfferDeclined arg1, string? arg2)
        {
            try
            {
                var envelope = new List<EventEnvelope> { new EventEnvelope { EntityId = arg1.WorkerId, EventTime = DateTimeOffset.UtcNow, EventType = arg1.GetType().ToString(),
                    EventData = new RouterWorkerOfferDeclined { WorkerId = arg1.WorkerId, OfferId = arg1.OfferId, ChannelId = arg1.ChannelId, ChannelReference = arg1.ChannelReference, JobId = arg1.JobId, QueueId = arg1.QueueId } } };

                var workerEntity = await _workerStatisticsRepository.Get(arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
                workerEntity.OfferDeclined.Add(arg1);
                await _workerStatisticsRepository.Replace(workerEntity, arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);

                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.OfferDeclined.Add(arg1);
                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleWorkerOfferAccepted(RouterWorkerOfferAccepted arg1, string? arg2)
        {
            try
            {
                var envelope = new List<EventEnvelope> { new EventEnvelope { EntityId = arg1.WorkerId, EventTime = DateTimeOffset.UtcNow, EventType = arg1.GetType().ToString(),
                    EventData = new RouterWorkerOfferAccepted { WorkerId = arg1.WorkerId, OfferId = arg1.OfferId, ChannelId = arg1.ChannelId, ChannelReference = arg1.ChannelReference, JobId = arg1.JobId, QueueId = arg1.QueueId, AssignmentId = arg1.AssignmentId, JobLabels = arg1.JobLabels, JobPriority = arg1.JobPriority, JobTags = arg1.JobTags } } };

                var workerEntity = await _workerStatisticsRepository.Get(arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
                workerEntity.OfferAccepted.Add(arg1);
                workerEntity.AssignedJob = arg1.JobId;
                await _workerStatisticsRepository.Replace(workerEntity, arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);

                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.OfferAccepted.Add(arg1);
                if (jobEntity.InitialAsignmentTime == null) 
                    jobEntity.InitialAsignmentTime = DateTimeOffset.UtcNow;
                jobEntity.CurrentAssignedWorker = arg1.WorkerId;
                jobEntity.State = JobRouterTelemetry.Models.State.Assigned;
                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleWorkerOFferExpired(RouterWorkerOfferExpired arg1, string? arg2)
        {
            try
            {
                var envelope = new List<EventEnvelope> { new EventEnvelope { EntityId = arg1.WorkerId, EventTime = DateTimeOffset.UtcNow, EventType = arg1.GetType().ToString(),
                    EventData = new RouterWorkerOfferExpired { WorkerId = arg1.WorkerId, OfferId = arg1.OfferId, ChannelId = arg1.ChannelId, ChannelReference = arg1.ChannelReference, JobId = arg1.JobId, QueueId = arg1.QueueId } } };
                var workerEntity = await _workerStatisticsRepository.Get(arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
                workerEntity.OfferExpired.Add(arg1);
                await _workerStatisticsRepository.Replace(workerEntity, arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);

                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.OfferExpired.Add(arg1);
                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleWorkerOfferRevoked(RouterWorkerOfferRevoked arg1, string? arg2)
        {
            try
            {
                var envelope = new List<EventEnvelope> { new EventEnvelope { EntityId = arg1.WorkerId, EventTime = DateTimeOffset.UtcNow, EventType = arg1.GetType().ToString(),
                    EventData = new RouterWorkerOfferRevoked { WorkerId = arg1.WorkerId, OfferId = arg1.OfferId, ChannelId = arg1.ChannelId, ChannelReference = arg1.ChannelReference, JobId = arg1.JobId, QueueId = arg1.QueueId } } };

                var workerEntity = await _workerStatisticsRepository.Get(arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
                workerEntity.OfferRevoked.Add(arg1);
                await _workerStatisticsRepository.Replace(workerEntity, arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
                
                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.OfferRevoked.Add(arg1);
                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleWorkerOfferIssued(RouterWorkerOfferIssued arg1, string? arg2)
        {
            try
            {
                var envelope = new List<EventEnvelope> { new EventEnvelope { EntityId = arg1.WorkerId, EventTime = DateTimeOffset.UtcNow, EventType = arg1.GetType().ToString(),
                    EventData = new RouterWorkerOfferIssued { WorkerId = arg1.WorkerId, OfferId = arg1.OfferId, ChannelId = arg1.ChannelId, ChannelReference = arg1.ChannelReference, JobId = arg1.JobId, QueueId = arg1.QueueId, ExpiryTimeUtc = arg1.ExpiryTimeUtc, JobTags = arg1.JobTags, JobPriority = arg1.JobPriority, JobLabels = arg1.JobLabels, OfferTimeUtc = arg1.OfferTimeUtc } } };

                var workerEntity = await _workerStatisticsRepository.Get(arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);
                workerEntity.OfferIssued.Add(arg1);
                await _workerStatisticsRepository.Replace(workerEntity, arg1.WorkerId, arg1.WorkerId).ConfigureAwait(false);

                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.OfferIssued.Add(arg1);
                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleJobQueued(RouterJobQueued arg1, string? arg2)
        {
            try
            {
                var jobEntity = await _jobStatisticsRepository.Get(arg1.JobId, arg1.JobId).ConfigureAwait(false);
                jobEntity.QueuedEvents.Add(arg1);
                jobEntity.QueueId = arg1.QueueId;
                jobEntity.Priority = arg1.Priority;
                jobEntity.State = JobRouterTelemetry.Models.State.Queued;
                jobEntity.InitialQueueTime = DateTimeOffset.UtcNow;

                await _jobStatisticsRepository.Replace(jobEntity, arg1.JobId, arg1.JobId).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask HandleJobReceived(RouterJobReceived arg1, string? arg2)
        {
            try
            {
                var jobEntity = await _jobStatisticsRepository.Create(new JobStatistics
                {
                    State = State.Created,
                    ChannelReference = arg1.ChannelReference,
                    id = arg1.JobId,
                    CreatedTime = @DateTimeOffset.UtcNow,
                    Channel = arg1.ChannelId,
                    QueueId = arg1.QueueId,
                    ClassificationPolicyId = arg1.ClassificationPolicyId,
                    Priority = arg1.Priority,
                    JobMetaData = arg1.Labels.Count > 0 ? arg1.Labels : new Dictionary<string, object>()
                }, arg1.JobId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
