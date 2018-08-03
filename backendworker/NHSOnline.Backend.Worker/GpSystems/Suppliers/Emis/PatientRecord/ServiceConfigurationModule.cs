using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<EmisPatientRecordService>();
            
            services.AddTransient<IEmisMyRecordMapper, EmisMyRecordMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
