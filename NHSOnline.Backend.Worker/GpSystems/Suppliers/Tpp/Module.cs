using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class Module : Support.DependencyInjection.Module
    {
        private readonly ILoggerFactory _loggerFactory;

        public Module(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var configValue = configuration.ConfigurationSettings().GetOrWarn("DefaultHttpTimeoutSeconds",
                _loggerFactory.CreateLogger<Module>());
            var timeout = int.Parse(configValue);
            
            services.AddSingleton<TppHttpClientHandler>();
            services.AddSingleton(x => new NamedHttpClient(
                HttpClientName.TppApiClient,
                new HttpClient(
                    new TppHttpClientHandler(configuration, _loggerFactory.CreateLogger<TppHttpClientHandler>())
                )
                {
                    Timeout = TimeSpan.FromSeconds(timeout)
                }));

            services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
            
            services.AddSingleton<IGpSystem, TppGpSystem>();
            services.AddSingleton<ITppClient, TppClient>();
            services.AddSingleton<ITppConfig, TppConfig>();

            services.AddTransient<TppIm1ConnectionService>();
            services.AddTransient<TppSessionService>();
            services.AddTransient<TppTokenValidationService>();

            base.ConfigureServices(services, configuration);
        }
    }
}