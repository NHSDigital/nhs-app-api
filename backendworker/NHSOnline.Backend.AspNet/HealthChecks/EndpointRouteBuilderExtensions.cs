using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks(
                NhsAppHealthCheckUrls.LivenessPath,
                HealthCheckOptionsBuilder.BuildLivenessCheckOptions());

            endpoints.MapHealthChecks(
                NhsAppHealthCheckUrls.ReadinessPath,
                HealthCheckOptionsBuilder.BuildReadinessCheckOptions());
        }
    }
}
