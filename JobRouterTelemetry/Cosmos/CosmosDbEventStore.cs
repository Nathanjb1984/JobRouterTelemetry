using Microsoft.Azure.Cosmos;
using JobRouterTelemetry.Models;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Azure.Cosmos;
using System.Reflection;
using System.Threading;
using JobRouterTelemetry.Interfaces;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;

namespace JobRouterTelemetry.Cosmos
{
    public abstract class CosmosDbEventStore<T> : IEventStoreDbRepository<EventEnvelope>
    {
        protected Database _db;
        protected Lazy<Container> _container;
        protected ILogger _logger;

        protected Container Container => _container.Value;

        protected abstract string ContainerId { get; }

        public CosmosDbEventStore(Database db, ILogger logger)
        {
            _container = new Lazy<Container>(() => _db.GetContainer(ContainerId));
            _db = db;
            _logger = logger;
        }

        public virtual async Task ApplyNewEventsAsync(string entityId, IEnumerable<EventEnvelope> events)
        {
            try
            {
                var tasks = new List<Task>();
                foreach (var @event in events)
                {
                    var result = await _container.Value.CreateItemAsync<EventEnvelope>(@event, new Microsoft.Azure.Cosmos.PartitionKey(entityId));
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }

        public virtual async Task<IEnumerable<EventEnvelope>> RetrieveEventsAsync(string entityId)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.entityId = @entityId")
                .WithParameter("@entityId", entityId);
            var events = new List<EventEnvelope>();
            using (var resultSet = _container.Value.GetItemQueryIterator<EventEnvelope>(query))
            {
                while (resultSet.HasMoreResults)
                {
                    var response = await resultSet.ReadNextAsync();
                    events.AddRange(response.Resource);
                }
            }
            return events;
        }

        public virtual async Task<EventEnvelope> Create(EventEnvelope model, string paritionKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await Container.CreateItemAsync(model, new Microsoft.Azure.Cosmos.PartitionKey(paritionKey), cancellationToken: cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while creating container.");
                throw;
            }
        }

        public virtual async Task<EventEnvelope> Replace(EventEnvelope model, string id, string paritionKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await Container.ReplaceItemAsync(model, id, new Microsoft.Azure.Cosmos.PartitionKey(paritionKey), cancellationToken: cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while creating container.");
                throw;
            }
        }

        public virtual async Task<EventEnvelope> Get(string id, string paritionKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await Container.ReadItemAsync<EventEnvelope>(id, new   Microsoft.Azure.Cosmos.PartitionKey(paritionKey), cancellationToken: cancellationToken);

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

                _ = await Container.DeleteItemAsync<EventEnvelope>(id, new Microsoft.Azure.Cosmos.PartitionKey(paritionKey), options, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while creating container.");
                throw;
            }
        }

        //public virtual async Task<QueryPage<EventEnvelope>> List(int? maxItemCount = 10, string continuationToken = null, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        var result = Container.GetItemQueryIterator<EventEnvelope>(new QueryDefinition("SELECT * FROM c"), DecodeToken(continuationToken), new QueryRequestOptions()
        //        {
        //            MaxItemCount = maxItemCount
        //        });

        //        var page = await result.ReadNextAsync(cancellationToken);

        //        return new QueryPage<TDomainModel>(page.ToList(), EncodeToken(page.ContinuationToken));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ex, "Error while creating container.");
        //        throw;
        //    }

        //}

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
            if (ContainerId == "WorkersEventStore")
            {
                return new(ContainerId, @"/entityId");
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
    }
}