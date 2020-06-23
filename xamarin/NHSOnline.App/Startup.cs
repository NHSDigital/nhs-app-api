using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas;

namespace NHSOnline.App
{
    internal static class Startup
    {
        internal static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAreas();
        }
    }
}
