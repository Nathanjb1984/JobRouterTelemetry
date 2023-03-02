namespace JobRouterTelemetry.Interfaces
{
    public interface IProvisionbleRepository
    {
        Task ProvisionContainer(CancellationToken cancellationToken = default);
    }
}
