using JobRouterTelemetry.Models;

namespace JobRouterTelemetry.Interfaces
{
    public interface IJobStatisticsRepository : ICosmosDbCrudRepository<JobStatistics>
    {
    }
}
