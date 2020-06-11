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
        public void CheckVisionPfsServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionPfsServices();

            CheckVisionBaseServices(services);
            CheckAllVisionPfsRegisteredServices(services);           
        }
        
        [TestMethod]
        public void CheckVisionCidServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionCidServices();

            CheckVisionBaseServices(services);
            CheckAllVisionCidRegisteredServices(services);           
        }

        private void CheckVisionBaseServices(ServiceCollection serviceCollection)
        {
            var registeredServices = serviceCollection.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(IGpSystem), typeof(VisionGpSystem), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IVisionLinkageConfig), typeof(VisionLinkageConfig), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IVisionPfsClient), typeof(VisionPfsClient), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IVisionLinkageClient), typeof(VisionLinkageClient), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IVisionClient), typeof(VisionClient), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionTokenValidationService), typeof(VisionTokenValidationService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionPFSHttpRequestIdentifier), typeof(VisionPFSHttpRequestIdentifier), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionLinkageHttpRequestIdentifier), typeof(VisionLinkageHttpRequestIdentifier), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionHttpClientHandler), typeof(VisionHttpClientHandler), ServiceLifetime.Transient),
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
            
            // check http clients
            registeredServices.Should().Contain(x => x.ServiceType == typeof(VisionPFSHttpClient) && x.Lifetime == ServiceLifetime.Transient);
            registeredServices.Should().Contain(x => x.ServiceType == typeof(VisionLinkageHttpClient) && x.Lifetime == ServiceLifetime.Transient);
        }

        public static void CheckAllVisionPfsRegisteredServices(ServiceCollection services)
        {
            Appointments.ServiceCollectionExtensionsTests.CheckRegisteredVisionAppointmentServices(services);
            Demographics.ServiceCollectionExtensionsTests.CheckRegisteredVisionDemographicsServices(services);
            Envelope.ServiceCollectionExtensionsTests.CheckRegisteredVisionEnvelopeServices(services);
            PatientRecord.ServiceCollectionExtensionsTests.CheckRegisteredVisionPatientRecordServices(services);
            Prescriptions.ServiceCollectionExtensionsTests.CheckRegisteredVisionPrescriptionsServices(services);
            Session.ServiceCollectionExtensionsTests.CheckRegisteredVisionSessionServices(services);
        }
        
        public static void CheckAllVisionCidRegisteredServices(ServiceCollection services)
        {
            Envelope.ServiceCollectionExtensionsTests.CheckRegisteredVisionEnvelopeServices(services);
            Im1Connection.ServiceCollectionExtensionsTests.CheckRegisteredVisionIm1ConnectionServices(services);
            Linkage.ServiceCollectionExtensionsTests.CheckRegisteredVisionLinkageServices(services);
        }
    }
}