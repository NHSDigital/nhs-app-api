using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Session
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckVisionSessionServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionSessionServices();
            
            CheckRegisteredVisionSessionServices(services);
        }

        public static void CheckRegisteredVisionSessionServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(VisionSessionService), typeof(VisionSessionService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionSessionExtendService), typeof(VisionSessionExtendService), ServiceLifetime.Transient),
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
        }
    }
}
