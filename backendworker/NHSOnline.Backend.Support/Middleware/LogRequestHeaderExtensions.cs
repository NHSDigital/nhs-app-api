using System;
using Microsoft.AspNetCore.Builder;

namespace NHSOnline.Backend.Support.Middleware
{
    public static class LogRequestHeaderExtensions
    {
        public static IApplicationBuilder UseLogRequestHeader(
            this IApplicationBuilder app, string headerName)
        {
            if (app == null)
                throw new ArgumentNullException(nameof (app));
            return app.UseLogRequestHeader(new LogRequestHeaderOptions{HeaderName = headerName});
        }
        
        public static IApplicationBuilder UseLogRequestHeader(this IApplicationBuilder app,
            LogRequestHeaderOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof (app));
            if (options == null)
                throw new ArgumentNullException(nameof (options));

            return app.UseMiddleware<LogRequestHeaderMiddleware>((object) Microsoft.Extensions.Options.Options.Create(options));
        }
    }
}