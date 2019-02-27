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
        public void CheckTppServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterTppServices();
            CheckAllTppRegisteredServices(services);
            CheckTppBaseServices(services);
        }
        
        public static void CheckAllTppRegisteredServices(ServiceCollection services)
        {
            Appointments.ServiceCollectionExtensionTests.CheckRegisteredTppAppointmentServices(services);          
            Demographics.ServiceCollectionExtensionTests.CheckRegisteredTppDemographicServices(services);
            Im1Connection.ServiceCollectionExtensionTests.CheckRegisteredTppIm1Services(services);
            Linkage.ServiceCollectionExtensionTests.CheckRegisteredTppLinkageServices(services);
            PatientRecord.ServiceCollectionExtensionTests.CheckRegisteredTppPatientRecordServices(services);
            Prescriptions.ServiceCollectionExtensionTests.CheckRegisteredTppPrescriptionServices(services);           
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
                new ServiceDescriptor(typeof(ITppConfig), typeof(TppConfig),ServiceLifetime.Singleton),
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