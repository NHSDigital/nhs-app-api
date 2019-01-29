using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Vision;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<VisionAllergyMapper>();
            services.AddTransient<VisionImmunisationsMapper>();
            services.AddTransient<VisionMedicationMapper>();
            services.AddTransient<VisionProblemsMapper>();
            services.AddTransient<VisionTestResultsMapper>();
            services.AddTransient<VisionDiagnosisMapper>();
            services.AddTransient<VisionExaminationsMapper>();
            services.AddTransient<VisionProceduresMapper>();

            services.AddTransient<VisionPatientRecordService>();
            services.AddTransient<IVisionMyRecordMapper, VisionMyRecordMapper>();

            base.ConfigureServices(services, configuration);
        }
    }
}