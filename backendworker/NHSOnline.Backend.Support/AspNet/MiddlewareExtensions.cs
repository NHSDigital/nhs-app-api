using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Support.AspNet
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseHeadersMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseHeaderMiddleware>();
        }

        public static IApplicationBuilder UseSecurityResponseHeadersMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityResponseHeaderMiddleware>();
        }

        public static IApplicationBuilder UseCors(this IApplicationBuilder app, IConfiguration configuration)
        {
            var origins = configuration["CORS_AUTHORITY"];
            return origins switch
            {
                "*" => app.UseCors(
                    builder => builder
                        .SetIsOriginAllowed(_ => true) // Cannot combine * origin with allow credentials
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()),
                { } => app.UseCors(
                    builder => builder
                        .WithOrigins(origins)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()),
                null => app
            };
        }
    }
}
