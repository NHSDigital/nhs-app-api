using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks(
                NhsAppServiceHealthCheck.LivenessProbe,
                HealthCheckOptionsBuilder.BuildLivenessCheckOptions());

            endpoints.MapHealthChecks(
                NhsAppServiceHealthCheck.ReadinessPath,
                HealthCheckOptionsBuilder.BuildReadinessCheckOptions());
        }
    }
}
