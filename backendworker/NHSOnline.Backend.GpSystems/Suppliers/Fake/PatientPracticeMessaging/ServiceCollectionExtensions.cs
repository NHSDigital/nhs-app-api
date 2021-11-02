using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientPracticeMessaging
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakePatientPracticeMessagingServices(this IServiceCollection services)
        {
            services.AddTransient<FakePatientMessagesService>();

            return services;
        }
    }
}