using JobRouterTelemetry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobRouterTelemetry.Cosmos
{
    public class DBOptions : IDBOptions
    {
        public string ConnectionString { get; set; }
        public string DbId { get; set; }
        public TimeSpan ProvisionTimeOut { get; set; } = TimeSpan.FromSeconds(30);
    }
}
