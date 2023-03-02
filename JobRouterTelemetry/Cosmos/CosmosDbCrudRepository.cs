using JobRouterTelemetry.Interfaces;
using JobRouterTelemetry.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using System.Text;

namespace JobRouterTelemetry.Cosmos
{
    public abstract class CosmosDbCrudRepository<TDomainModel> : ICosmosDbCrudRepository<TDomainModel>
    {
        protected Database _db;
        protected Lazy<Container> _container;
        protected ILogger _logger;

        protected Container Container => _container.Value;

        protected abstract string ContainerId { get; }

        protected CosmosDbCrudRepository(Database db, ILogger logger)
        {
            _db = db;
            _container = new Lazy<Container>(() => _db.GetContainer(ContainerId));
            _logger = logger;
        }

        public virtual async Task<TDomainModel> Create(TDomainModel model, string paritionKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await Container.CreateItemAsync(model, new PartitionKey(paritionKey), cancellationToken: cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while creating container.");
                throw;
            }
        }

        public virtual async Task<TDomainModel> Replace(TDomainModel model, string id, string paritionKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await Container.ReplaceItemAsync(model, id, new PartitionKey(paritionKey), cancellationToken: cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while creating container.");
                throw;
            }
        }

        public virtual async Task<TDomainModel> Get(string id, string paritionKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await Container.ReadItemAsync<TDomainModel>(id, new PartitionKey(paritionKey), cancellationToken: cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while creating container.");
                throw;
            }
        }

        public virtual async Task Delete(string paritionKey, string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new ItemRequestOptions();

                _ = await Container.DeleteItemAsync<TDomainModel>(id, new PartitionKey(paritionKey), options, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while creating container.");
                throw;
            }
        }

        public virtual async Task<QueryPage<TDomainModel>> List(int? maxItemCount = 10, string continuationToken = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = Container.GetItemQueryIterator<TDomainModel>(new QueryDefinition("SELECT * FROM c"), DecodeToken(continuationToken), new QueryRequestOptions()
                {
                    MaxItemCount = maxItemCount
                });

                var page = await result.ReadNextAsync(cancellationToken);

                return new QueryPage<TDomainModel>(page.ToList(), EncodeToken(page.ContinuationToken));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while creating container.");
                throw;
            }

        }

        //public virtual async Task<QueryPage<WorkFlow>> GetMyOffers(string userId, string partitionKey, string continuationToken = null, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        var result = Container.GetItemQueryIterator<WorkFlow>(new QueryDefinition($"SELECT * FROM c WHERE c.ContractOfferIssuedByWorkerIds. = '{userId}'"), DecodeToken(continuationToken), new QueryRequestOptions()
        //        {
        //            PartitionKey = new PartitionKey(partitionKey)
        //        });

        //        var page = await result.ReadNextAsync(cancellationToken);

        //        return new QueryPage<WorkFlow>(page.ToList(), EncodeToken(page.ContinuationToken));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ex, "Error while creating container.");
        //        throw;
        //    }
        //}

        public virtual async Task<QueryPage<TDomainModel>> ListFromUser(string userId, int? maxItemCount = 10, string continuationToken = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = Container.GetItemQueryIterator<TDomainModel>(new QueryDefinition($"SELECT * FROM c WHERE c.user.id = '{userId}' ORDER BY c.created_at"), DecodeToken(continuationToken), new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(userId),
                    MaxItemCount = maxItemCount
                });

                var page = await result.ReadNextAsync(cancellationToken);

                return new QueryPage<TDomainModel>(page.ToList(), EncodeToken(page.ContinuationToken));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while creating container.");
                throw;
            }

        }

        public virtual async Task ProvisionContainer(CancellationToken cancellationToken = default)
        {
            var properties = BuildContainerProperties();
            await _db.CreateContainerIfNotExistsAsync(properties, cancellationToken: cancellationToken);
            foreach (var udf in UserDefinedFuctions)
            {
                await ProvisionUdf(udf);
            }
        }

        protected virtual ContainerProperties BuildContainerProperties()
        {
            if (ContainerId == "QueueStatistics")
            {
                return new(ContainerId, @"/id");
            }
            else if (ContainerId == "JobStatistics")
            {
                return new(ContainerId, @"/id");
            }
            else if (ContainerId == "WorkerStatistics")
            {
                return new(ContainerId, @"/id");
            }
            else
            {
                return null;
            }
        }

        protected virtual IEnumerable<UserDefinedFunctionProperties> UserDefinedFuctions => new List<UserDefinedFunctionProperties>();

        protected async Task ProvisionUdf(UserDefinedFunctionProperties udf)
        {
            try
            {
                var existing = await Container.Scripts.ReadUserDefinedFunctionAsync(udf.Id);
                if (existing.Resource.Body != udf.Body)
                    await Container.Scripts.ReplaceUserDefinedFunctionAsync(udf);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //protected virtual Exception TranslateCosmosException(CosmosException exception) => exception.StatusCode switch
        //{
        //    HttpStatusCode.NotFound => new NotFoundException("The item was not found.", exception),
        //    HttpStatusCode.Conflict => new ConflictException("The current state of the item conflicts with the operation.", exception),
        //    HttpStatusCode.PreconditionFailed => new PreconditionFailed("The item was in an unexpectant state.", exception),
        //    _ => exception,
        //};

        protected string EncodeToken(string? token)
        {
            return token == null ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
        }

        protected string DecodeToken(string? token)
        {
            return token == null ? null : Encoding.UTF8.GetString(Convert.FromBase64String(token));
        }
    }
}
