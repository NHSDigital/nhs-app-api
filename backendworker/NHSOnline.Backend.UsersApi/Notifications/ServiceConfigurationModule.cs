using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.UsersApi.Notifications.Migration;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<INotificationClient, NotificationClient>();
            services.AddSingleton<IAzureNotificationHubWrapperService, AzureNotificationHubWrapperService>();
            services.AddSingleton<IDeviceIdGenerator, DeviceIdGenerator>();
            services.AddTransient<IInstallationTemplateFactory, InstallationTemplateFactory>();
            services.AddTransient<IInstallationFactory, InstallationFactory>();
            services.AddTransient<INotificationRegistrationService, NotificationRegistrationService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IMigrationService, MigrationService>();

            base.ConfigureServices(services, configuration);
        }
    }
}