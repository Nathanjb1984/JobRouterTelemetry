using System;

namespace JobRouterTelemetry.Interfaces
{
    public interface IDBOptions
    {
        string ConnectionString { get; set; }
        string DbId { get; set; }
        TimeSpan ProvisionTimeOut { get; set; }
    }
}