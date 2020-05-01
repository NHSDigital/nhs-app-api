using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public sealed class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISessionCreator, SessionCreator>();
            services.AddTransient<SessionCreatorCitizenIdService>();
            services.AddTransient<SessionCreatorServiceJourneyRuleService>();
            services.AddTransient<SessionCreatorUserInfoService>();

            services.AddScoped<UserSessionService>();
            services.AddTransient<IUserSessionService>(sp => sp.GetRequiredService<UserSessionService>());

            services.AddTransient<IUserSessionManager, UserSessionManager>();
            services.AddTransient<UserSessionCreator>();
            services.AddTransient<P9UserSessionCreator>();
            services.AddTransient<P5UserSessionCreator>();
            
            services.AddTransient<SessionLoggerScope>();

            services.Configure<MvcOptions>(opts =>
            {
                opts.ModelBinderProviders.Insert(0, new UserSessionBinderProvider());

                // Special binding source prevents the user session parameter from being validated as part of the model state
                // https://stackoverflow.com/a/56893947
                opts.ModelMetadataDetailsProviders.Add(new BindingSourceMetadataProvider(typeof(UserSession), BindingSource.Special));
            });
        }
    }
}