using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterVisionPatientRecordServices(this IServiceCollection services)
        {
            services.AddTransient<VisionAllergyMapper>();
            services.AddTransient<VisionImmunisationsMapper>();
            services.AddTransient<VisionMedicationMapper>();
            services.AddTransient<VisionProblemsMapper>();
            services.AddTransient<VisionTestResultsMapper>();
            services.AddTransient<VisionDiagnosisMapper>();
            services.AddTransient<VisionExaminationsMapper>();
            services.AddTransient<VisionProceduresMapper>();
            services.AddTransient<PatientRecordSectionResolver>();

            services.AddTransient<AllergiesSection>();
            services.AddTransient<ImmunisationsSection>();
            services.AddTransient<MedicationsSection>();
            services.AddTransient<ProblemsSection>();
            services.AddTransient<TestResultSection>();
            services.AddTransient<DiagnosisSection>();
            services.AddTransient<ExaminationsSection>();
            services.AddTransient<ProceduresSection>();

            services.AddTransient<VisionPatientRecordService>();
            services.AddTransient<IVisionMyRecordMapper, VisionMyRecordMapper>();
            services.AddTransient<IVisionMyRecordSectionMapper, VisionMyRecordSectionMapper>();

            return services;
        }
    }
}