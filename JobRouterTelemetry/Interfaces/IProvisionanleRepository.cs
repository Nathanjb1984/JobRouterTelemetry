using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobRouterTelemetry.Interfaces
{
    public interface IProvisionanleRepository
    {
        Task ProvisionContainer(CancellationToken cancellationToken = default);
    }
}
