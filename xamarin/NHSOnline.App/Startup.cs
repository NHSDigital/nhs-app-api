using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api;
using NHSOnline.App.Areas;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Logging;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App
{
    internal static class Startup
    {
        internal static void ConfigureServices(IServiceCollection services, ILoggerFactory loggerFactory)
        {
            services
                .AddConfiguration()
                .AddServices()
                .AddDependencyServices()
                .AddNhsLoginServices()
                .AddApiServices()
                .AddThreadingServices()
                .AddAreas()
                .AddLoggingServices(loggerFactory);
        }
    }
}
