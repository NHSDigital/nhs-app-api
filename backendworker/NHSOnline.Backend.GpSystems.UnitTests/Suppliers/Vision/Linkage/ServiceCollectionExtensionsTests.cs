using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Linkage
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckVisionAppointmentServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionLinkageServices();
            
            CheckRegisteredVisionLinkageServices(services);
        }

        public static void CheckRegisteredVisionLinkageServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(VisionLinkageService), typeof(VisionLinkageService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IVisionLinkageMapper), typeof(VisionLinkageMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionLinkageValidationService), typeof(VisionLinkageValidationService), ServiceLifetime.Singleton),
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
        }
    }
}
