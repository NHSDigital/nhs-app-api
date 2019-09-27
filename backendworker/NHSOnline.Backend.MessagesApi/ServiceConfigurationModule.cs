using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.MessagesApi
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IMessageRepository, MongoMessageRepository>();
            services.AddSingleton<IMongoClient, ApiMongoClient>();
            services.AddSingleton<ICitizenIdClient, CitizenIdClient>();
            services.AddSingleton<ICitizenIdConfig, CitizenIdConfig>();
            services.AddSingleton<ICitizenIdSigningKeysService, CitizenIdSigningKeysService>();

            base.ConfigureServices(services, configuration);
        }
    }
}