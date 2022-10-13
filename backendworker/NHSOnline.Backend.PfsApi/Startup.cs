using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.AspNet.CorrelationId;
using NHSOnline.Backend.AspNet.HealthChecks;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter;
using NHSOnline.Backend.AspNet.Middleware.PerformanceCounter;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Fake;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.PfsApi.Areas.KnownServices.Models;
using NHSOnline.Backend.PfsApi.DependencyInjection;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.DependencyInjection;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Users;
using NHSOnline.Backend.Users.Notifications;
using ConfigurationSettings = NHSOnline.Backend.Support.Settings.ConfigurationSettings;

namespace NHSOnline.Backend.PfsApi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        private readonly ILogger<Startup> _logger;

        private IConfiguration Configuration { get; }

        private readonly ModularStartup _modularStartup;
        private readonly SupplierStartup _supplierStartup;

        private readonly string _apiAppVersion;

        private ConfigurationSettings _configurationSettings;

        public Startup(IConfiguration configuration, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _env = env;

            _apiAppVersion = Configuration.GetApiAppVersion();

            _modularStartup = new ModularStartup(configuration, loggerFactory);
            _supplierStartup = new SupplierStartup(configuration, loggerFactory, new GpSystemRegistrationService());

            _logger = loggerFactory.CreateLogger<Startup>();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Note that some service registration has now been moved into Module classes within the namespaces containing the services that they register, to avoid namespace dependency cycles.
        public void ConfigureServices(IServiceCollection services)
        {
            SetupConfigurationSettings(services);
            SetupNotificationConfigurationSettings(services);

            services.ConfigureAuthentication(Configuration, _logger, _env.IsDevelopment(), _configurationSettings);

            services.AddScoped<CustomCookieAuthenticationEvents>();
            services.AddNhsAppCorrelationId();

            services.AddCors();

            services.SetupApiKeys(Configuration, _logger);
            services.SetupHttpHandlers();

            services.AddMemoryCache(options=> options.SizeLimit = 1000000);

            services
                .AddControllers(PfsMvcOptions.Configure)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            services.AddApiVersioning(options =>
            {
                options.UseApiBehavior = false; // Treat all controllers as API controller
                options.DefaultApiVersion = ApiVersion.Parse("1.0");
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ErrorResponses = new NotFoundErrorResponseProvider();
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            services.AddNhsAppHealthCheckService(Configuration);
            services.AddPerformanceCounterService(Configuration);

            services.RegisterDatabaseClient(Configuration, _logger);
            services.RegisterSqlApiDatabaseClient(Configuration, _logger);

            _supplierStartup.ConfigureServices(services);
            _modularStartup.ConfigureServices(services);
        }

        private void SetupConfigurationSettings(IServiceCollection services)
        {
            services.UseConfigurationValidation();

            _configurationSettings = ConfigurationSettings.CreateAndValidate(Configuration, _logger);
            services.AddSingleton(_configurationSettings);
            services.AddSingleton<IHttpTimeoutConfigurationSettings>(_configurationSettings);

            services.AddSingleton(EmisConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(TppConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(VisionConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(SessionConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(OnlineConsultationsConfigurationSettings.CreateAndValidate(Configuration, _logger));

            services.AddSingleton(AppointmentsConfigurationSettings.CreateAndValidate(Configuration, _logger));

            services.AddSingleton(SpineLdapConfigurationSettings.CreateAndValidate(Configuration, _logger));

            var nominatedPharmacyConfig = new NominatedPharmacyConfigurationSettings(false, null, null, 0, null, null);
            services.AddSingleton<INominatedPharmacyConfigurationSettings>(nominatedPharmacyConfig);

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            services.ConfigureValidatableSetting<KnownServicesV2>(Configuration);
            services.ConfigureValidatableSetting<KnownServicesV3>(Configuration);
            services.RegisterFakeUserConfiguration(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UsePerformanceCounterMiddleware(Configuration, _logger);

            app.UseMiddleware<SessionLoggingScopeMiddleware>();

            app.Use(async (context, next) =>
            {
                var logger = loggerFactory.CreateLogger<Startup>();
                logger.LogVersion(context, "PfsApi", _apiAppVersion);
                await next();
            });

            app.UseSecurityResponseHeadersMiddleware();
            app.UseResponseHeadersMiddleware();

            app.UseRouting();
            app.UseCors(Configuration);
            app.UseAuthentication();
            app.UseAuthorization(); // order is important here
            app.UseMiddleware<ProxyAuditingMiddleware>();

            app.UseNhsAppCorrelationId();

            app.UseEndpoints(b =>
            {
                b.MapHealthCheckEndpoints();
                b.MapControllers();
            });

            _modularStartup.Configure(app, env);
        }

        private void SetupNotificationConfigurationSettings(IServiceCollection services)
        {
            var mockNotificationHub = Configuration.GetBoolOrFallback("MOCK_NOTIFICATION_HUB_CLIENT", false);
            INotificationHubClientFactory clientFactory;

            if (mockNotificationHub)
            {
                _logger.LogWarning("Using mock notification hub client");
                clientFactory = new MockNotificationHubClientFactory();
            }
            else
            {
                clientFactory = new NotificationHubClientFactory();
            }

            var notificationsConfiguration = CreateNotificationsConfiguration();

            services.AddSingleton(
                CreateAzureNotificationHubConfigurations()
                    .Select(x =>
                        new AzureNotificationHubWrapper(x, notificationsConfiguration, clientFactory))
                    .Cast<IAzureNotificationHubWrapper>()
            );

            var httpTimeoutConfig = CreateHttpTimeoutConfiguration();
            services.AddSingleton<IHttpTimeoutConfigurationSettings>(httpTimeoutConfig);
            services.AddSingleton<INotificationsConfiguration>(notificationsConfiguration);
        }

        private IEnumerable<AzureNotificationHubConfiguration> CreateAzureNotificationHubConfigurations()
        {
            var output = new List<AzureNotificationHubConfiguration>();
            var hubConfigurations = Configuration.GetSection("AZURE_NOTIFICATION_HUBS").GetChildren();

            foreach (var config in hubConfigurations)
            {
                var connectionString = config.GetOrThrow("AZURE_NOTIFICATION_HUB_CONNECTION_STRING", _logger);
                var path = config.GetOrThrow("AZURE_NOTIFICATION_HUB_PATH", _logger);
                var sharedAccessKey = config.GetOrThrow("AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY", _logger);
                var readCharacters = config.GetOrThrow("AZURE_NOTIFICATION_HUB_READ_CHARACTERS", _logger);
                var writeCharacters = config.GetOrNull("AZURE_NOTIFICATION_HUB_WRITE_CHARACTERS");
                var generation = config.GetIntOrThrow("AZURE_NOTIFICATION_HUB_GENERATION", _logger);

                output.Add(new AzureNotificationHubConfiguration(
                    connectionString,
                    path,
                    sharedAccessKey,
                    readCharacters,
                    writeCharacters,
                    generation
                ));
            }
            new AzureNotificationHubConfigurationValidator(_logger).Validate(output);

            return output;
        }

        private HttpTimeoutConfigurationSettings CreateHttpTimeoutConfiguration()
        {
            var defaultHttpTimeoutSeconds = Configuration.GetIntOrThrow("ConfigurationSettings:DefaultHttpTimeoutSeconds", _logger);
            var config = new HttpTimeoutConfigurationSettings(defaultHttpTimeoutSeconds);
            return config;
        }

        private NotificationsConfiguration CreateNotificationsConfiguration()
        {
            var iosBadgeCountEnabled = Configuration.GetBoolOrFallback("IOS_BADGE_COUNT_ENABLED", false, _logger);
            var notificationInstallationExpiryMonths = Configuration.GetIntOrThrow("NOTIFICATION_INSTALLATION_EXPIRY_MONTHS", _logger);

            return new NotificationsConfiguration(iosBadgeCountEnabled, notificationInstallationExpiryMonths);
        }

    }
}
