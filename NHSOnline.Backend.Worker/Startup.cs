using System;
using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Ods;
using NHSOnline.Backend.Worker.Router;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker
{
    public class Startup
    {
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
                    options => { options.Filters.Add(typeof(ModelStateValidationFilter)); }
                    )
                .AddJsonOptions(
                    options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                    );

            services.AddSingleton<ISystemProviderFactory, SystemProviderFactory>();
            services.AddSingleton<IEmisClient, EmisClient>();
            services.AddSingleton<IEmisConfig, EmisConfig>();
            services.AddSingleton<IOdsCodeLookup, OdsCodeLookup>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<EmisSystemProvider>();
            services.AddSingleton(x => new NamedConnectionMultiplexer(
                ConnectionMultiplexerName.OdsCodeLookup,
                ConnectionMultiplexer.Connect(Configuration["REDIS_ODSLOOKUP_CONFIG"])));
            services.AddSingleton<IConnectionMultiplexerFactory, ConnectionMultiplexerFactory>();

            services.AddSingleton(x => new NamedHttpClient(HttpClientName.EmisApiClient, new HttpClient()));
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

            app.UseMvc();
        }
    }
}
