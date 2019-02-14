using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Emis;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<EmisAllergyMapper>();
            services.AddTransient<IEmisMedicationMapper, EmisMedicationMapper>();
            services.AddTransient<EmisImmunisationMapper>();
            services.AddTransient<EmisProblemMapper>();
            services.AddTransient<EmisTestResultMapper>();
            services.AddTransient<EmisConsultationMapper>();

            services.AddTransient<GetAllergiesTaskChecker>();
            services.AddTransient<GetMedicationsTaskChecker>();
            services.AddTransient<GetImmunisationsTaskChecker>();
            services.AddTransient<GetProblemsTaskChecker>();
            services.AddTransient<GetTestResultsTaskChecker>();
            services.AddTransient<GetConsultationsTaskChecker>();

            services.AddTransient<EmisPatientRecordService>();

            services.AddTransient<IEmisMyRecordMapper, EmisMyRecordMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
