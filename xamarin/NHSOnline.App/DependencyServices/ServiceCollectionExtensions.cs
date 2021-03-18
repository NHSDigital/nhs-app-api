using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Api;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.DependencyServices.Paycasso;
using NHSOnline.App.Logging;
using Xamarin.Forms;

namespace NHSOnline.App.DependencyServices
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDependencyServices(this IServiceCollection services)
        {
            return services
                .AddTransient(_ => DependencyService.Get<ICookies>())
                .AddTransient(_ => DependencyService.Get<ILifecycle>())
                .AddTransient(_ => DependencyService.Get<INativeLog>())
                .AddTransient(_ => DependencyService.Get<IPaycasso>())
                .AddTransient(_ => DependencyService.Get<IPrimaryHttpMessageHandlerFactory>())
                .AddTransient(_ => DependencyService.Get<INotifications>());
        }
    }
}