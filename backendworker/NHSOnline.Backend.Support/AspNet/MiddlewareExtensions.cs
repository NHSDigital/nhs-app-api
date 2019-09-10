using Microsoft.AspNetCore.Builder;

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
    }
}
