using JobRouterTelemetry.Models;

namespace JobRouterTelemetry.Interfaces
{
    public interface IWorkerStatisticsRepository : ICosmosDbCrudRepository<WorkerStatistics>
    {
    }
}
