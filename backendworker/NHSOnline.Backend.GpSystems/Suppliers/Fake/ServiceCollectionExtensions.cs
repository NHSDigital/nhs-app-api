using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakePfsServices(this IServiceCollection services)
        {
            services.RegisterFakeBaseServices();

            services.RegisterFakeAppointmentsServices();
            services.RegisterFakeDemographicsServices();
            services.RegisterFakeLinkedAccountsServices();
            services.RegisterFakePatientRecordServices();
            services.RegisterFakePrescriptionServices();
            services.RegisterFakePfsSessionServices();

            return services;
        }

        public static IServiceCollection RegisterFakeCidServices(this IServiceCollection services)
        {
            services.RegisterFakeBaseServices();

            services.RegisterFakeIm1ConnectionServices();
            services.RegisterFakeLinkageServices();
            services.RegisterFakeCidSessionServices();

            return services;
        }

        private static IServiceCollection RegisterFakeBaseServices(this IServiceCollection services)
        {
            services.AddSingleton<IGpSystem, FakeGpSystem>();
            services.AddTransient<FakeTokenValidationService>();
            services.AddTransient<IFakeUserRepository, FakeUserRepository>();

            services.RegisterFakeUsers();

            return services;
        }
    }
}