using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.NominatedPharmacy;
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
            services.AddTransient<IIm1CacheServiceConfig, Im1CacheServiceConfig>();
            services.AddTransient<IIm1CacheService, Im1CacheService>();
            services.AddTransient<IGpSessionManager, GpSessionManager>();
            services.AddTransient<IOdsCodeLookup, OdsCodeLookup>();
            services.AddTransient<IGpSystemResolver, GpSystemResolver>();
            services.AddSingleton<IOdsCodeMassager, OdsCodeMassager>();
            services.AddSingleton<ISecurityTokenValidator, JwtSecurityTokenHandler>();
            services.AddTransient(typeof(HttpTimeoutHandler<>));
            services.AddTransient(typeof(HttpRequestIdentificationHandler<>));

            NominatedPharmacyStartup.RegisterServices(services);

            services.AddHostedService<SpinePdsConfigurationBackgroundService>();
        }
    }
}