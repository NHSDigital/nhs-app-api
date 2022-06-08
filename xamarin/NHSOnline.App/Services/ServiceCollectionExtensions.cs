using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Services.FIDO;
using NHSOnline.App.Services.ForcedUpdate;
using NHSOnline.App.Services.Media;

namespace NHSOnline.App.Services
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IForcedUpdateCheckService, ForcedUpdateCheckService>()
                .AddSingleton<INavigationService, NhsAppNavigationService>()
                .AddTransient<NhsAppCookieService>()
                .AddTransient<IBiometricAuthenticationService, BiometricAuthenticationService>()
                .AddTransient<BiometricRegistrationService>()
                .AddTransient<BiometricLoginService>()
                .AddTransient<IBrowser, Browser>()
                .AddTransient<ISelectMediaService, SelectMediaService>()
                .AddTransient<IUserPreferencesService, UserPreferencesService>()
                .AddTransient<RedirectorUrlFactory>()
                .AddTransient<FileDownloadService>();
        }
    }
}