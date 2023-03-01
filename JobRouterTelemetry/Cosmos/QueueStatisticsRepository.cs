using JobRouterTelemetry.Cosmos;
using JobRouterTelemetry.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace JobRouterTelemetry.Interfaces
{
    public class QueueStatisticsRepository : CosmosDbCrudRepository<QueueStatistics>, IQueueStatisticsRepository
    {
        protected override string ContainerId => "QueueStatistics";
        public QueueStatisticsRepository(Database db, ILogger<QueueStatisticsRepository> logger)
            : base(db, logger)
        {
        }
    }
}
