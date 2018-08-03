using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TppPatientRecordService>();
            
            services.AddTransient<ITppMyRecordMapper, TppMyRecordMapper>();
            services.AddTransient<ITppDetailedTestResultMapper, TppDetailedTestResultMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}