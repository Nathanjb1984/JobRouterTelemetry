using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using JobRouterTelemetry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobRouterTelemetry.Cosmos
{
    public abstract class CosmosDbProvisioner : ICosmosDbProvisioner
    {
        protected readonly CosmosClient _client;
        protected readonly DBOptions _config;
        protected readonly IEnumerable<IProvisionanleRepository> _provisionableRepositories;
        protected readonly ILogger _logger;

        protected CosmosDbProvisioner(DBOptions config, IEnumerable<IProvisionanleRepository> provisionableRepositories, ILogger logger)
        {
            _client = new CosmosClient(config.ConnectionString);
            _config = config;
            _provisionableRepositories = provisionableRepositories;
            _logger = logger;
        }

        public async Task Provision(CancellationToken cancellationToken = default)
        {
            var timeoutSource = new CancellationTokenSource(_config.ProvisionTimeOut);
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken);
            try
            {
                await ProvisionDb(linkedTokenSource.Token);
                await ProvisionContainer(linkedTokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "DB provisioning failed");
                throw;
            }
        }

        protected virtual async Task ProvisionDb(CancellationToken cancellationToken)
        {
            await _client.CreateDatabaseIfNotExistsAsync(_config.DbId, cancellationToken: cancellationToken);
        }

        protected virtual async Task ProvisionContainer(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Provisioning Containers");
            foreach (var repo in _provisionableRepositories)
            {
                await repo.ProvisionContainer(cancellationToken);
            }
        }

    }
}
