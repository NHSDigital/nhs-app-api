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
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Mappers;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfoApi.Repository;

namespace NHSOnline.Backend.UserInfoApi
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMapper<UserProfile, InfoUserProfile>, InfoUserProfileMapper>();
            services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();

            ConfigureRepositoryServices(services);
            ConfigureCitizenIdServices(services);
            ConfigureUserProfileServices(services);

            base.ConfigureServices(services, configuration);
        }

        private static void ConfigureRepositoryServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IApiMongoClient<>), typeof(ApiMongoClient<>));
            services.AddSingleton<IRepository<UserAndInfo>, MongoRepository<IMongoConfiguration, UserAndInfo>>();
            services.AddSingleton<IInfoService, InfoService>();
            services.AddSingleton<IInfoRepository, UserInfoRepository>();
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