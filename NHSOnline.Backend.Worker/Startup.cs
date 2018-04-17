using System;
using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.CitizenId;
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

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(
                    options =>
                    {
                        options.Filters.Add(typeof(ModelStateValidationFilter));
                        options.Filters.Add(typeof(AccessControlAllowOriginFilter));
                    }
                )
                .AddJsonOptions(
                    options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
                );

            services.AddSingleton<ISystemProviderFactory, SystemProviderFactory>();
            services.AddSingleton<IEmisClient, EmisClient>();
            services.AddSingleton<IEmisConfig, EmisConfig>();
            services.AddSingleton<IOdsCodeLookup, OdsCodeLookup>();
            services.AddSingleton<ISessionCacheService, SessionCacheService>();
            services.AddSingleton<ICitizenIdService, CitizenIdService>();
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
            services.AddSingleton<IHttpClientFactory, HttpClientFactory>();

            var module = services.FirstOrDefault(t => t.ImplementationFactory?.GetType() == typeof(Func<IServiceProvider, DependencyTrackingTelemetryModule>));
            if (module != null)
            {
                services.Remove(module);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePathBase(new PathString("/v1"));

            app.UseMvc();
        }
    }
}
