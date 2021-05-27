using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    /// <summary>
    /// Options here call NHS App health checks that are
    /// tagged with a certain value, if none are defined then
    /// they simply return a 204 response to let the caller know
    /// that this ASP.NET app can respond to HTTP requests.
    /// </summary>
    public static class HealthCheckOptionsBuilder
    {
        /// <summary>
        /// Only run health checks tagged with 'Liveness'.
        /// </summary>
        public static HealthCheckOptions BuildLivenessCheckOptions() =>
            BuildForTagValue(NhsAppHealthCheckTags.LivenessValue);

        /// <summary>
        /// Only run health checks tagged with 'Readiness'.
        /// </summary>
        public static HealthCheckOptions BuildReadinessCheckOptions() =>
            BuildForTagValue(NhsAppHealthCheckTags.ReadinessValue);

        private static HealthCheckOptions BuildForTagValue(string healthCheckTag) =>
            new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains(healthCheckTag),
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status204NoContent,
                    [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                ResponseWriter = (httpContext, healthReport) => Task.CompletedTask
            };
    }
}
