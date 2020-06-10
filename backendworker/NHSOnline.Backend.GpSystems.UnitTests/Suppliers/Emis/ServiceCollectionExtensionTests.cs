using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

using NHSOnline.Backend.GpSystems.Suppliers.Emis;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckEmisPfsServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterEmisPfsServices();
            CheckEmisBaseServices(services);
            CheckAllEmisPfsRegisteredServices(services);
        }
        
        [TestMethod]
        public void CheckEmisCidServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterEmisCidServices();
            CheckEmisBaseServices(services);
            CheckAllEmisCidRegisteredServices(services);
        }

        private static void CheckEmisBaseServices(ServiceCollection services)
        {
            var registeredServices = services.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(EmisHttpClientHandler), typeof(EmisHttpClientHandler), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(EmisHttpRequestIdentifier), typeof(EmisHttpRequestIdentifier), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IGpSystem), typeof(EmisGpSystem), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IEmisClient), typeof(EmisClient), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IEmisEnumMapper), typeof(EmisEnumMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(EmisTokenValidationService), typeof(EmisTokenValidationService), ServiceLifetime.Transient)
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
            
            registeredServices.Should().Contain(x => x.ServiceType == typeof(EmisHttpClient) && x.Lifetime == ServiceLifetime.Transient);
        }

        public static void CheckAllEmisPfsRegisteredServices(ServiceCollection services)
        {
            Prescriptions.ServiceCollectionExtensionsTests.CheckRegisteredEmisPrescriptionServices(services);
            Appointments.ServiceCollectionExtensionTests.CheckRegisteredEmisAppointmentServices(services);
            Demographics.ServiceCollectionExtensionTests.CheckRegisteredEmisDemographicsService(services);
            PatientRecord.ServiceCollectionExtensionTests.CheckRegisteredPatientRecordService(services);
            Session.ServiceCollectionExtensionTests.CheckRegisteredEmisPfsSessionService(services);
        }
        
        public static void CheckAllEmisCidRegisteredServices(ServiceCollection services)
        {
            Linkage.ServiceCollectionExtensionTests.CheckRegisteredEmisLinkageService(services);
            Im1Connection.ServiceCollectionExtensionTests.CheckRegisteredEmisIm1ConnectionService(services);
            Session.ServiceCollectionExtensionTests.CheckRegisteredEmisCidSessionService(services);
        }
    }
}