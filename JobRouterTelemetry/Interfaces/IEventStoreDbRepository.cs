using JobRouterTelemetry.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;

namespace JobRouterTelemetry.Interfaces
{
    public interface IEventStoreDbRepository<TDomainModel> : IProvisionbleRepository
    {
        Task ApplyNewEventsAsync(string entityId, IEnumerable<EventEnvelope> events);
        Task<IEnumerable<EventEnvelope>> RetrieveEventsAsync(string entityId);
        Task<TDomainModel> Create(TDomainModel model, string partitionKey, CancellationToken cancellationToken = default);
        Task Delete(string userId, string id, CancellationToken cancellationToken = default);
        Task<TDomainModel> Get(string id, string partitionKey, CancellationToken cancellationToken = default);
        Task ProvisionContainer(CancellationToken cancellationToken = default);
        Task<TDomainModel> Replace(TDomainModel model, string id, string partitionKey, CancellationToken cancellationToken = default);
    }
}
