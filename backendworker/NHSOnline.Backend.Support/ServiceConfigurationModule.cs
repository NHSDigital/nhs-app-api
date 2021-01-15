using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Support
{
    public sealed class ServiceConfigurationModule : DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            base.ConfigureServices(services, configuration);

            services.AddTransient<IGuidCreator, GuidCreator>();

            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddTransient<RandomNumberGenerator, RNGCryptoServiceProvider>();
            services.AddTransient<IRandomStringGenerator, RandomStringGenerator>();
            services.AddTransient<IErrorReferenceGenerator, ErrorReferenceGenerator>();

            services.AddTransient<IFireAndForgetService, FireAndForgetService>();
        }
    }
}