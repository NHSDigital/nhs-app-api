using CorrelationId;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using System.Security.Cryptography;
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
using NHSOnline.Backend.Support.Logging;
using StackExchange.Redis;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.PfsApi.DependencyInjection;
using NHSOnline.Backend.PfsApi.Devices;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.RequestFormatters;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.DependencyInjection;
using SettingValidationStartupFilter = NHSOnline.Backend.Support.AspNet.Filters.SettingValidationStartupFilter;
using NHSOnline.Backend.Support.Middleware;
using NHSOnline.Backend.PfsApi.SpineSearch;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.PfsApi
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<Startup> _logger;

        private IConfiguration Configuration { get; }

        private readonly ModularStartup _modularStartup;
        private readonly SupplierStartup _supplierStartup;

        private readonly string _apiAppVersion;

        private ConfigurationSettings configurationSettings;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _env = env;
            _loggerFactory = loggerFactory;

            var logSettings = LoggingSettings.GetSettings(Configuration);
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Out, logSettings.StandardLevel, logSettings.ErrorLevel, logSettings.CensorFilters));
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Error, logSettings.ErrorLevel, LogLevel.None, logSettings.CensorFilters));

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

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(ConfigureServiceCookies);

            services.AddScoped<CustomCookieAuthenticationEvents>();

            services.AddCorrelationId();

            services.AddCors();

            services.AddMemoryCache();

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

            services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();
            services.AddSingleton<IMongoSessionCacheServiceConfig, MongoSessionCacheServiceConfig>();
            services.AddSingleton<IGuidCreator, GuidCreator>();
            services.AddSingleton<ISessionCacheService, MongoSessionCacheService>();
            services.AddTransient<IIm1CacheServiceConfig, Im1CacheServiceConfig>();
            services.AddSingleton<IIm1CacheService, Im1CacheService>();
            services.AddSingleton<IUserSessionManager, UserSessionManager>();
            services.AddSingleton<IOdsCodeLookup, OdsCodeLookup>();
            services.AddSingleton<IOdsCodeMassager, OdsCodeMassager>();
            services.AddSingleton<ISecurityTokenValidator, JwtSecurityTokenHandler>();
            services.AddSingleton<IConnectionMultiplexerFactory, ConnectionMultiplexerFactory>();
            services.AddSingleton<RandomNumberGenerator, RNGCryptoServiceProvider>();
            services.AddSingleton<IRandomStringGenerator, RandomStringGenerator>();
            services.AddSingleton<IErrorReferenceGenerator, ErrorReferenceGenerator>();
            services.AddSingleton(typeof(HttpTimeoutHandler<>));
            services.AddSingleton(typeof(HttpRequestIdentificationHandler<>));

            services.Configure<OnlineConsultationsProvidersSettings>(Configuration.GetSection("OnlineConsultationsProvidersSettings"));
            services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<OnlineConsultationsProvidersSettings>>().Value);
            services.AddSingleton<IValidatable>(resolver =>
                resolver.GetRequiredService<IOptions<OnlineConsultationsProvidersSettings>>().Value);

            services.AddSingleton<IFhirSanitizationHelper, FhirSanitizationHelper>();
            services.AddSingleton<IOnlineConsultationsProviderHttpClientPool, OnlineConsultationsProviderHttpClientPool>();

            services.AddSingleton(x => new NamedConnectionMultiplexer(
                ConnectionMultiplexerName.OdsCodeLookup,
                ConnectionMultiplexer.Connect(Configuration["REDIS_ODSLOOKUP_CONFIG"])));
            // Add functionality to inject IOptions<T>
            services.AddOptions();

            _supplierStartup.ConfigureServices(services);
            _modularStartup.ConfigureServices(services);
            NominatedPharmacyStartup.RegisterServices(services);
        }

        private static void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add(typeof(HttpContextAuditActionFilterAttribute), 1);
            options.Filters.Add(typeof(HttpContextLogActionFilterAttribute), 1);
            options.Filters.Add(typeof(ModelStateValidationFilterAttribute), 1);
            options.Filters.Add(new AuthorizeFilter(
                new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            );

            /* NB - order of adding these filters is important. LIFO stack is used, and the optional
             *      'Order' parameter appears to be ignored.
             *      Therefore please ensure UnhandledExceptionFilterAttribute is added first, so that
             *      it is invoked as a last resort. */
            options.Filters.Add(typeof(UnhandledExceptionFilterAttribute));
            options.Filters.Add(typeof(TimeoutExceptionFilterAttribute));
            options.Filters.Add(typeof(UnauthorisedGpSystemHttpRequestExceptionFilterAttribute));

            options.InputFormatters.Insert(0, new FhirParametersInputFormatter());
        }

        private void ConfigureServiceCookies(CookieAuthenticationOptions options)
        {
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

        private void SetupConfigurationSettings(IServiceCollection services, string environment)
        {
            configurationSettings = CreateAndValidateEnvironmentVariables();

            services.AddSingleton(configurationSettings);
            services.AddSingleton<IHttpTimeoutConfigurationSettings>(configurationSettings);

            var deviceConfigurationSettings = CreateAndValidateDeviceEnvironmentVariables();
            services.AddSingleton(deviceConfigurationSettings);

            var microtestConfig = CreateAndValidateMicrotestEnvironmentVariables(environment);
            services.AddSingleton(microtestConfig);

            var emisConfig = CreateAndValidateEmisEnvironmentVariables(environment);
            services.AddSingleton(emisConfig);

            var tppConfig = CreateAndValidateTppEnvironmentVariables(environment);
            services.AddSingleton(tppConfig);

            var visionConfig = CreateAndValidateVisionEnvironmentVariables(environment);
            services.AddSingleton(visionConfig);

            var proxyConfig = CreateAndValidateProxyEnvironmentVariables();
            services.AddSingleton(proxyConfig);

            var isSpineLdapLookupEnabled = bool.TrueString.Equals(Configuration.GetOrWarn("SPINE_LDAP_LOOKUP_ENABLED", _logger), StringComparison.OrdinalIgnoreCase);

            var spineLdapConfigurationSettings = CreateAndValidateSpineLdapConfig(isSpineLdapLookupEnabled);
            services.AddSingleton(spineLdapConfigurationSettings);

            NhsAppSpinePdsTraceProperties nhsAppSpinePdsTraceProperties = null;
            NhsAppSpinePdsUpdateProperties nhsAppSpinePdsUpdateProperties = null;

            

            if (isSpineLdapLookupEnabled)
            {
                var spineSearchService = GetSpineSearchService(spineLdapConfigurationSettings);
                nhsAppSpinePdsTraceProperties = spineSearchService.RetrieveSpinePropertiesForPdsTrace();
                nhsAppSpinePdsUpdateProperties = spineSearchService.RetrieveSpinePropertiesForPdsUpdate();
            }
            else
            {
                nhsAppSpinePdsTraceProperties = new NhsAppSpinePdsTraceProperties
                {
                    FromAsid = Configuration.GetOrWarn("TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", _logger),
                    ToAsid = Configuration.GetOrWarn("TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", _logger),
                };

                nhsAppSpinePdsUpdateProperties = new NhsAppSpinePdsUpdateProperties
                {
                    FromAsid = Configuration.GetOrWarn("TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", _logger),
                    ToAsid = Configuration.GetOrWarn("TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", _logger),
                    CpaId = Configuration.GetOrWarn("TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", _logger),
                    ToPartyId = Configuration.GetOrWarn("TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", _logger),
                };
            }

            var nominatedPharmacyConfig = CreateAndValidateSpineEnvironmentVariables(nhsAppSpinePdsTraceProperties, nhsAppSpinePdsUpdateProperties);
            services.AddSingleton<INominatedPharmacyConfigurationSettings>(nominatedPharmacyConfig);
        }

        private SpineSearchService GetSpineSearchService(SpineLdapConfigurationSettings spineLdapConfigurationSettings)
        {
            return new SpineSearchService(
                _loggerFactory.CreateLogger<SpineSearchService>(),
                spineLdapConfigurationSettings,
                new LdapConnectionService(
                    _loggerFactory.CreateLogger<LdapConnectionService>(),
                    spineLdapConfigurationSettings,
                    configurationSettings,
                    new CertificateService(
                        _loggerFactory.CreateLogger<CertificateService>(),
                        Configuration)));
        }

        private SessionConfigurationSettings CreateAndValidateProxyEnvironmentVariables()
        {
            var proxyEnabled = bool.TrueString.Equals(Configuration.GetOrThrow("PROXY_ACCESS_ENABLED", _logger), StringComparison.OrdinalIgnoreCase);
            return new SessionConfigurationSettings(proxyEnabled);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();

            var logSettings = LoggingSettings.GetSettings(Configuration);
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Out, logSettings.StandardLevel, logSettings.ErrorLevel, logSettings.CensorFilters));
            loggerFactory.AddProvider(new HttpContexedLoggerProvider(Console.Error, logSettings.ErrorLevel, LogLevel.None, logSettings.CensorFilters));

            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                var logger = loggerFactory.CreateLogger<Startup>();
                logger.LogVersion(context, "PfsApi", _apiAppVersion);
                await next();
            });

            app.UseSecurityResponseHeadersMiddleware();
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

            _modularStartup.Configure(app, env);
        }

        private ConfigurationSettings CreateAndValidateEnvironmentVariables()
        {
            var cookieDomain = Configuration["ConfigurationSettings:CookieDomain"];
            var prescriptionsDefaultLastNumberMonthsToDisplay = Configuration.GetIntOrThrow(
                "ConfigurationSettings:PrescriptionsDefaultLastNumberMonthsToDisplay", _logger
            );
            var defaultSessionExpiryMinutes = Configuration.GetIntOrThrow("ConfigurationSettings:DefaultSessionExpiryMinutes" ,_logger);
            var defaultHttpTimeoutSeconds = Configuration.GetIntOrThrow("ConfigurationSettings:DefaultHttpTimeoutSeconds" ,_logger);
            var minimumAppAge = Configuration.GetIntOrThrow("ConfigurationSettings:MinimumAppAge", _logger);
            var minimumLinkageAge = Configuration.GetIntOrThrow("ConfigurationSettings:MinimumLinkageAge", _logger);
            var currentTermsConditionsEffectiveDate = DateTimeOffset.Parse(
                Configuration.GetOrWarn("ConfigurationSettings:CurrentTermsConditionsEffectiveDate", _logger),
                CultureInfo.InvariantCulture
            );

            var config =  new ConfigurationSettings(cookieDomain, prescriptionsDefaultLastNumberMonthsToDisplay,
            defaultSessionExpiryMinutes, defaultHttpTimeoutSeconds,  minimumAppAge, minimumLinkageAge, currentTermsConditionsEffectiveDate);

            config.Validate();
            return config;
        }

        private SpineLdapConfigurationSettings CreateAndValidateSpineLdapConfig(bool isLDAPEnabled)
        {
            var nhsAppPartyId = Configuration.GetOrThrow("NHS_APP_PARTY_ID_FOR_SPINE", _logger);
            SpineLdapConfigurationSettings config;

            if (isLDAPEnabled)
            {
                var ldapHost = Configuration.GetOrThrow("SPINE_LDAP_HOST", _logger);
                var ldapPort = Configuration.GetIntOrThrow("SPINE_LDAP_PORT", _logger);
                var loginDn = Configuration.GetOrThrow("SPINE_LDAP_LOGIN_DN", _logger);
                var certPath = Configuration.GetOrThrow("SPINE_CERT_PATH", _logger);
                var certPassword = Configuration.GetOrThrow("SPINE_CERT_PASSWORD", _logger);
                config = new SpineLdapConfigurationSettings(ldapHost, ldapPort, loginDn, certPath, certPassword, nhsAppPartyId);
            }
            else
            {
                config = new SpineLdapConfigurationSettings(nhsAppPartyId);
            }

            config.Validate(isLDAPEnabled);

            return config;
        }

        public EmisConfigurationSettings CreateAndValidateEmisEnvironmentVariables(string environment){
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

        public MicrotestConfigurationSettings CreateAndValidateMicrotestEnvironmentVariables(string environment)
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

        public DeviceConfigurationSettings CreateAndValidateDeviceEnvironmentVariables()
        {
            var minimumSupportedAndroidVersion = Configuration["ConfigurationSettings:MinimumSupportedAndroidVersion"];
            var minimumSupportediOSVersion = Configuration["ConfigurationSettings:MinimumSupportediOSVersion"];
            var fidoServerUrl = new Uri(Configuration["ConfigurationSettings:FidoServerUrl"], UriKind.Absolute);
            var throttlingEnabled = Configuration["ConfigurationSettings:ThrottlingEnabled"];

            var config = new DeviceConfigurationSettings(minimumSupportedAndroidVersion, minimumSupportediOSVersion, fidoServerUrl, throttlingEnabled);
            config.Validate();

            return config;
        }

        public TppConfigurationSettings CreateAndValidateTppEnvironmentVariables(string environment){
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
            var minimumLinkageAge = Configuration.GetIntOrWarn("ConfigurationSettings:MinimumLinkageAge", _logger);

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
                environment
                );

                config.Validate();
                return config;
        }

        public VisionConfigurationSettings CreateAndValidateVisionEnvironmentVariables(string environment)
        {
            var applicationProviderId = Configuration.GetOrWarn("VISION_APPLICATION_PROVIDER_ID", _logger);
            var apiBaseUriString = Configuration.GetOrWarn("VISION_BASE_URI", _logger);
            var visionPFSPath = Configuration.GetOrWarn("VISION_PFS_PATH", _logger);
            var certificatePath = Configuration.GetOrWarn("VISION_CERT_PATH", _logger);
            var certificatePassphrase = Configuration.GetOrWarn("VISION_CERT_PASSPHRASE", _logger);
            var requestUsername = Configuration.GetOrWarn("VISION_USERNAME", _logger);
            var visionSenderUserName = Configuration.GetOrWarn("VISION_SENDER_USERNAME", _logger);
            var visionSenderUserFullName = Configuration.GetOrWarn("VISION_SENDER_USERFULLNAME", _logger);
            var visionSenderUserIdentity = Configuration.GetOrWarn("VISION_SENDER_USERIDENTITY", _logger);
            var visionSenderUserRole = Configuration.GetOrWarn("VISION_SENDER_USERROLE", _logger);

            var prescriptionsMaxCoursesSoftLimit = Configuration.GetIntOrWarn("ConfigurationSettings:PrescriptionsMaxCoursesSoftLimit", _logger);
            var coursesMaxCoursesLimit = Configuration.GetIntOrWarn("ConfigurationSettings:CoursesMaxCoursesLimit", _logger);
            var visionAppointmentSlotsRequestCount = Configuration.GetIntOrWarn("ConfigurationSettings:VisionAppointmentSlotsRequestCount", _logger);

            var config = new VisionConfigurationSettings(
                applicationProviderId,
                new Uri(apiBaseUriString + visionPFSPath, UriKind.Absolute),
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
                environment);

                config.Validate();
                return config;
        }

        public NominatedPharmacyConfigurationSettings CreateAndValidateSpineEnvironmentVariables(NhsAppSpinePdsTraceProperties nhsAppSpinePdsTraceProperties, NhsAppSpinePdsUpdateProperties nhsAppSpinePdsUpdateProperties)
        {
            var isNominatedPharmacyEnabled = bool.TrueString.Equals(Configuration.GetOrWarn("NOMINATED_PHARMACY_ENABLED", _logger), StringComparison.OrdinalIgnoreCase);
            var nominatedPharmacyUriString = Configuration.GetOrWarn("SPINE_PDS_URL", _logger);
            var pdsQueryFromAddress = Configuration.GetOrWarn("PDS_QUERY_FROM_ADDRESS", _logger);
            var pdsQueryTo = Configuration.GetOrWarn("PDS_QUERY_TO", _logger);
            var artificialDelayAfterNominatedPharmacyUpdateInMilliseconds = Configuration.GetIntOrDefault("DELAY_AFTER_NOMINATED_PHARMACY_UPDATE_IN_MILLISECONDS", _logger);

            var pdsTraceConfigurationSettings = new PdsTraceConfigurationSettings
            {
                FromAddress = pdsQueryFromAddress,
                ToAddress = pdsQueryTo,
                FromAsid = nhsAppSpinePdsTraceProperties.FromAsid,
                ToAsid = nhsAppSpinePdsTraceProperties.ToAsid,
            };

            var pdsUpdateConfigurationSettings = new PdsUpdateConfigurationSettings
            {
                FromAsid = nhsAppSpinePdsUpdateProperties.FromAsid,
                ToAsid = nhsAppSpinePdsUpdateProperties.ToAsid,
                CpaId = nhsAppSpinePdsUpdateProperties.CpaId,
                FromPartyId = Configuration.GetOrWarn("NHS_APP_PARTY_ID_FOR_SPINE", _logger),
                ToPartyId = nhsAppSpinePdsUpdateProperties.ToPartyId,
            };

            var config = new NominatedPharmacyConfigurationSettings(
                isNominatedPharmacyEnabled,
                new Uri(nominatedPharmacyUriString, UriKind.Absolute),
                artificialDelayAfterNominatedPharmacyUpdateInMilliseconds,
                pdsTraceConfigurationSettings,
                pdsUpdateConfigurationSettings
                );

            if (isNominatedPharmacyEnabled)
            {
                var nominatedPharmacyConfigIsValid = config.Validate();
                if (!nominatedPharmacyConfigIsValid)
                {
                    _logger.LogWarning($"Not all nominated pharmacy config is populated, disabling nominated pharmacy feature (initial value was {isNominatedPharmacyEnabled})");
                    config.IsNominatedPharmacyEnabled = false;
                }
            }

            return config;
        }
    }
}
