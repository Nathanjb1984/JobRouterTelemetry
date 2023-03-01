using JobRouterTelemetry.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;

namespace JobRouterTelemetry.Interfaces
{
    public interface ICosmosDbCrudRepository<TDomainModel> : IProvisionanleRepository
    {
        Task<TDomainModel> Create(TDomainModel model, string partitionKey, CancellationToken cancellationToken = default);
        Task Delete(string userId, string id, CancellationToken cancellationToken = default);
        Task<TDomainModel> Get(string id, string partitionKey, CancellationToken cancellationToken = default);
        //Task<QueryPage<WorkFlow>> GetMyOffers(string userId, string partitionKey, string continuationToken = null, CancellationToken cancellationToken = default);
        Task<QueryPage<TDomainModel>> List(int? maxItemCount = 10, string continuationToken = null, CancellationToken cancellationToken = default);
        Task<QueryPage<TDomainModel>> ListFromUser(string userId, int? maxItemCount = 10, string continuationToken = null, CancellationToken cancellationToken = default);
        Task ProvisionContainer(CancellationToken cancellationToken = default);
        Task<TDomainModel> Replace(TDomainModel model, string id, string partitionKey, CancellationToken cancellationToken = default);
    }
}