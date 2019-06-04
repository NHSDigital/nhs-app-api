using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMicrotestPatientRecordServices(this IServiceCollection services)
        {
            services.AddTransient<MicrotestPatientRecordService>();

            return services;
        }
    }
}