﻿using JobRouterTelemetry.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JobRouterTelemetry.Cosmos
{
    public class JobStatisticsDbProvisioner : CosmosDbProvisioner
    {
        public JobStatisticsDbProvisioner(DBOptions config, IEnumerable<IProvisionbleRepository> provisionableRepositories, ILogger<JobStatisticsDbProvisioner> logger)
            : base(config, provisionableRepositories, logger)
        {
        }
    }
}
