using CorrelationId;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.DependencyInjection;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Middleware;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.UsersApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }
        private bool IsDevelopment => Environment.IsDevelopment();

        private readonly ModularStartup _modularStartup;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, IWebHostEnvironment env)
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
                .AddControllers(ConfigureMvcOptions)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            SetupApiKeys(services);

            services.AddOptions();
            services.AddCorrelationId();

            services.AddSingleton(typeof(HttpTimeoutHandler<>));
            services.AddSingleton(typeof(HttpRequestIdentificationHandler<>));

            _modularStartup.ConfigureServices(services);

            ConfigureAuth(services);
        }

        private void SetupApiKeys(IServiceCollection services)
        {
            var secureKeyValue = Configuration.GetOrThrow("NHSAPP_API_KEY", _logger);
            var apiKeyConfig = new ApiKeyConfig(new[] { new SecureApiKey("ExternalService", secureKeyValue) });
            services.AddSingleton<IApiKeyConfig>(apiKeyConfig);
            services.AddSingleton<IGetApiKeyQuery, InMemoryGetApiKeyQuery>();
        }

        private void SetupConfigurationSettings(IServiceCollection services)
        {
            var azureNotificationConfiguration = CreateAzureNotificationConfiguration();
            services.AddSingleton(azureNotificationConfiguration);

            var httpTimeoutConfig = CreateHttpTimeoutConfiguration();
            services.AddSingleton<IHttpTimeoutConfigurationSettings>(httpTimeoutConfig);
        }

        private AzureNotificationConfiguration CreateAzureNotificationConfiguration()
        {
            var azureConnectionString = Configuration.GetOrThrow("AZURE_NOTIFICATION_HUB_CONNECTION_STRING", _logger);
            var notificationHubPath = Configuration.GetOrThrow("AZURE_NOTIFICATION_HUB_PATH", _logger);
            var sharedAccessKey = Configuration.GetOrThrow("AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY", _logger);
            return new AzureNotificationConfiguration(azureConnectionString, notificationHubPath, sharedAccessKey);
        }

        private HttpTimeoutConfigurationSettings CreateHttpTimeoutConfiguration()
        {
            var defaultHttpTimeoutSeconds = Configuration.GetIntOrThrow("ConfigurationSettings:DefaultHttpTimeoutSeconds", _logger);
            var config = new HttpTimeoutConfigurationSettings(defaultHttpTimeoutSeconds);
            return config;
        }

        private static void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add(typeof(ModelStateValidationFilterAttribute), 1);
            options.Filters.Add(typeof(TimeoutExceptionFilterAttribute));
            options.Filters.Add<UserProfileFilter>();
            options.Filters.Add(new AuthorizeFilter(
                new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UsePathBase("/v1");

            app.UseSecurityResponseHeadersMiddleware();
            app.UseResponseHeadersMiddleware();

            app.UseRouting();
            app.UseCors(Configuration);
            app.UseAuthentication();
            app.UseAuthorization();

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

            app.UseEndpoints(b => b.MapControllers());
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
    }
}