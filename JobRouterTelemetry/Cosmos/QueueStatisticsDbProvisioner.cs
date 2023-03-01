using JobRouterTelemetry.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JobRouterTelemetry.Cosmos
{
    public class QueueStatisticsDbProvisioner : CosmosDbProvisioner
    {
        public QueueStatisticsDbProvisioner(DBOptions config, IEnumerable<IProvisionanleRepository> provisionableRepositories, ILogger<QueueStatisticsDbProvisioner> logger)
            : base(config, provisionableRepositories, logger)
        {
        }
    }
}
