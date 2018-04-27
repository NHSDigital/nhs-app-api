using System;
using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.DataProtection;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Ods;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Session;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker
{
    public class  Startup
    {
        public const int DefaultHttpTimeoutSeconds = 10;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configurationSettings = Configuration.GetSection("ConfigurationSettings").Get<ConfigurationSettings>();
            EnsureConfigurationSettingsPopulated(configurationSettings);
            services.Configure<ConfigurationSettings>(Configuration.GetSection("ConfigurationSettings"));
            
            services.AddCors();
            services
                .AddMvc(
                    options =>
                    {
                        options.Filters.Add(typeof(ModelStateValidationFilter));
                    }
                )
                .AddJsonOptions(
                    options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
                );

            services.AddDataProtection();
            services.AddSingleton<ISystemProviderFactory, SystemProviderFactory>();
            services.AddSingleton<IEmisClient, EmisClient>();
            services.AddSingleton<IEmisConfig, EmisConfig>();
            services.AddSingleton<IOdsCodeLookup, OdsCodeLookup>();
            services.AddSingleton<ISessionCacheService, SessionCacheService>();
            services.AddSingleton<ICipherService, CipherService>();
            services.AddSingleton<ICitizenIdService, CitizenIdService>();
            services.AddSingleton<ICitizenIdClient, CitizenIdClient>();
            services.AddSingleton<ICitizenIdConfig, CitizenIdConfig>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<EmisSystemProvider>();
            services.AddSingleton(x => new NamedConnectionMultiplexer(
                ConnectionMultiplexerName.OdsCodeLookup,
                ConnectionMultiplexer.Connect(Configuration["REDIS_ODSLOOKUP_CONFIG"])));
            services.AddSingleton(x => new NamedConnectionMultiplexer(
                ConnectionMultiplexerName.Session,
                ConnectionMultiplexer.Connect(Configuration["REDIS_SESSION_CONFIG"])));
            services.AddSingleton<IConnectionMultiplexerFactory, ConnectionMultiplexerFactory>();
            int.TryParse(Configuration["HTTP_TIMEOUT_SECONDS"], out var timeout);
            timeout = timeout == default(int) ? DefaultHttpTimeoutSeconds : timeout;

            services.AddSingleton(x => new NamedHttpClient(HttpClientName.EmisApiClient, new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeout)
            }));
            services.AddSingleton(x => new NamedHttpClient(HttpClientName.CitizenIdApiClient, new HttpClient()));
            services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
            
            // Add functionality to inject IOptions<T>
            services.AddOptions();
            
            var module = services.FirstOrDefault(t => t.ImplementationFactory?.GetType() == typeof(Func<IServiceProvider, DependencyTrackingTelemetryModule>));

            if (module != null)
            {
                services.Remove(module);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            
            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
                loggerFactory.AddConsole();
                app.UseDeveloperExceptionPage();
            }

            app.UsePathBase(new PathString("/v1"));

            var corsAuthority = Configuration["CORS_AUTHORITY"];
            if (corsAuthority != null)
            {
                app.UseCors(builder => builder.WithOrigins(corsAuthority).AllowAnyMethod().AllowAnyHeader());   
            }
            
            app.UseMvc();
        }

        public void EnsureConfigurationSettingsPopulated(ConfigurationSettings config)
        {
            if (config.PrescriptionsDefaultLastNumberMonthsToDisplay == null)
            {
                throw new Exception(string.Format(ExceptionMessages.ConfigurationValueNotFound, nameof(config.PrescriptionsDefaultLastNumberMonthsToDisplay)));
            }
        }
    }
}
