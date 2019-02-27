using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisPatientRecordServices(this IServiceCollection services)
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

            return services;
        }
    }
}
