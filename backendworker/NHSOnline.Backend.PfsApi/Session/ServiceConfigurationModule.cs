using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public sealed class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureSessionScopeServices(services);
            ConfigureSessionManagementServices(services);
        }

        private static void ConfigureSessionScopeServices(IServiceCollection services)
        {
            services.AddScoped<UserSessionService>();
            services.AddTransient<IUserSessionService>(sp => sp.GetRequiredService<UserSessionService>());
            services.AddTransient<IMetricContext, UserSessionMetricContext>();
            services.AddTransient<ICreateSessionResultVisitor<Task<IActionResult>>, SessionCreateResultVisitor>();
            services.AddSingleton<ISessionErrorResultBuilder, SessionErrorResultBuilder>();

            services.AddTransient<SessionLoggerScope>();

            services.Configure<MvcOptions>(opts =>
            {
                opts.ModelBinderProviders.Insert(0, new UserSessionBinderProvider());

                // Special binding source prevents the session parameters from being validated as part of the model state
                // https://stackoverflow.com/a/56893947
                opts.ModelMetadataDetailsProviders.Add(new BindingSourceMetadataProvider(typeof(UserSession), BindingSource.Special));
            });
        }

        private static void ConfigureSessionManagementServices(IServiceCollection services)
        {
            services.AddTransient<IUserSessionManager, UserSessionManager>();

            ConfigureCreateSessionServices(services);
            ConfigureDeleteSessionServices(services);
        }

        private static void ConfigureCreateSessionServices(IServiceCollection services)
        {
            services.AddTransient<ISessionCreator, SessionCreator>();
            services.AddTransient<SessionCreatorCitizenIdService>();
            services.AddTransient<SessionCreatorServiceJourneyRuleService>();
            services.AddTransient<SessionCreatorUserInfoService>();

            services.AddTransient<UserSessionCreator>();
            services.AddTransient<P9UserSessionCreator>();
            services.AddTransient<P5UserSessionCreator>();
        }

        private static void ConfigureDeleteSessionServices(IServiceCollection services)
        {
            services.AddTransient<UserSessionDeleter>();
            services.AddTransient<UserSessionDeleteSteps>();
            services.AddTransient<IUserSessionDeleteStep<P9UserSession>, UserSessionDeleteGpSessionStep>();
            services.AddTransient<IUserSessionDeleteStep<UserSession>, UserSessionDeleteCachedSessionStep>();
            services.AddTransient<IUserSessionDeleteStep<UserSession>, UserSessionDeleteSignOutStep>();
        }
    }
}