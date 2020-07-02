using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
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
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public static class ServiceCollectionExtensions
    {
        private const string FakeGpSystemNamespace = "NHSOnline.Backend.GpSystems.Suppliers.Fake";

        public static IServiceCollection RegisterFakePfsServices(this IServiceCollection services)
        {
            services.RegisterFakeBaseServices();

            services.RegisterFakeAppointmentsServices();
            services.RegisterFakeDemographicsServices();
            services.RegisterFakeLinkedAccountsServices();
            services.RegisterFakePatientRecordServices();
            services.RegisterFakePrescriptionServices();
            services.RegisterFakePfsSessionServices();

            services.RegisterBehaviourMappings();

            return services;
        }

        public static IServiceCollection RegisterFakeCidServices(this IServiceCollection services)
        {
            services.RegisterFakeBaseServices();

            services.RegisterFakeIm1ConnectionServices();
            services.RegisterFakeLinkageServices();
            services.RegisterFakeCidSessionServices();

            services.RegisterBehaviourMappings();

            return services;
        }

        public static IServiceCollection RegisterFakeUserConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<FakeGpSupplierConfiguration>(configuration);

            return services;
        }

        private static IServiceCollection RegisterBehaviourMappings(this IServiceCollection services)
        {
            // get all types in this namespace
            var assemblyTypes = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(t => t.MatchesNamespacePrefix(FakeGpSystemNamespace))
                .ToList();

            var fakeGpSupplierAreas = assemblyTypes.Where(t => t.IsInterface)
                .Where(t => t.HasAttribute<FakeGpAreaAttribute>())
                .ToList();

            var areaBehaviours = assemblyTypes.Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Select(t =>
                    (type: t, behaviour: t.GetAttribute<FakeGpAreaBehaviourAttribute>())
                )
                .Where(t => t.behaviour != null)
                .ToList();

            var areaTypeToBehaviourTypes = fakeGpSupplierAreas.ToDictionary(
                a => a,
                a => BuildBehaviourToTypeMap(a, areaBehaviours)
            );

            areaTypeToBehaviourTypes.Values
                .SelectMany(bt => bt.Values)
                .ForEach(t => services.AddTransient(t));

            services.AddSingleton<IDictionary<Type, IDictionary<Behaviour, Type>>>(areaTypeToBehaviourTypes);

            return services;
        }

        private static IDictionary<Behaviour, Type> BuildBehaviourToTypeMap(
            Type areaType,
            List<(Type, FakeGpAreaBehaviourAttribute)> areaBehaviours
        )
        {
            return areaBehaviours.Where(b =>
                    areaType.IsAssignableFrom(b.Item1)
                )
                .GroupBy(b => b.Item2.Behaviour)
                .ToDictionary(
                    b => b.Key,
                    b => b.Select(t => t.Item1).First()
                );
        }

        private static IServiceCollection RegisterFakeBaseServices(this IServiceCollection services)
        {
            services.AddSingleton<IGpSystem, FakeGpSystem>();
            services.AddTransient<FakeTokenValidationService>();

            services.RegisterFakeUsers();

            return services;
        }
    }
}