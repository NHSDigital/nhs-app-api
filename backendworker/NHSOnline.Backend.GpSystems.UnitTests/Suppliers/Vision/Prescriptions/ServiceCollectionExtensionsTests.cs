using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Prescriptions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Prescriptions
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckVisionPrescriptionServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionPrescriptionsServices();
            
            CheckRegisteredVisionPrescriptionsServices(services);
        }

        public static void CheckRegisteredVisionPrescriptionsServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(VisionPrescriptionService), typeof(VisionPrescriptionService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionPrescriptionRequestValidationService), typeof(VisionPrescriptionRequestValidationService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IVisionPrescriptionMapper), typeof(VisionPrescriptionMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionCourseService), typeof(VisionCourseService), ServiceLifetime.Transient),
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
        }
    }
}
