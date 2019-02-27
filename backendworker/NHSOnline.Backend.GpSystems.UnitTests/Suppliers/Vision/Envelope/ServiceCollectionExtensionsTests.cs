using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Envelope
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckVisionEnvelopeServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionEnvelopeServices();
            
            CheckRegisteredVisionEnvelopeServices(services);
        }

        public static void CheckRegisteredVisionEnvelopeServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(IEnvelopeService), typeof(EnvelopeService), ServiceLifetime.Transient),
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
        }
    }
}
