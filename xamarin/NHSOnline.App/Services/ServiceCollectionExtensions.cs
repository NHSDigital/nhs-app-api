using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Services.Media;

namespace NHSOnline.App.Services
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IBrowserOverlay, BrowserOverlay>()
                .AddTransient<ISelectMediaService, SelectMediaService>()
                .AddTransient<IUserPreferencesService, UserPreferencesService>();
        }
    }
}