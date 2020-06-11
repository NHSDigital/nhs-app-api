using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMicrotestPatientRecordServices(this IServiceCollection services)
        {
            services.AddTransient<MicrotestPatientRecordService>();
            services.AddTransient<IMicrotestMyRecordMapper, MicrotestMyRecordMapper>();
            services.AddTransient<MicrotestMyRecordSummaryRecordMapper>();
            services.AddTransient<MicrotestMyRecordDetailedRecordMapper>();
            services.AddTransient<MicrotestMyRecordImmunisationsMapper>();
            services.AddTransient<MicrotestMyRecordProblemsMapper>();
            services.AddTransient<MicrotestMyRecordTestResultsMapper>();
            services.AddTransient<MicrotestMyRecordMedicalHistoryMapper>();
            services.AddTransient<MicrotestMyRecordRecallsMapper>();
            services.AddTransient<MicrotestMyRecordEncountersMapper>();
            services.AddTransient<MicrotestMyRecordReferralsMapper>();

            return services;
        }
    }
}