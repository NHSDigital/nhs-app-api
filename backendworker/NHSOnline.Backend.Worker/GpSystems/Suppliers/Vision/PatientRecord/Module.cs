using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<VisionPatientRecordService>();
            services.AddTransient<IVisionMyRecordMapper, VisionMyRecordMapper>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}
