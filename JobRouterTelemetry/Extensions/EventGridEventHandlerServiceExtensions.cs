//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using JobRouterTelemetry.Events;
//using JobRouterTelemetry.Interfaces;

//namespace JobRouterTelemetry.Extensions
//{
//    public static class EventGridEventHandlerServiceExtensions
//    {
//    public static IServiceCollection AddAllEventGridEventHandlers(this IServiceCollection services)
//        {
//    services.AddSingleton<IEventGridEventHandler<OfferIssuedEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<OfferRevokedEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<OfferAcceptedEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<OfferDeclinedEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<OfferExpiredEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<JobReceivedEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<JobClassifiedEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<JobClassificationFailedEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<JobCancelledEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<JobClosedEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<JobCompletedEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<JobExceptionTriggerd>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<JobQueuedEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<WorkerRegisteredEvent>, DistributionEventHandler>();
//    services.AddSingleton<IEventGridEventHandler<WorkerDeregisteredEvent>, DistributionEventHandler>();
//    services.AddSingleton<EventConverter>();

//    return services;

//}
//    }
//}
