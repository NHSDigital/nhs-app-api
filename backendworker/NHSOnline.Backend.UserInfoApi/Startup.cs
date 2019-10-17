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
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Middleware;
using NHSOnline.Backend.Support.Repository;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.UserInfoApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }
        private bool IsDevelopment => Environment.IsDevelopment();

        private readonly ModularStartup _modularStartup;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            Configuration = configuration;
            _modularStartup = new ModularStartup(configuration, loggerFactory);
            _logger = loggerFactory.CreateLogger<Startup>();
            Environment = env;
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
                ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOptions();
            services.AddCorrelationId();

            services.AddSingleton<RandomNumberGenerator, RNGCryptoServiceProvider>();
            services.AddSingleton<IRandomStringGenerator, RandomStringGenerator>();
            services.AddSingleton<IErrorReferenceGenerator, ErrorReferenceGenerator>();

            services.AddSingleton(typeof(HttpTimeoutHandler<>));
            services.AddSingleton(typeof(HttpRequestIdentificationHandler<>));

            _modularStartup.ConfigureServices(services);

            ConfigureAuth(services);
        }

        private void SetupConfigurationSettings(IServiceCollection services)
        {
            var mongoConfiguration = CreateMongoConfiguration();
            services.AddSingleton(mongoConfiguration);


            var configurationSettings = CreateAndValidateEnvironmentVariables();
            services.AddSingleton(configurationSettings);
            services.AddSingleton<IHttpTimeoutConfigurationSettings>(configurationSettings);
        }


        private HttpTimeoutConfigurationSettings CreateAndValidateEnvironmentVariables()
        {
            var defaultHttpTimeoutSeconds = Configuration.GetIntOrThrow("ConfigurationSettings:DefaultHttpTimeoutSeconds", _logger);
            var config = new HttpTimeoutConfigurationSettings(defaultHttpTimeoutSeconds);
            return config;
        }

        private IMongoConfiguration CreateMongoConfiguration()
        {
            var databaseName = Configuration.GetOrThrow("USERINFO_MONGO_DATABASE_NAME", _logger);
            var userInfoCollectionName =
                Configuration.GetOrThrow("USERINFO_MONGO_DATABASE_COLLECTION", _logger);
            var host = Configuration.GetOrThrow("USERINFO_MONGO_DATABASE_HOST", _logger);
            var port = Configuration.GetIntOrThrow("USERINFO_MONGO_DATABASE_PORT", _logger);
            var username = Configuration.GetOrNull("USERINFO_MONGO_DATABASE_USERNAME");
            var password = Configuration.GetOrNull("USERINFO_MONGO_DATABASE_PASSWORD");

            return new MongoConfiguration(host, port, databaseName, username, password, userInfoCollectionName);
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
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // Read in optional log configuration...
            var logSettings = LoggingSettings.GetSettings(Configuration);
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Out, logSettings.StandardLevel,
                logSettings.ErrorLevel, logSettings.CensorFilters));
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Error, logSettings.ErrorLevel,
                LogLevel.None, logSettings.CensorFilters));

            if (IsDevelopment)
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

        private void ConfigureAuth(IServiceCollection services)
        {
            var clientId = Configuration.GetOrThrow("CITIZEN_ID_CLIENT_ID", _logger);
            var issuer = Configuration.GetOrThrow("CITIZEN_ID_JWT_ISSUER", _logger);
            var authority = Configuration.GetOrThrow("CITIZEN_ID_BASE_URL", _logger);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;

                    if (IsDevelopment)
                    {
                        options.RequireHttpsMetadata = false;
                    }

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = clientId,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };
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

    internal class HttpTimeoutConfigurationSettings : IHttpTimeoutConfigurationSettings
    {
        public HttpTimeoutConfigurationSettings(int defaultHttpTimeoutSeconds)
        {
            DefaultHttpTimeoutSeconds = defaultHttpTimeoutSeconds;
        }

        public int DefaultHttpTimeoutSeconds { get; set; }
    }
}