using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.HealthCheck.Models;

namespace NHSOnline.Backend.PfsApi.HealthCheck
{
    public interface IHealthCheckService
    {
        Task<HealthCheckResponse> RunHealthChecks();
    }
}