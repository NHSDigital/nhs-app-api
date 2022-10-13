using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Users.Areas.Devices;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Repository;

namespace NHSOnline.Backend.Users
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterRepository<UserDevice, UsersRepositoryConfiguration>(configuration);
            services.AddSingleton<IUserDeviceRepository, UserDeviceRepository>();
            services.AddSingleton<IDeviceRepositoryService, DeviceRepositoryService>();
            services.AddSingleton<IMapper<AddNotificationSenderContext, SenderContextEventLogData>, NotificationSenderContextEventLogDataMapper>();

            ConfigureUserProfileServices(services);

            base.ConfigureServices(services, configuration);
        }

        private static void ConfigureUserProfileServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
            services.AddScoped<UserProfileService>();
            services.AddScoped<IUserProfileService>(sp => sp.GetService<UserProfileService>());
            services.AddTransient<AccessTokenMetricContext>();

            services.Configure<MvcOptions>(opts =>
            {
                opts.ModelBinderProviders.Insert(0, new UserProfileBinderProvider());

                // Special binding source prevents the user profile parameter from being validated as part of the model state
                // https://stackoverflow.com/a/56893947
                opts.ModelMetadataDetailsProviders.Add(new BindingSourceMetadataProvider(typeof(UserProfile), BindingSource.Special));
            });
        }
    }
}