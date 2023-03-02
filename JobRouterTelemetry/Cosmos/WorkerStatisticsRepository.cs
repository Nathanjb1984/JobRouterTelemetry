using Azure.Communication.JobRouter.Models;
using JobRouterTelemetry.Cosmos;
using JobRouterTelemetry.Interfaces;
using JobRouterTelemetry.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace JobRouterTelemetry.Models
{
    public class WorkerStatisticsRepository : CosmosDbCrudRepository<WorkerStatistics>, IWorkerStatisticsRepository
    {
        protected override string ContainerId => "WorkerStatistics";
        public WorkerStatisticsRepository(Database db, ILogger<WorkerStatisticsRepository> logger)
            : base(db, logger)
        {
        }
    }
}
