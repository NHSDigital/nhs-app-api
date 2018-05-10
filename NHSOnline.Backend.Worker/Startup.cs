using System;
using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Ods;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Validators;
using NHSOnline.Backend.Worker.Session;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker
{
    public class Startup
    {
        private const int DefaultHttpTimeoutSeconds = 10;
        private const int DefaultSessionExpiryMinutes = 20;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configurationSettings = Configuration.GetSection("ConfigurationSettings").Get<ConfigurationSettings>();
            EnsureConfigurationSettingsPopulated(configurationSettings);
            services.Configure<ConfigurationSettings>(Configuration.GetSection("ConfigurationSettings"));

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = Constants.Cookies.SessionId;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = _env.IsDevelopment()
                        ? CookieSecurePolicy.SameAsRequest
                        : CookieSecurePolicy.Always;
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);

                    int.TryParse(Configuration["SESSION_EXPIRY_MINUTES"], out var sessionExpiryMinutes);
                    sessionExpiryMinutes = sessionExpiryMinutes == default(int)
                        ? DefaultSessionExpiryMinutes
                        : sessionExpiryMinutes;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionExpiryMinutes);
                    options.SlidingExpiration = true;
                });

            services.AddScoped<CustomCookieAuthenticationEvents>();

            services.AddCors();
            services
                .AddMvc(
                    options =>
                    {
                        options.Filters.Add(typeof(ModelStateValidationFilter));
                        options.Filters.Add(new AuthorizeFilter(
                            new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
                        );
                    }
                )
                .AddJsonOptions(
                    options => options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver()
                );

            services.AddDataProtection();
            services.AddSingleton<ISystemProviderFactory, SystemProviderFactory>();
            services.AddSingleton<IEmisClient, EmisClient>();
            services.AddSingleton<IEmisConfig, EmisConfig>();
            services.AddTransient<IEmisPrescriptionMapper, EmisPrescriptionMapper>();
            services.AddTransient<IPrescriptionRequestValidationService, PrescriptionRequestValidationService>();
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
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole();
                loggerFactory.AddDebug();
                app.UseDeveloperExceptionPage();
            }

            app.UsePathBase(new PathString("/v1"));

            var corsAuthority = Configuration["CORS_AUTHORITY"];
            if (corsAuthority != null)
            {
                app.UseCors(builder => builder
                    .WithOrigins(corsAuthority)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
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
