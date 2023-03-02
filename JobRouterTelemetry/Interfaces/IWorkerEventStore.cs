using JobRouterTelemetry.Models;
using Microsoft.Azure.Cosmos;

namespace JobRouterTelemetry.Interfaces
{
    public interface IWorkerEventStore : IEventStoreDbRepository<EventEnvelope>
    {
    }
}
