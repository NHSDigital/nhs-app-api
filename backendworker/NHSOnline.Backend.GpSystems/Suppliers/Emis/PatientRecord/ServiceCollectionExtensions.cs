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
            services.AddTransient<EmisDocumentsMapper>();
            services.AddTransient<IEmisPatientDocumentMapper, EmisPatientDocumentMapper>();

            services.AddTransient<GetAllergiesTaskChecker>();
            services.AddTransient<GetMedicationsTaskChecker>();
            services.AddTransient<GetImmunisationsTaskChecker>();
            services.AddTransient<GetProblemsTaskChecker>();
            services.AddTransient<GetTestResultsTaskChecker>();
            services.AddTransient<GetConsultationsTaskChecker>();
            services.AddTransient<GetDocumentsTaskChecker>();
            services.AddTransient<IEmisDocumentDownloadConverter, EmisDocumentDownloadConverter>();
            services.AddTransient<IGetPatientDocumentTaskChecker, GetPatientDocumentTaskChecker>();

            services.AddTransient<EmisPatientRecordService>();

            services.AddTransient<IEmisMyRecordMapper, EmisMyRecordMapper>();

            return services;
        }
    }
}
