using JobRouterTelemetry.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JobRouterTelemetry.Cosmos
{
    public class WorkerStatisticsDbProvisioner : CosmosDbProvisioner
    {
        public WorkerStatisticsDbProvisioner(DBOptions config, IEnumerable<IProvisionanleRepository> provisionableRepositories, ILogger<WorkerStatisticsDbProvisioner> logger)
            : base(config, provisionableRepositories, logger)
        {
        }
    }
}
