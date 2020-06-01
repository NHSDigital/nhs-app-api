using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
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
            services.AddTransient<CitizenIdHttpRequestIdentifier>();
            services.AddTransient<IMapper<UserProfile, InfoUserProfile>, InfoUserProfileMapper>();

            services.AddHttpClient<CitizenIdHttpClient>()
                .AddHttpMessageHandler<HttpTimeoutHandler<CitizenIdHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<CitizenIdHttpRequestIdentifier>>();

            services.AddScoped<ICitizenIdService, CitizenIdService>();
            services.AddSingleton<ICitizenIdClient, CitizenIdClient>();
            services.AddSingleton<ICitizenIdJwtHelper, CitizenIdJwtHelper>();
            services.AddSingleton<ICitizenIdConfig, CitizenIdConfig>();
            services.AddSingleton<ITokenValidationParameterBuilder, TokenValidationParameterBuilder>();
            services.AddSingleton<ISecurityTokenValidator, JwtSecurityTokenHandler>();
            services.AddSingleton<IJwtTokenService<IdToken>, IdTokenService>();
            services.AddSingleton<ICitizenIdSigningKeysService, CitizenIdSigningKeysService>();

            services.AddSingleton(typeof(IApiMongoClient<>), typeof(ApiMongoClient<>));
            services.AddSingleton<IInfoService, InfoService>();
            services.AddSingleton<IInfoRepository, MongoUserInfoRepository>();

            base.ConfigureServices(services, configuration);
        }
    }
}