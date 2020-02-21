using System;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using CorrelationId;
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
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.AspNetCore.Mvc;
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
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.DependencyInjection;
using NHSOnline.Backend.Support.Middleware;

namespace NHSOnline.Backend.CidApi
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        private IConfiguration Configuration { get; }
        private readonly ModularStartup _modularStartup;
        private readonly SupplierStartup _supplierStartup;
        private readonly string _apiAppVersion;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(LogLevel.Debug);
            }

            _apiAppVersion = Configuration.GetApiAppVersion();

            _modularStartup = new ModularStartup(configuration, loggerFactory);
            _supplierStartup = new SupplierStartup(configuration, loggerFactory, new GpSystemRegistrationService());

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Note that some service registration has now been moved into Module classes within the namespaces containing the services that they register, to avoid namespace dependency cycles.
        public void ConfigureServices(IServiceCollection services)
        {
            var environment = Configuration.GetOrWarn("ASPNETCORE_ENVIRONMENT", _logger);
            SetupConfigurationSettings(services, environment);

            services.AddCorrelationId();

            services.AddCors();

            services.AddMemoryCache();

            services
                .AddMvc(ConfigureMvcOptions)
                .AddJsonOptions(
                    options => options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver()
                ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = ApiVersion.Parse("1.0");
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ErrorResponses = new NotFoundErrorResponseProvider();
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            services.AddSingleton(Configuration);

            services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();

            services.AddTransient<IGuidCreator, GuidCreator>();
            services.AddTransient<IIm1CacheServiceConfig, Im1CacheServiceConfig>();
            services.AddTransient<IIm1CacheService, Im1CacheService>();
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
            options.Filters.Add(typeof(HttpContextLogActionFilterAttribute), 1);
            options.Filters.Add(typeof(ModelStateValidationFilterAttribute), 1);
            options.Filters.Add(new AuthorizeFilter(
                new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            );
            options.Filters.Add(typeof(Im1TimeoutExceptionFilterAttribute));
            options.Filters.Add(typeof(UnauthorisedGpSystemHttpRequestExceptionFilterAttribute));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();

            loggerFactory.ConfigureLogging(Configuration);

            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                var logger = loggerFactory.CreateLogger<Startup>();
                logger.LogVersion(context, "CidApi", _apiAppVersion);
                await next();
            });

            app.UseSecurityResponseHeadersMiddleware();
            app.UseResponseHeadersMiddleware();

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

            app.UseLogRequestHeader(new LogRequestHeaderOptions
            {
                HeaderName = Constants.HttpHeaders.LoginClient,
            });

            app.UseMvc();

            _modularStartup.Configure(app, env);
        }

        private void SetupConfigurationSettings(IServiceCollection services, string environment)
        {
            var configurationSettings = CreateAndValidateEnvironmentVariables();
            services.AddSingleton(configurationSettings);
            services.AddSingleton<IHttpTimeoutConfigurationSettings>(configurationSettings);

            var microtestConfig = CreateAndValidateMicrotestEnvironmentVariables(environment);
            services.AddSingleton(microtestConfig);

            var emisConfig = CreateAndValidateEmisEnvironmentVariables(environment);
            services.AddSingleton(emisConfig);

            var tppConfig = CreateAndValidateTppEnvironmentVariables(environment);
            services.AddSingleton(tppConfig);

            var visionConfig = CreateAndValidateVisionEnvironmentVariables(environment);
            services.AddSingleton(visionConfig);
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

            var currentTermsConditionsEffectiveDate = DateTimeOffset.Parse(
                Configuration.GetOrWarn("ConfigurationSettings:CurrentTermsConditionsEffectiveDate", _logger),
                CultureInfo.InvariantCulture
            );

            var config = new ConfigurationSettings(cookieDomain, prescriptionsDefaultLastNumberMonthsToDisplay,
            defaultSessionExpiryMinutes, defaultHttpTimeoutSeconds, minimumAppAge, minimumLinkageAge, currentTermsConditionsEffectiveDate);

            config.Validate();
            return config;
        }

        private EmisConfigurationSettings CreateAndValidateEmisEnvironmentVariables(string environment)
        {
            var emisBaseUrl = Configuration.GetOrWarn("EMIS_BASE_URL", _logger);
            var applicationId = Configuration.GetOrWarn("EMIS_APPLICATION_ID", _logger);
            var version = Configuration.GetOrWarn("EMIS_VERSION", _logger);
            var certificatePath = Configuration.GetOrWarn("EMIS_CERTIFICATE_PATH", _logger);
            var certificatePassphrase = Configuration.GetOrWarn("EMIS_CERTIFICATE_PASSWORD", _logger);

            var emisExtendedHttpTimeoutSeconds = Configuration.GetIntOrWarn("ConfigurationSettings:EmisExtendedHttpTimeoutSeconds", _logger);
            var defaultHttpTimeoutSeconds = Configuration.GetIntOrWarn("ConfigurationSettings:DefaultHttpTimeoutSeconds", _logger);
            var coursesMaxCoursesLimit = Configuration.GetIntOrWarn("ConfigurationSettings:CoursesMaxCoursesLimit", _logger);
            var prescriptionsMaxCoursesSoftLimit = Configuration.GetIntOrWarn("ConfigurationSettings:PrescriptionsMaxCoursesSoftLimit", _logger);

            var config = new EmisConfigurationSettings(new Uri(emisBaseUrl, UriKind.Absolute), applicationId, version, certificatePath, certificatePassphrase,
             emisExtendedHttpTimeoutSeconds, defaultHttpTimeoutSeconds, coursesMaxCoursesLimit, prescriptionsMaxCoursesSoftLimit, environment);
            config.Validate();

            return config;
        }

        private MicrotestConfigurationSettings CreateAndValidateMicrotestEnvironmentVariables(string environment)
        {
            var baseUrlstring = Configuration.GetOrWarn("MICROTEST_BASE_URL", _logger);
            var certificatePath = Configuration.GetOrWarn("MICROTEST_CERT_PATH", _logger);
            var certificatePassphrase = Configuration.GetOrWarn("MICROTEST_CERT_PASSPHRASE", _logger);
            var prescriptionsMaxCoursesSoftLimit = Configuration.GetIntOrWarn("ConfigurationSettings:PrescriptionsMaxCoursesSoftLimit", _logger);
            var coursesMaxCoursesLimit = Configuration.GetIntOrWarn("ConfigurationSettings:CoursesMaxCoursesLimit", _logger);

            var config = new MicrotestConfigurationSettings(new Uri(baseUrlstring), certificatePath, certificatePassphrase, environment, prescriptionsMaxCoursesSoftLimit, coursesMaxCoursesLimit);
            config.Validate();

            return config;
        }

        private TppConfigurationSettings CreateAndValidateTppEnvironmentVariables(string environment)
        {
            var tppBaseUrl = Configuration.GetOrWarn("TPP_BASE_URL", _logger);
            var apiVersion = Configuration.GetOrWarn("TPP_API_VERSION", _logger);
            var applicationName = Configuration.GetOrWarn("TPP_APPLICATION_NAME", _logger);
            var applicationVersion = Configuration.GetOrWarn("TPP_APPLICATION_VERSION", _logger);
            var applicationProviderId = Configuration.GetOrWarn("TPP_APPLICATION_PROVIDER_ID", _logger);
            var applicationDeviceType = Configuration.GetOrWarn("TPP_APPLICATION_DEVICE_TYPE", _logger);
            var certificatePath = Configuration.GetOrWarn("TPP_CERTIFICATE_PATH", _logger);
            var certificatePassphrase = Configuration.GetOrWarn("TPP_CERTIFICATE_PASSWORD", _logger);

            var prescriptionsMaxCoursesSoftLimit = Configuration.GetIntOrWarn("ConfigurationSettings:PrescriptionsMaxCoursesSoftLimit", _logger);
            var coursesMaxCoursesLimit = Configuration.GetIntOrWarn("ConfigurationSettings:CoursesMaxCoursesLimit", _logger);

            var config = new TppConfigurationSettings(
                new Uri(tppBaseUrl, UriKind.Absolute),
                apiVersion,
                applicationName,
                applicationVersion,
                applicationProviderId,
                applicationDeviceType,
                certificatePath,
                certificatePassphrase,
                prescriptionsMaxCoursesSoftLimit,
                coursesMaxCoursesLimit,
                environment);

            config.Validate();
            return config;
        }

        private VisionConfigurationSettings CreateAndValidateVisionEnvironmentVariables(string environment)
        {
            var applicationProviderId = Configuration.GetOrWarn("VISION_APPLICATION_PROVIDER_ID", _logger);
            var apiBaseUriString = Configuration.GetOrWarn("VISION_BASE_URI", _logger);
            var visionPfsPath = Configuration.GetOrWarn("VISION_PFS_PATH", _logger);
            var certificatePath = Configuration.GetOrWarn("VISION_CERT_PATH", _logger);
            var certificatePassphrase = Configuration.GetOrWarn("VISION_CERT_PASSPHRASE", _logger);
            var requestUsername = Configuration.GetOrWarn("VISION_USERNAME", _logger);
            var visionSenderUserName = Configuration.GetOrWarn("VISION_SENDER_USERNAME", _logger);
            var visionSenderUserFullName = Configuration.GetOrWarn("VISION_SENDER_USERFULLNAME", _logger);
            var visionSenderUserIdentity = Configuration.GetOrWarn("VISION_SENDER_USERIDENTITY", _logger);
            var visionSenderUserRole = Configuration.GetOrWarn("VISION_SENDER_USERROLE", _logger);

            var prescriptionsMaxCoursesSoftLimit = int.Parse(Configuration["ConfigurationSettings:PrescriptionsMaxCoursesSoftLimit"], CultureInfo.InvariantCulture);
            var coursesMaxCoursesLimit = int.Parse(Configuration["ConfigurationSettings:CoursesMaxCoursesLimit"], CultureInfo.InvariantCulture);
            var visionAppointmentSlotsRequestCount = int.Parse(Configuration["ConfigurationSettings:VisionAppointmentSlotsRequestCount"], CultureInfo.InvariantCulture);

            var config = new VisionConfigurationSettings(
                applicationProviderId,
                new Uri(apiBaseUriString + visionPfsPath, UriKind.Absolute),
                certificatePath,
                certificatePassphrase,
                requestUsername,
                visionSenderUserName,
                visionSenderUserFullName,
                visionSenderUserIdentity,
                visionSenderUserRole,
                visionAppointmentSlotsRequestCount,
                coursesMaxCoursesLimit,
                prescriptionsMaxCoursesSoftLimit,
                environment
                );

            config.Validate();
            return config;
        }
    }
}
