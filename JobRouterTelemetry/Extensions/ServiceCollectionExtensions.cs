using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using JobRouterTelemetry.Cosmos;
using JobRouterTelemetry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobRouterTelemetry.Models;

namespace JobRouterTelemetry.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosPersistence(this IServiceCollection services, Action<DBOptions> cfg)
        {
            var options = new DBOptions();
            cfg(options);
            services.AddSingleton(options);

            var client = new CosmosClient(options.ConnectionString);
            var database = client.GetDatabase(options.DbId);

            services.AddSingleton(client);
            services.AddSingleton(database);
            services.AddTransient<ICosmosDbProvisioner, WorkerStatisticsDbProvisioner>();
            services.AddTransient<ICosmosDbProvisioner, JobStatisticsDbProvisioner>();
            services.AddTransient<ICosmosDbProvisioner, QueueStatisticsDbProvisioner>();

            services.AddSingleton<IWorkerStatisticsRepository, WorkerStatisticsRepository>();
            services.AddSingleton<IJobStatisticsRepository, JobStatisticsRepository>();
            services.AddSingleton<IQueueStatisticsRepository, QueueStatisticsRepository>();

            services.AddTransient<IProvisionanleRepository, WorkerStatisticsRepository>();
            services.AddTransient<IProvisionanleRepository, JobStatisticsRepository>();
            services.AddTransient<IProvisionanleRepository, QueueStatisticsRepository>();

            return services;
        }

        public static IApplicationBuilder UseCosmosDb(this IApplicationBuilder app)
        {
            try
            {
                var provisioner = app.ApplicationServices.GetRequiredService<ICosmosDbProvisioner>();
                provisioner.Provision().Wait();
            }
            catch (Exception)
            {

                throw;
            }

            return app;
        }

    }
}
