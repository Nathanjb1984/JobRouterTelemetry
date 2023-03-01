using JobRouterTelemetry.Cosmos;
using JobRouterTelemetry.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace JobRouterTelemetry.Interfaces
{
    public class JobStatisticsRepository : CosmosDbCrudRepository<JobStatistics>, IJobStatisticsRepository
    {
        protected override string ContainerId => "JobStatistics";
        public JobStatisticsRepository(Database db, ILogger<JobStatisticsRepository> logger)
            : base(db, logger)
        {
        }
    }
}
