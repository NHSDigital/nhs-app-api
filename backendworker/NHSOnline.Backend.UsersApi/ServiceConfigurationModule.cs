using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Support.Http;
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
            services.AddTransient<CitizenIdHttpRequestIdentifier>();

            services.AddHttpClient<CitizenIdHttpClient>()
               .AddHttpMessageHandler<HttpTimeoutHandler<CitizenIdHttpRequestIdentifier>>()
               .AddHttpMessageHandler<HttpRequestIdentificationHandler<CitizenIdHttpRequestIdentifier>>();
            
            services.AddSingleton<IUserDeviceRepository, MongoUserDeviceRepository>();
            services.AddSingleton<IDeviceIdGenerator, DeviceIdGenerator>();
            services.AddSingleton<IDeviceRepositoryService, DeviceRepositoryService>();
            services.AddSingleton<IMongoClient, UsersApiMongoClient>();
            services.AddTransient<IOperationStepBuilder, OperationStepBuilder>();
            services.AddTransient<RemoveRegistrationsStep>();
            services.AddTransient<CreateRegistrationIdStep>();
            services.AddTransient<CreateOrUpdateRegistrationStep>();
            services.AddSingleton<ICitizenIdClient, CitizenIdClient>();
            services.AddSingleton<ICitizenIdConfig, CitizenIdConfig>();
            services.AddSingleton<ICitizenIdSigningKeysService, CitizenIdSigningKeysService>();
            services.AddSingleton<ITokenValidationParameterBuilder, TokenValidationParameterBuilder>();

            base.ConfigureServices(services, configuration);
        }
    }
}