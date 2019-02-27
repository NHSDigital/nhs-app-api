using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckVisionServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionServices();

            CheckVisionBaseServices(services);
            CheckAllVisionRegisteredServices(services);           
        }

        private void CheckVisionBaseServices(ServiceCollection serviceCollection)
        {
            var registeredServices = serviceCollection.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(IGpSystem), typeof(VisionGpSystem), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IVisionPFSConfig), typeof(VisionPFSConfig), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IVisionLinkageConfig), typeof(VisionLinkageConfig), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IVisionPFSClient), typeof(VisionPFSClient), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IVisionLinkageClient), typeof(VisionLinkageClient), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IVisionClient), typeof(VisionClient), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(VisionTokenValidationService), typeof(VisionTokenValidationService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionPFSHttpRequestIdentifier), typeof(VisionPFSHttpRequestIdentifier), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionLinkageHttpRequestIdentifier), typeof(VisionLinkageHttpRequestIdentifier), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionHttpClientHandler), typeof(VisionHttpClientHandler), ServiceLifetime.Singleton),
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
            
            // check http clients
            registeredServices.Should().Contain(x => x.ServiceType == typeof(VisionPFSHttpClient) && x.Lifetime == ServiceLifetime.Transient);
            registeredServices.Should().Contain(x => x.ServiceType == typeof(VisionLinkageHttpClient) && x.Lifetime == ServiceLifetime.Transient);
        }

        public static void CheckAllVisionRegisteredServices(ServiceCollection services)
        {
            Suppliers.Vision.Appointments.ServiceCollectionExtensionsTests.CheckRegisteredVisionAppointmentServices(services);
            Suppliers.Vision.Demographics.ServiceCollectionExtensionsTests.CheckRegisteredVisionDemographicsServices(services);
            Suppliers.Vision.Envelope.ServiceCollectionExtensionsTests.CheckRegisteredVisionEnvelopeServices(services);
            Suppliers.Vision.Im1Connection.ServiceCollectionExtensionsTests.CheckRegisteredVisionIm1ConnectionServices(services);
            Suppliers.Vision.Linkage.ServiceCollectionExtensionsTests.CheckRegisteredVisionLinkageServices(services);
            Suppliers.Vision.PatientRecord.ServiceCollectionExtensionsTests.CheckRegisteredVisionPatientRecordServices(services);
            Suppliers.Vision.Prescriptions.ServiceCollectionExtensionsTests.CheckRegisteredVisionPrescriptionsServices(services);
            Suppliers.Vision.Session.ServiceCollectionExtensionsTests.CheckRegisteredVisionSessionServices(services);
        }
    }
}