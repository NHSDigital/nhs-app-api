using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public static class HealthCheckOptionsBuilder
    {
        /// <summary>
        /// These options do not call any custom NHS App
        /// checks, they simply return a 204 response to
        /// let the caller know that this ASP.NET app can 
        /// respond to HTTP requests.
        /// </summary>
        public static HealthCheckOptions BuildLivenessCheckOptions()
        {
            return new HealthCheckOptions
            {
                Predicate = (_) => false
            };
        }

        /// <summary>
        /// These options call all registered custom NHS App
        /// checks, and a 204 response is only returned if they
        /// all return a Healthy status.
        /// </summary>
        public static HealthCheckOptions BuildReadinessCheckOptions()
        {
            return new HealthCheckOptions
            {
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
}
