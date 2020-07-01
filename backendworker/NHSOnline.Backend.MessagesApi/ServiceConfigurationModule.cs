using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.MessagesApi
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterRepository<UserMessage, MessagesRepositoryConfiguration>();
            services.AddSingleton<IMessagesValidationService, MessagesValidationService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IMessageRepository, MessageRepository>();
            services.AddSingleton<IMapper<List<UserMessage>, MessagesResponse>, MessagesResponseMapper>();
            services.AddSingleton<IMapper<List<SummaryMessage>, MessagesResponse>, MessagesResponseMapper>();

            ConfigureUserProfileServices(services);
            ConfigureCitizenIdServices(services);

            base.ConfigureServices(services, configuration);
        }

        private static void ConfigureCitizenIdServices(IServiceCollection services)
        {
            services.AddHttpClient<CitizenIdHttpClient>()
                .AddHttpMessageHandler<HttpTimeoutHandler<CitizenIdHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<CitizenIdHttpRequestIdentifier>>();

            services.AddScoped<ICitizenIdService, CitizenIdService>();
            services.AddSingleton<ICitizenIdClient, CitizenIdClient>();
            services.AddTransient<CitizenIdHttpRequestIdentifier>();
            services.AddSingleton<ICitizenIdJwtHelper, CitizenIdJwtHelper>();
            services.AddSingleton<ICitizenIdConfig, CitizenIdConfig>();
            services.AddSingleton<ITokenValidationParameterBuilder, TokenValidationParameterBuilder>();
            services.AddSingleton<ISecurityTokenValidator, JwtSecurityTokenHandler>();
            services.AddSingleton<IJwtTokenService<IdToken>, IdTokenService>();
            services.AddSingleton<ICitizenIdSigningKeysService, CitizenIdSigningKeysService>();
        }

        private static void ConfigureUserProfileServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
            services.AddScoped<UserProfileService>();
            services.AddScoped<IUserProfileService>(sp => sp.GetService<UserProfileService>());
            services.AddScoped<IMetricContext, AccessTokenMetricContext>();

            services.Configure<MvcOptions>(opts =>
            {
                opts.ModelBinderProviders.Insert(0, new UserProfileBinderProvider());

                // Special binding source prevents the user profile parameter from being validated as part of the model state
                // https://stackoverflow.com/a/56893947
                opts.ModelMetadataDetailsProviders.Add(new BindingSourceMetadataProvider(typeof(UserProfile), BindingSource.Special));
            });
        }
    }
}