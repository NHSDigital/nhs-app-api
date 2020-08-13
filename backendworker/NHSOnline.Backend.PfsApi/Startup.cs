using CorrelationId;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.PfsApi.DependencyInjection;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.DependencyInjection;
using NHSOnline.Backend.Support.Middleware;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Fake;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.PfsApi.Session;
using Wkhtmltopdf.NetCore;
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

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(ConfigureServiceCookies);
            services.AddScoped<CustomCookieAuthenticationEvents>();

            services.AddCorrelationId();

            services.AddCors();

            services.AddMemoryCache();

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

            services.AddWkhtmltopdf();

            services.AddSingleton(Configuration);
            services.AddSingleton<IMongoSessionCacheServiceConfig, MongoSessionCacheServiceConfig>();
            services.AddTransient<ISessionCacheService, MongoSessionCacheService>();
            services.AddTransient<IIm1CacheServiceConfig, Im1CacheServiceConfig>();
            services.AddTransient<IIm1CacheService, Im1CacheService>();
            services.AddTransient<IGpSessionManager, GpSessionManager>();
            services.AddTransient<IOdsCodeLookup, OdsCodeLookup>();
            services.AddTransient<IGpSystemResolver, GpSystemResolver>();
            services.AddSingleton<IOdsCodeMassager, OdsCodeMassager>();
            services.AddSingleton<ISecurityTokenValidator, JwtSecurityTokenHandler>();
            services.AddTransient(typeof(HttpTimeoutHandler<>));
            services.AddTransient(typeof(HttpRequestIdentificationHandler<>));

            _supplierStartup.ConfigureServices(services);
            _modularStartup.ConfigureServices(services);
            NominatedPharmacyStartup.RegisterServices(services);

            services.AddHostedService<SpinePdsConfigurationBackgroundService>();
        }

        private void ConfigureServiceCookies(CookieAuthenticationOptions options)
        {
            options.Cookie.Name = Constants.CookieNames.SessionId;
            options.Cookie.HttpOnly = true;
            options.EventsType = typeof(CustomCookieAuthenticationEvents);
            options.TicketDataFormat = new UnencryptedCookieDataFormat();

            if (!string.IsNullOrEmpty(_configurationSettings.CookieDomain))
            {
                options.Cookie.Domain = _configurationSettings.CookieDomain;
            }

            if (_env.IsDevelopment())
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
            }
            else
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            }
        }

        private void SetupConfigurationSettings(IServiceCollection services)
        {
            services.UseConfigurationValidation();

            _configurationSettings = ConfigurationSettings.CreateAndValidate(Configuration, _logger);
            services.AddSingleton(_configurationSettings);
            services.AddSingleton<IHttpTimeoutConfigurationSettings>(_configurationSettings);

            services.AddSingleton(EmisConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(TppConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(MicrotestConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(VisionConfigurationSettings.CreateAndValidate(Configuration, _logger));
            services.AddSingleton(SessionConfigurationSettings.CreateAndValidate(Configuration, _logger));

            services.AddSingleton(AppointmentsConfigurationSettings.CreateAndValidate(Configuration, _logger));

            services.AddSingleton(SpineLdapConfigurationSettings.CreateAndValidate(Configuration, _logger));

            var nominatedPharmacyConfig = new NominatedPharmacyConfigurationSettings(false, null, null, 0, null, null);
            services.AddSingleton<INominatedPharmacyConfigurationSettings>(nominatedPharmacyConfig);

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            services.ConfigureValidatableSetting<KnownServices>(Configuration);
            services.RegisterFakeUserConfiguration(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
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

            app.UseMiddleware<ProxyAuditingMiddleware>();

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

            _modularStartup.Configure(app, env);
        }
    }
}
