using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Views;

namespace NHSOnline.App
{
    internal static class Startup
    {
        internal static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddViews();
        }
    }
}
