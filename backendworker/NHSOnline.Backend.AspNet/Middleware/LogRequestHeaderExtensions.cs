using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace NHSOnline.Backend.AspNet.Middleware
{
    public static class LogRequestHeaderExtensions
    {
        public static IApplicationBuilder UseLogRequestHeader(this IApplicationBuilder app,
            LogRequestHeaderOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof (app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof (options));
            }

            return app.UseMiddleware<LogRequestHeaderMiddleware>((object) Options.Create(options));
        }
    }
}