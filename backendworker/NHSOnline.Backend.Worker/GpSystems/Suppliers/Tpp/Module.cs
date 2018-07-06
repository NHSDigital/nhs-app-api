using System;
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

            services.AddHttpClient<TppHttpClient>(client => 
                {
                    client.Timeout = TimeSpan.FromSeconds(timeout);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new TppHttpClientHandler(configuration, _loggerFactory.CreateLogger<TppHttpClientHandler>());
                });

            services.AddSingleton<IGpSystem, TppGpSystem>();
            services.AddSingleton<ITppClient, TppClient>();
            services.AddSingleton<ITppConfig, TppConfig>();

            services.AddTransient<TppTokenValidationService>();
                        
            base.ConfigureServices(services, configuration);
        }
    }
}