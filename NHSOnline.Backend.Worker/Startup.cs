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
using NHSOnline.Backend.Worker.Bridges.Emis.Demographics;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Date;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Validators;
using NHSOnline.Backend.Worker.Support.DependencyInjection;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker
{
    public class Startup
    {
        private const int DefaultHttpTimeoutSeconds = 10;
        private readonly IHostingEnvironment _env;
        private IConfiguration Configuration { get; }

        private readonly ModularStartup _modularStartup;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;

            _modularStartup = new ModularStartup(configuration);
        }

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
                });

            services.AddScoped<CustomCookieAuthenticationEvents>();

            services.AddCors();
            services
                .AddMvc(
                    options =>
                    {
                        options.Filters.Add(typeof(ModelStateValidationFilterAttribute));
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
            services.AddSingleton(Configuration);
            services.AddSingleton<IBridgeFactory, BridgeFactory>();
            services.AddSingleton<IEmisClient, EmisClient>();
            services.AddSingleton<IEmisConfig, EmisConfig>();
            services.AddTransient<IEmisPrescriptionMapper, EmisPrescriptionMapper>();
            services.AddTransient<IEmisDemographicsMapper, EmisDemographicsMapper>();
            services.AddTransient<IPrescriptionRequestValidationService, PrescriptionRequestValidationService>();
            services.AddSingleton<IOdsCodeLookup, OdsCodeLookup>();
            services.AddSingleton<ISessionCacheService, SessionCacheService>();
            services.AddSingleton<ICipherService, CipherService>();

            services.AddSingleton<HttpClient>();
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
            services.AddSingleton<TimeZoneInfoProvider>();
            services.AddSingleton<TimeZoneConverter>();
            services.AddSingleton<IDateTimeOffsetProvider, DateTimeOffsetProvider>();

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            var module = services.FirstOrDefault(t => t.ImplementationFactory?.GetType() == typeof(Func<IServiceProvider, DependencyTrackingTelemetryModule>));

            if (module != null)
            {
                services.Remove(module);
            }

            _modularStartup.ConfigureServices(services);
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

            _modularStartup.Configure(app, env);
        }

        private void EnsureConfigurationSettingsPopulated(ConfigurationSettings config)
        {
            if (config.PrescriptionsDefaultLastNumberMonthsToDisplay == null)
            {
                throw new Exception(string.Format(ExceptionMessages.ConfigurationValueNotFound, nameof(config.PrescriptionsDefaultLastNumberMonthsToDisplay)));
            }

            if (config.PrescriptionsMaxCoursesSoftLimit == null)
            {
                throw new Exception(string.Format(ExceptionMessages.ConfigurationValueNotFound, nameof(config.PrescriptionsMaxCoursesSoftLimit)));
            }

            if (config.CoursesMaxCoursesLimit == null)
            {
                throw new Exception(string.Format(ExceptionMessages.ConfigurationValueNotFound, nameof(config.CoursesMaxCoursesLimit)));
            }
            
            if (config.DefaultSessionExpiryMinutes == default(int))
            {
                throw new Exception(string.Format(ExceptionMessages.ConfigurationValueNotFound, nameof(config.DefaultSessionExpiryMinutes)));
            }
        }
    }
}
