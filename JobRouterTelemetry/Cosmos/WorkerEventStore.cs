using Azure.Communication.JobRouter.Models;
using JobRouterTelemetry.Interfaces;
using JobRouterTelemetry.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JobRouterTelemetry.Cosmos
{
    public class WorkerEventStore : CosmosDbEventStore<EventEnvelope>, IWorkerEventStore
    {
        protected override string ContainerId => "WorkersEventStore";

        public WorkerEventStore(Database db, ILogger<WorkerEventStore> logger)
            : base(db, logger)
        {
        }

        public Task<QueryPage<EventEnvelope>> List(int? maxItemCount = 10, string continuationToken = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<QueryPage<EventEnvelope>> ListFromUser(string userId, int? maxItemCount = 10, string continuationToken = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}