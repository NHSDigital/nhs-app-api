using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckTppPfsServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterTppPfsServices();
            CheckAllTppPfsRegisteredServices(services);
            CheckTppBaseServices(services);
        }
        
        [TestMethod]
        public void CheckTppCidServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterTppCidServices();
            CheckAllTppCidRegisteredServices(services);
            CheckTppBaseServices(services);
        }
        
        public static void CheckAllTppPfsRegisteredServices(ServiceCollection services)
        {
            Appointments.ServiceCollectionExtensionTests.CheckRegisteredTppAppointmentServices(services);          
            Demographics.ServiceCollectionExtensionTests.CheckRegisteredTppDemographicServices(services);
            PatientRecord.ServiceCollectionExtensionTests.CheckRegisteredTppPatientRecordServices(services);
            Prescriptions.ServiceCollectionExtensionTests.CheckRegisteredTppPrescriptionServices(services);           
        }
        
        public static void CheckAllTppCidRegisteredServices(ServiceCollection services)
        {
            Im1Connection.ServiceCollectionExtensionTests.CheckRegisteredTppIm1Services(services);
            Linkage.ServiceCollectionExtensionTests.CheckRegisteredTppLinkageServices(services);
        }

        private void CheckTppBaseServices(ServiceCollection services)
        {
            var registeredServices = services.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(TppHttpClientHandler), typeof(TppHttpClientHandler), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(TppHttpRequestIdentifier), typeof(TppHttpRequestIdentifier), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IGpSystem), typeof(TppGpSystem), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(ITppClient), typeof(TppClient), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(TppTokenValidationService), typeof(TppTokenValidationService),ServiceLifetime.Transient)
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
            
            registeredServices.Should().Contain(x => x.ServiceType == typeof(TppHttpClient) && x.Lifetime == ServiceLifetime.Transient);
        }
    }
}