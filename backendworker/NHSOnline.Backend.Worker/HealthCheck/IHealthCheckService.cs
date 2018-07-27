using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.HealthCheck.Models;

namespace NHSOnline.Backend.Worker.HealthCheck
{
    public interface IHealthCheckService
    {
        Task<HealthCheckResponse> RunHealthChecks();
    }
}