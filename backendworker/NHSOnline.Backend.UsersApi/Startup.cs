using System;
using System.Security.Cryptography;
using CorrelationId;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.DependencyInjection;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Middleware;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.UsersApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private readonly ModularStartup _modularStartup;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _modularStartup = new ModularStartup(configuration, loggerFactory);
            _logger = loggerFactory.CreateLogger<Startup>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SetupConfigurationSettings(services);
            
            services
                .AddMvc(ConfigureMvcOptions)
                .AddJsonOptions(
                    options => options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver()
                );

            services.AddOptions();
            services.AddCorrelationId();

            ConfigureAuth(services);
            
            services.AddSingleton<RandomNumberGenerator, RNGCryptoServiceProvider>();
            services.AddSingleton<IRandomStringGenerator, RandomStringGenerator>();
            services.AddSingleton<IErrorReferenceGenerator, ErrorReferenceGenerator>();

            _modularStartup.ConfigureServices(services);
        }

        private void SetupConfigurationSettings(IServiceCollection services)
        {
            var mongoConfiguration = CreateMongoConfiguration();
            services.AddSingleton(mongoConfiguration);
            
            var config = CreateAzureNotificationConfiguration();
            services.AddSingleton(config);
        }

        private IMongoConfiguration CreateMongoConfiguration()
        {
            var databaseName = Configuration.GetOrThrow("USERS_MONGO_DATABASE_NAME", _logger);
            var userDeviceCollectionName =
                Configuration.GetOrThrow("USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION", _logger);
            var host = Configuration.GetOrThrow("USERS_MONGO_DATABASE_HOST", _logger);
            var port = Configuration.GetIntOrThrow("USERS_MONGO_DATABASE_PORT", _logger);
            var username = Configuration.GetOrNull("USERS_MONGO_DATABASE_USERNAME");
            var password = Configuration.GetOrNull("USERS_MONGO_DATABASE_PASSWORD");

            return new MongoConfiguration(host, port, databaseName, username, password, userDeviceCollectionName);
        }

        private AzureNotificationConfiguration CreateAzureNotificationConfiguration()
        {
            var azureConnectionString = Configuration.GetOrThrow("AZURE_NOTIFICATION_HUB_CONNECTION_STRING", _logger);
            var notificationHubPath = Configuration.GetOrThrow("AZURE_NOTIFICATION_HUB_PATH", _logger);
            var sharedAccessKey = Configuration.GetOrThrow("AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY", _logger);
            return new AzureNotificationConfiguration(azureConnectionString, notificationHubPath, sharedAccessKey);
        }

        private static void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add(typeof(HttpContextLogActionFilterAttribute), 1);
            options.Filters.Add(typeof(ModelStateValidationFilterAttribute), 1);
            options.Filters.Add(typeof(TimeoutExceptionFilterAttribute));
            options.Filters.Add(new AuthorizeFilter(
                new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Read in optional log configuration...
            var logSettings = LoggingSettings.GetSettings(Configuration);
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Out, logSettings.StandardLevel,
                logSettings.ErrorLevel, logSettings.CensorFilters));
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Error, logSettings.ErrorLevel,
                LogLevel.None, logSettings.CensorFilters));

            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
                app.UseDeveloperExceptionPage();
            }

            UseSecurityHeaders(app);
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

            app.UseAuthentication();

            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = Constants.HttpHeaders.CorrelationIdentifier,
                UseGuidForCorrelationId = true,
                UpdateTraceIdentifier = false
            });

            app.UseLogRequestHeader(new LogRequestHeaderOptions
            {
                HeaderName = Constants.HttpHeaders.CorrelationIdentifier,
                LogTemplate = "CorrelationId={value}",
            });

            app.UseMvc();
        }

        private static void ConfigureAuth(IServiceCollection services)
        {
            var tokenValidationParameters = new TokenValidationParameters();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new MockTokenValidation());
                });
        }

        private static void UseSecurityHeaders(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("Content-Security-Policy", "default-src https:");
                context.Response.Headers.Add("Strict-Transport-Security",
                    "max-age=31536000; includeSubDomains; preload");

                await next();
            });
        }
    }
}