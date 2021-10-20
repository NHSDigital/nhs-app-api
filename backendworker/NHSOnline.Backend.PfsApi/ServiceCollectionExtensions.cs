using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.Configuration;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add an <see cref="IStartupFilter"/> to the application that invokes <see cref="IValidatable.Validate"/> on all registered objects
        /// </summary>
        public static IServiceCollection UseConfigurationValidation(this IServiceCollection services)
        {
            return services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();
        }

        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions"/> will bind against, and registers as a validatble setting.
        /// Additionally registers the configuration object directly with the DI container, so can be retrieved without referencing IOptions.
        ///
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured</typeparam>
        /// <param name="services">The <see cref="IServiceCollection "/> to add the services</param>
        /// <param name="configuration">The configuration being bound.</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureValidatableSetting<TOptions>
            (this IServiceCollection services, IConfiguration configuration)
            where TOptions : class, IValidatable, new()
        {
            services.Configure<TOptions>(configuration);
            services.AddSingleton(ctx => ctx.GetRequiredService<IOptions<TOptions>>().Value);
            services.AddSingleton<IValidatable>(ctx => ctx.GetRequiredService<IOptions<TOptions>>().Value);
            return services;
        }

        public static IServiceCollection SetupApiKeys(this IServiceCollection services, IConfiguration configuration, ILogger<Startup> logger)
        {
            var secureKeyValue = configuration.GetOrThrow("NHSAPP_API_KEY", logger);
            var apiKeyConfig = new ApiKeyConfig(new[] { new SecureApiKey("ExternalService", secureKeyValue) });
            services.AddSingleton<IApiKeyConfig>(apiKeyConfig);
            services.AddSingleton<IGetApiKeyQuery, InMemoryGetApiKeyQuery>();

            return services;
        }

        public static IServiceCollection SetupHttpHandlers(this IServiceCollection services)
        {
            services.AddTransient(typeof(HttpTimeoutHandler<>));
            services.AddTransient(typeof(HttpRequestIdentificationHandler<>));
            return services;
        }

        public static IServiceCollection ConfigureAuthentication
            (this IServiceCollection services,
            IConfiguration configuration, ILogger<Startup> logger,
            bool isDevelopment, IConfigurationSettings configurationSettings)
        {
            var clientId = configuration.GetOrThrow("CITIZEN_ID_CLIENT_ID", logger);
            var issuer = configuration.GetOrThrow("CITIZEN_ID_JWT_ISSUER", logger);
            var authority = configuration.GetOrThrow("CITIZEN_ID_BASE_URL", logger);

            services.AddAuthentication()
                .AddCookie(options => {
                    CookieAuthenticationConfiguration(options, isDevelopment, configurationSettings);
                })
                .AddJwtBearer(options => {
                    JwtBearerAuthenticationConfiguration(options, isDevelopment, authority, clientId, issuer);
                })
                .AddApiKeySupport(options => { });
            return services;
        }

        private static void JwtBearerAuthenticationConfiguration(
            JwtBearerOptions options, bool isDevelopment,  string authority, string clientId, string issuer)
        {
            options.Authority = authority;

            if (isDevelopment)
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
        }

        private static void CookieAuthenticationConfiguration(
            CookieAuthenticationOptions options, bool isDevelopment, IConfigurationSettings configurationSettings)
        {
            options.Cookie.Name = Constants.CookieNames.SessionId;
            options.Cookie.HttpOnly = true;
            options.EventsType = typeof(CustomCookieAuthenticationEvents);
            options.TicketDataFormat = new UnencryptedCookieDataFormat();
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = isDevelopment
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;

            if (!string.IsNullOrEmpty(configurationSettings.CookieDomain))
            {
                options.Cookie.Domain = configurationSettings.CookieDomain;
            }
        }
    }
}
