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
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.DependencyInjection;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Middleware;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.MessagesApi
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

            SetupApiKeys(services);

            services.AddOptions();
            services.AddCorrelationId();

            services.AddSingleton<RandomNumberGenerator, RNGCryptoServiceProvider>();
            services.AddSingleton<IRandomStringGenerator, RandomStringGenerator>();
            services.AddSingleton<IErrorReferenceGenerator, ErrorReferenceGenerator>();

            _modularStartup.ConfigureServices(services);

            ConfigureAuth(services);
        }

        private void SetupConfigurationSettings(IServiceCollection services)
        {
            var mongoConfiguration = CreateMongoConfiguration();
            services.AddSingleton(mongoConfiguration);
        }

        private IMongoConfiguration CreateMongoConfiguration()
        {
            var connectionString = Configuration.GetOrThrow("DEVICES_MONGO_CONNECTION_STRING", _logger);
            var databaseName = Configuration.GetOrThrow("MESSAGES_MONGO_DATABASE_NAME", _logger);
            var messagesCollectionName =
                Configuration.GetOrThrow("MESSAGES_MONGO_DATABASE_MESSAGES_COLLECTION", _logger);

            return new MongoConfiguration(connectionString, databaseName,  messagesCollectionName);
        }

        private void SetupApiKeys(IServiceCollection services)
        {
            var secureKeyValue = Configuration.GetOrThrow("NHSAPP_API_KEY", _logger);
            var apiKeyConfig = new ApiKeyConfig(new[] { new SecureApiKey("ExternalService", secureKeyValue) });
            services.AddSingleton<IApiKeyConfig>(apiKeyConfig);
            services.AddSingleton<IGetApiKeyQuery, InMemoryGetApiKeyQuery>();
        }

        private static void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add(typeof(ModelStateValidationFilterAttribute), 1);
            options.Filters.Add(typeof(TimeoutExceptionFilterAttribute));
            options.Filters.Add(new AuthorizeFilter(
                new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.ConfigureLogging(Configuration);

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
                })
                .AddApiKeySupport(options =>
                {
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