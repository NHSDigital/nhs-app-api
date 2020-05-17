using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Clients;
using NHSOnline.Backend.NominatedPharmacy.Envelope;
using NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests
{
    [TestClass]
    public class NominatedPharmacyStartupTests
    {
        [TestMethod]
        public void CheckServicesAreRegistered()
        {
            var services = new ServiceCollection();

            // Act
            NominatedPharmacyStartup.RegisterServices(services);

            // Assert
            var registeredServices = services.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(INominatedPharmacyService), typeof(NominatedPharmacyService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(INominatedPharmacyClient), typeof(NominatedPharmacyClient), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(INominatedPharmacyPDSClient), typeof(NominatedPharmacyPDSClient), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(INominatedPharmacySubmitClient), typeof(NominatedPharmacySubmitClient), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(INominatedPharmacyGatewayUpdateService), typeof(NominatedPharmacyGatewayUpdateService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(INominatedPharmacyEnvelopeService), typeof(NominatedPharmacyEnvelopeService), ServiceLifetime.Transient)
            };
            
            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
            
            registeredServices.Should().Contain(x => x.ServiceType == typeof(NominatedPharmacyHttpClient) && x.Lifetime == ServiceLifetime.Transient);
        }
    }
}