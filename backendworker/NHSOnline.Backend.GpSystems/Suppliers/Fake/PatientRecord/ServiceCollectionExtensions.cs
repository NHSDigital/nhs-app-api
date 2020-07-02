using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakePatientRecordServices(this IServiceCollection services)
        {
            services.AddTransient<FakePatientRecordService>();

            return services;
        }
    }
}