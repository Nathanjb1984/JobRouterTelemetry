using JobRouterTelemetry.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JobRouterTelemetry.Cosmos
{
    public class WorkerEventStoreDbProvisioner : CosmosDbProvisioner
    {
        public WorkerEventStoreDbProvisioner(DBOptions config, IEnumerable<IProvisionbleRepository> provisionableRepositories, ILogger<WorkerEventStoreDbProvisioner> logger)
            : base(config, provisionableRepositories, logger)
        {
        }
    }
}
