using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Users.Registrations
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IRegistrationService, RegistrationService>();
            services.AddTransient<INotificationsDecisionAuditService, NotificationsDecisionAuditService>();

            base.ConfigureServices(services, configuration);
        }
    }
}