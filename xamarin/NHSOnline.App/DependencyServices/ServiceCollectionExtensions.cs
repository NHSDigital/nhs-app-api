using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Api;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Logging;
using Xamarin.Forms;

namespace NHSOnline.App.DependencyServices
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDependencyServices(this IServiceCollection services)
        {
            return services
                .AddTransient(_ => DependencyService.Get<IPrimaryHttpMessageHandlerFactory>())
                .AddTransient(_ => DependencyService.Get<IBiometrics>())
                .AddTransient(_ => DependencyService.Get<ICookieService>())
                .AddTransient(_ => DependencyService.Get<ILifecycle>())
                .AddTransient(_ => DependencyService.Get<INativeLog>())
                .AddTransient(_ => DependencyService.Get<INotifications>())
                .AddTransient(_ => DependencyService.Get<IPreHomeLogoutMonitor>())
                .AddTransient(_ => DependencyService.Get<ISettingsService>())
                .AddTransient(_ => DependencyService.Get<INativeAppVersionCheckService>())
                .AddTransient(_ => DependencyService.Get<IFileHandler>())
                .AddTransient(_ => DependencyService.Get<IUpdateService>())
                .AddTransient(_ => DependencyService.Get<ICalendar>())
                .AddTransient(_ => DependencyService.Get<IDialogPresenter>())
                .AddTransient(_ => DependencyService.Get<IInstallReferrer>())
                .AddTransient(_ => DependencyService.Get<IAccessibilityService>())
                .AddTransient(_ => DependencyService.Get<IPlatformVersion>())
                .AddTransient(_ => DependencyService.Get<IBadgeService>());
        }
    }
}