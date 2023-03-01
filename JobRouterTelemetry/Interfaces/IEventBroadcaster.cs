// © Microsoft Corporation. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace JobRouterTelemetry.Interfaces
{
    public interface IEventBroadcaster
    {
        Task<bool> SendEvent(object? eventData, CancellationTokenSource? cts = default);
    }
}
