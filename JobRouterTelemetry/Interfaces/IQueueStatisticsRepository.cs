using JobRouterTelemetry.Models;

namespace JobRouterTelemetry.Interfaces
{
    public interface IQueueStatisticsRepository : ICosmosDbCrudRepository<QueueStatistics>
    {
    }
}
