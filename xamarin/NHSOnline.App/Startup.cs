using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas;
using NHSOnline.App.Services;

namespace NHSOnline.App
{
    internal static class Startup
    {
        internal static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddServices()
                .AddAreas();
        }
    }
}
