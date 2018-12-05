using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Support.DependencyInjection;
using NHSOnline.Backend.Worker.Support.Logging;
using StackExchange.Redis;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        
        private readonly ILoggerFactory _loggerFactory;
        
        private readonly RunMode _runMode;
        private IConfiguration Configuration { get; }

        private readonly ModularStartup _modularStartup;

        private readonly string _apiAppVersion;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _env = env;
            _loggerFactory = loggerFactory;
            _runMode = GetRunMode(configuration);

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(LogLevel.Debug);
            }
            
            _apiAppVersion = GetApiAppVersion();

            _modularStartup = new ModularStartup(configuration, loggerFactory);
        }

        private string GetApiAppVersion()
        {
            var apiVersionStringBuilder = new StringBuilder();
            apiVersionStringBuilder.Append(Configuration[Constants.EnvironmentalVariables.VersionTag]);
            apiVersionStringBuilder.Append(" (commit:");
            apiVersionStringBuilder.Append(Configuration[Constants.AppConfig.GitCommitId]);
            apiVersionStringBuilder.Append(")");

            return apiVersionStringBuilder.ToString();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Note that some service registration has now been moved into Module classes within the namespaces containing the services that they register, to avoid namespace dependency cycles.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConfigurationSettings>(
                Configuration.GetSection(ConfigurationSettings.ConfigurationSectionName));

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(ConfigureServiceCookies);

            services.AddScoped<CustomCookieAuthenticationEvents>();
        
            services.AddCors();

            services
                .AddMvc(ConfigureMvcOptions)
                .AddJsonOptions(
                    options => options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver()
                );

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            services.AddSingleton(Configuration);

            services.AddTransient<IIm1CacheServiceConfig, Im1CacheServiceConfig>();
            services.AddSingleton<IIm1CacheService, Im1CacheService>();
            services.AddSingleton<IOdsCodeLookup, OdsCodeLookup>();
            services.AddSingleton<ISecurityTokenValidator, JwtSecurityTokenHandler>();
            services.AddSingleton<IConnectionMultiplexerFactory, ConnectionMultiplexerFactory>();
            services.AddSingleton(typeof(HttpTimeoutHandler<>));
            services.AddSingleton(typeof(HttpRequestIdentificationHandler<>));

            services.AddSingleton(x => new NamedConnectionMultiplexer(
                ConnectionMultiplexerName.OdsCodeLookup,
                ConnectionMultiplexer.Connect(Configuration["REDIS_ODSLOOKUP_CONFIG"])));

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            _modularStartup.ConfigureServices(services);
        }

        private void ConfigureMvcOptions(MvcOptions options)
        {
            options.Conventions.Add(new SecurityModeConvention(
                            _runMode, _loggerFactory.CreateLogger<SecurityModeConvention>()));
            options.Filters.Add(typeof(HttpContextAuditActionFilterAttribute), 1);
            options.Filters.Add(typeof(HttpContextLogActionFilterAttribute), 1);
            options.Filters.Add(typeof(ModelStateValidationFilterAttribute), 1);
            options.Filters.Add(new AuthorizeFilter(
                new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            );
            options.Filters.Add(typeof(TimeoutExceptionFilterAttribute));
        }

        private void ConfigureServiceCookies(CookieAuthenticationOptions options)
        {
            var configurationSettings = ConfigurationSettings.GetSettings(Configuration);

            options.Cookie.Name = Constants.CookieNames.SessionId;
            options.Cookie.HttpOnly = true;
            options.EventsType = typeof(CustomCookieAuthenticationEvents);
            options.TicketDataFormat = new UnencryptedCookieDataFormat();

            if (!string.IsNullOrEmpty(configurationSettings.CookieDomain))
            {
                options.Cookie.Domain = configurationSettings.CookieDomain;
            }

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();

            // Read in optional log configuration...
            var logSettings = LoggingSettings.GetSettings(Configuration);
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Out, logSettings.StandardLevel, logSettings.ErrorLevel, logSettings.CensorFilters));
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Error, logSettings.ErrorLevel, LogLevel.None, logSettings.CensorFilters));

            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
                app.UseDeveloperExceptionPage();
            }

            UseSecurityHeaders(app, _apiAppVersion, loggerFactory.CreateLogger<Startup>());
            app.UseResponseHeadersMiddleware();

            app.UsePathBase(new PathString("/v1"));

            var corsAuthority = Configuration["CORS_AUTHORITY"];
            if (corsAuthority != null)
            {
                app.UseCors(builder => builder
                    .WithOrigins(corsAuthority)
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            }

            app.UseMvc();

            _modularStartup.Configure(app, env);
        }

        private static void UseSecurityHeaders(IApplicationBuilder app, string apiAppVersion, ILogger<Startup> startupLogger)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("Content-Security-Policy", "default-src https:");
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");

                LogVersion(context, apiAppVersion, startupLogger);
                
                await next();
            });
        }

        private static void LogVersion(HttpContext context, string apiAppVersion, ILogger<Startup> startupLogger)
        {
            string webAppVersion = context.Request.Headers[Constants.HttpHeaders.WebAppVersion];
            string nativeAppVersion = context.Request.Headers[Constants.HttpHeaders.NativeAppVersion];

            if (string.IsNullOrEmpty(webAppVersion)) return;
            
            var logMessageStringBuilder = new StringBuilder();
                    
            logMessageStringBuilder.Append(
                $"Beginning HTTP Request. Backendworker version: {apiAppVersion}. Web App Version: {webAppVersion}.");
                    
            if (!string.IsNullOrEmpty(nativeAppVersion))
            {
                logMessageStringBuilder.Append($" Native App Version: {nativeAppVersion}.");
            }

            startupLogger.LogInformation(logMessageStringBuilder.ToString());
        }

        private static RunMode GetRunMode(IConfiguration configuration)
        {
            if (null == configuration["runMode"])
            {
                throw new ConfigurationNotFoundException("command line parameter runMode is not set");
            }

            var stringMode = configuration["runMode"];

            if (!Enum.TryParse(stringMode, true, out RunMode runMode))
            {
                runMode = RunMode.None;
            }

            return runMode;
        }
    }
}