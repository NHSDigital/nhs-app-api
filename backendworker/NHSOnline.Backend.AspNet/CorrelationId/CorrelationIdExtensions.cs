using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NHSOnline.Backend.AspNet.Middleware;

namespace NHSOnline.Backend.AspNet.CorrelationId
{
    public static class CorrelationIdExtensions
    {
        private const string CorrelationIdentifier = "NHSO-Request-ID";

        public static void UseNhsAppCorrelationId(this IApplicationBuilder app)
        {
            app.UseCorrelationId();

            app.UseLogRequestHeader(new LogRequestHeaderOptions
            {
                HeaderName = CorrelationIdentifier,
                LogTemplate = "CorrelationId={value}"
            });
        }

        public static void AddNhsAppCorrelationId(this IServiceCollection services)
        {
            services.AddDefaultCorrelationId(options =>
            {
                options.AddToLoggingScope = false;
                options.IgnoreRequestHeader = false;
                options.IncludeInResponse = false;
                options.RequestHeader = CorrelationIdentifier;
                options.UpdateTraceIdentifier = false;
            });
        }

        internal static void PrepareCorrelationIdContext(this HealthCheckContext context)
        {
            // There is an issue in the CorrelationId package that causes a NullReferenceException to be thrown
            // when HTTP requests are made outside of the context of an ASP.NET request. Creating our own context
            // allows us to support this behaviour until the issue is resolved.
            // https://github.com/stevejgordon/CorrelationId/issues/82

            var correlationId = $"HealthCheck-{context.Registration.Name}";
            var _ = new CorrelationContextAccessor
            {
                CorrelationContext = new CorrelationContext(correlationId, CorrelationIdentifier)
            };
        }
    }
}