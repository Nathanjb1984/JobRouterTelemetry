using System.Threading;
using System.Threading.Tasks;

namespace JobRouterTelemetry.Interfaces
{
    public interface ICosmosDbProvisioner
    {
        Task Provision(CancellationToken cancellationToken = default);
    }
}