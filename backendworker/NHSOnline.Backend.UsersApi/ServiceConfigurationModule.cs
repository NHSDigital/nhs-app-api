using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Azure;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAzureNotificationHubService, MockAzureNotificationHubService>();
            services.AddSingleton<IUserDeviceRepository, MongoUserDeviceRepository>();
            services.AddSingleton<IDeviceIdGenerator, DeviceIdGenerator>();
            services.AddSingleton<IDeviceRepositoryService, DeviceRepositoryService>();
            services.AddSingleton<IMongoClient, UsersApiMongoClient>();
            base.ConfigureServices(services, configuration);
        }
    }
}