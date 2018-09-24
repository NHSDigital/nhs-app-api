using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Worker
{
    public class ResponseHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.GetTypedHeaders().CacheControl = 
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        NoCache = true,
                        NoStore = true,
                        NoTransform = true,
                        Private = true
                    };

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
    
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseHeadersMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseHeaderMiddleware>();
        }
    } 
}