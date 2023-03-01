// © Microsoft Corporation. All rights reserved.

using System.Threading.Tasks;

namespace JobRouterTelemetry.Interfaces
{
    public interface IEventGridEventHandler<TEvent>
    {
        Task Handle(TEvent @event);
    }
}
