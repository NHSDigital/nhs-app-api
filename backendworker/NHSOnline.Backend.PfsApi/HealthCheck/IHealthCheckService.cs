using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.HealthCheck
{
    public interface IHealthCheckService
    {
        Task<HealthCheckResponse> RunHealthChecks();
    }
}