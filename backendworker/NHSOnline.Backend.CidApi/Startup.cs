using System.IdentityModel.Tokens.Jwt;
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
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.AspNet.CorrelationId;
using NHSOnline.Backend.AspNet.HealthChecks;
using NHSOnline.Backend.AspNet.Middleware;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.CidApi.DependencyInjection;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;
using NHSOnline.Backend.CidApi.Filters;
using NHSOnline.Backend.GpSystems.Im1Connection.Cache;
using NHSOnline.Backend.GpSystems.Suppliers.Fake;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.DependencyInjection;

namespace NHSOnline.Backend.CidApi
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        private readonly ModularStartup _modularStartup;
        private readonly SupplierStartup _supplierStartup;
        private readonly string _apiAppVersion;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;

            _apiAppVersion = Configuration.GetApiAppVersion();

            _modularStartup = new ModularStartup(configuration, loggerFactory);
            _supplierStartup = new SupplierStartup(configuration, loggerFactory, new GpSystemRegistrationService());

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Note that some service registration has now been moved into Module classes within the namespaces containing the services that they register, to avoid namespace dependency cycles.
        public void ConfigureServices(IServiceCollection services)
        {
            SetupConfigurationSettings(services);

            services.AddNhsAppCorrelationId();

            services.AddCors();

            services.AddMemoryCache();

            services
                .AddControllers(ConfigureMvcOptions)
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

            services.AddNhsAppHealthCheckService();

            services.AddSingleton(Configuration);

            services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();

            services.AddIm1CacheService();
            services.AddSingleton<IOdsCodeMassager, OdsCodeMassager>();
            services.AddTransient<IRetrieveLinkageKeysService, RetrieveLinkageKeysService>();
            services.AddTransient<IGetLinkageKeysService, GetLinkageKeysService>();
            services.AddTransient<ICreateLinkageKeysService, CreateLinkageKeysService>();
            services.AddTransient<IOdsCodeLookup, OdsCodeLookup>();
            services.AddTransient<IGpSystemResolver, GpSystemResolver>();

            services.AddSingleton<ISecurityTokenValidator, JwtSecurityTokenHandler>();
            services.AddTransient(typeof(HttpTimeoutHandler<>));
            services.AddTransient(typeof(HttpRequestIdentificationHandler<>));

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            _supplierStartup.ConfigureServices(services);
            _modularStartup.ConfigureServices(services);
        }

        private static void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add(typeof(HttpContextAuditActionFilterAttribute), 1);
            options.Filters.Add(typeof(ModelStateValidationFilterAttribute), 1);
            options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            options.Filters.Add(typeof(Im1TimeoutExceptionFilterAttribute));
            options.Filters.Add(typeof(UnauthorisedGpSystemHttpRequestExceptionFilterAttribute));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Use(async (context, next) =>
            {
                var logger = loggerFactory.CreateLogger<Startup>();
                logger.LogVersion(context, "CidApi", _apiAppVersion);
                await next();
            });

            app.UseSecurityResponseHeadersMiddleware();
            app.UseResponseHeadersMiddleware();

            app.UseRouting();
            app.UseCors(Configuration);
            app.UseAuthentication();

            app.UseNhsAppCorrelationId();

            app.UseLogRequestHeader(new LogRequestHeaderOptions
            {
                HeaderName = Constants.HttpHeaders.LoginClient,
            });

            app.UseEndpoints(b => b.MapControllers());

            _modularStartup.Configure(app, env);
        }

        private void SetupConfigurationSettings(IServiceCollection services)
        {
            var configurationSettings = CreateAndValidateEnvironmentVariables();
            services.AddSingleton(configurationSettings);
            services.AddSingleton<IHttpTimeoutConfigurationSettings>(configurationSettings);

            services.AddSingleton(EmisConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(TppConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(MicrotestConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(VisionConfigurationSettings.CreateAndValidate(Configuration, _logger));

            services.RegisterFakeUserConfiguration(Configuration);
        }

        private ConfigurationSettings CreateAndValidateEnvironmentVariables()
        {
            var cookieDomain = Configuration["ConfigurationSettings:CookieDomain"];
            var prescriptionsDefaultLastNumberMonthsToDisplay = Configuration.GetIntOrWarn(
                "ConfigurationSettings:PrescriptionsDefaultLastNumberMonthsToDisplay", _logger
            );
            var defaultSessionExpiryMinutes = Configuration.GetIntOrWarn("ConfigurationSettings:DefaultSessionExpiryMinutes", _logger);
            var defaultHttpTimeoutSeconds = Configuration.GetIntOrWarn("ConfigurationSettings:DefaultHttpTimeoutSeconds", _logger);
            var minimumAppAge = Configuration.GetIntOrWarn("ConfigurationSettings:MinimumAppAge", _logger);
            var minimumLinkageAge = Configuration.GetIntOrWarn("ConfigurationSettings:MinimumLinkageAge", _logger);


            var config = new ConfigurationSettings(cookieDomain, prescriptionsDefaultLastNumberMonthsToDisplay,
                defaultSessionExpiryMinutes, defaultHttpTimeoutSeconds, minimumAppAge, minimumLinkageAge);

            config.Validate();
            return config;
        }
    }
}
