using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;

namespace NHSOnline.App
{
    internal static class Startup
    {
        internal static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddConfiguration()
                .AddServices()
                .AddDependencyServices()
                .AddNhsLoginServices()
                .AddAreas();
        }
    }
}
