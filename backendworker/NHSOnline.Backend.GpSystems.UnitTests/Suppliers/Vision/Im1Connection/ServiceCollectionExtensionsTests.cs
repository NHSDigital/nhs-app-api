using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Im1Connection
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckVisionIm1ServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionIm1ConnectionServices();
            
            CheckRegisteredVisionIm1ConnectionServices(services);
        }

        public static void CheckRegisteredVisionIm1ConnectionServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(VisionIm1ConnectionService), typeof(VisionIm1ConnectionService), ServiceLifetime.Transient),
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
        }
    }
}
