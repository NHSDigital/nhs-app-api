using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAzureNotificationHubClientWrapper, AzureNotificationHubClientWrapper>();
            services.AddTransient<IAzureNotificationHubClient, AzureNotificationHubClient>();
            services.AddSingleton<IDeviceIdGenerator, DeviceIdGenerator>();
            services.AddTransient<IInstallationTemplateFactory, InstallationTemplateFactory>();
            services.AddTransient<IInstallationFactory, InstallationFactory>();
            services.AddTransient<INotificationRegistrationService, AzureNotificationHubRegistrationService>();
            services.AddTransient<IMigrationService, MigrationService>();

            base.ConfigureServices(services, configuration);
        }
    }
}