using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(typeof(IApiMongoClient<>), typeof(ApiMongoClient<>));
            
            services.AddSingleton<IMessagesValidationService, MessagesValidationService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IRepository<UserMessage>, MongoRepository<IMongoConfiguration, UserMessage>>();
            services.AddSingleton<IMessageRepository, MessageRepository>();
            services.AddSingleton<IMapper<List<UserMessage>, MessagesResponse>, MessagesResponseMapper>();
            services.AddSingleton<IMapper<List<SummaryMessage>, MessagesResponse>, MessagesResponseMapper>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}