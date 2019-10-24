using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NHSOnline.Backend.Support.Repository;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Notifications.Azure;
using NHSOnline.Backend.UsersApi.Notifications.Azure.Steps;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRegistrationDescriptionFactory, RegistrationDescriptionFactory>();
            services.AddSingleton<IAzureNotificationHubClient, AzureNotificationHubClient>();
            services.AddSingleton<INotificationService, AzureNotificationHubService>();
            
            services.AddSingleton(typeof(IApiMongoClient<>),typeof(ApiMongoClient<>));
            services.AddSingleton<IUserDeviceRepository, MongoUserDeviceRepository>();
            services.AddSingleton<IDeviceRepositoryService, DeviceRepositoryService>();
            
            services.AddSingleton<IDeviceIdGenerator, DeviceIdGenerator>();
            services.AddTransient<IOperationStepBuilder, OperationStepBuilder>();
            services.AddTransient<RemoveRegistrationsStep>();
            services.AddTransient<CreateRegistrationIdStep>();
            services.AddTransient<CreateOrUpdateRegistrationStep>();

            base.ConfigureServices(services, configuration);
        }
    }
}