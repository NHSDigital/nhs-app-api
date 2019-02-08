using System.Threading.Tasks;
using NHSOnline.Backend.Worker.HealthCheck.Models;

namespace NHSOnline.Backend.Worker.HealthCheck
{
    public interface IHealthCheckService
    {
        Task<HealthCheckResponse> RunHealthChecks();
    }
}