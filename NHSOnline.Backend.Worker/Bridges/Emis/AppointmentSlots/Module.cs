using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAppointmentSlotsResponseMapper, AppointmentSlotsResponseMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
