using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NHSOnline.Backend.HealthChecks
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks(
                NhsAppServiceHealthCheck.ReadinessPath,
                new HealthCheckOptions
                {
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status204NoContent,
                        [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                    ResponseWriter = (httpContext, healthReport) => Task.CompletedTask
                });
        }
    }
}
