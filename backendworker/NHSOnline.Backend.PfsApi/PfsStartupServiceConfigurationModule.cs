using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Im1Connection.Cache;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Messages;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Mappers;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using Wkhtmltopdf.NetCore;
using ServiceConfigurationModule = NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule;

namespace NHSOnline.Backend.PfsApi
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Discovered by ModularStartup")]
    internal sealed class PfsStartupServiceConfigurationModule : ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            base.ConfigureServices(services, configuration);

            services.AddWkhtmltopdf();

            services.AddSingleton(configuration);
            services.AddMongoSessionCacheService();
            services.AddIm1CacheService(configuration);
            services.AddTransient<IGpSessionManager, GpSessionManager>();
            services.AddTransient<IOdsCodeLookup, OdsCodeLookup>();
            services.AddTransient<IGpSystemResolver, GpSystemResolver>();
            services.AddSingleton<IOdsCodeMassager, OdsCodeMassager>();
            services.AddSingleton<ISecurityTokenValidator, JwtSecurityTokenHandler>();
            services.AddTransient(typeof(HttpTimeoutHandler<>));
            services.AddTransient(typeof(HttpRequestIdentificationHandler<>));

            NominatedPharmacyStartup.RegisterServices(services);

            services.AddHostedService<SpinePdsConfigurationBackgroundService>();

            ConfigureMessagingServices(services, configuration);
        }

        private void ConfigureMessagingServices(IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterRepository<UserMessage, MessagesRepositoryConfiguration>(configuration); //error
            services.AddSingleton<IMessagesValidationService, MessagesValidationService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IMessageRepository, MessageRepository>();
            services.AddSingleton<IMapper<List<UserMessage>, MessagesResponse>, MessagesResponseMapper>();
            services.AddSingleton<IMapper<UserMessage, MessagesResponse>, MessagesResponseMapper>();
            services.AddSingleton<IMapper<List<SummaryMessage>, MessagesResponse>, MessagesResponseMapper>();
            services.AddSingleton<IMapper<AddMessageRequest, string, UserMessage>, UserMessageMapper>();
            services.AddScoped<IMessageLinkClickedService, MessageLinkClickedService>();
            services.AddSingleton<IMessageLinkClickedValidationService, MessageLinkClickedValidationService>();
            services.AddSingleton<IMapper<SenderContext, SenderContextEventLogData>, MessageSenderContextEventLogDataMapper>();

            services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
        }
    }
}
