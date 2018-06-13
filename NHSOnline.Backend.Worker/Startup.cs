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
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.ResponseParsers;
using NHSOnline.Backend.Worker.Support.DependencyInjection;
using NHSOnline.Backend.Worker.Support.Logging;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net.Http;

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
        // Note that some service registration has now been moved into Module classes within the namespaces containing the services that they register, to avoid namespace dependency cycles.
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
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);

                    if (_env.IsDevelopment())
                    {
                        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                        options.Cookie.SameSite = SameSiteMode.None;
                    }
                    else
                    {
                        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                        options.Cookie.SameSite = SameSiteMode.Lax;
                    }
                });

            services.AddScoped<CustomCookieAuthenticationEvents>();

            services.AddCors();
            services
                .AddMvc(
                    options =>
                    {
                        options.Filters.Add(typeof(HttpContextLogActionFilterAttribute));
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
            services.AddSingleton<IOdsCodeLookup, OdsCodeLookup>();
            services.AddSingleton<ISessionCacheService, SessionCacheService>();
            services.AddSingleton<ICipherService, CipherService>();
            services.AddSingleton<IJsonResponseParser, JsonResponseParser>();
            services.AddSingleton<IXmlResponseParser, XmlResponseParser>();
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

            _modularStartup.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();

            // Read in optional log configuration...
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Out, LogLevel.Information, Configuration["Logging:Application:StandardLevel"], LogLevel.Error, Configuration["Logging:Application:ErrorLevel"]));
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Error, LogLevel.Error, Configuration["Logging:Application:ErrorLevel"]));

            if (env.IsDevelopment())
            {
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
