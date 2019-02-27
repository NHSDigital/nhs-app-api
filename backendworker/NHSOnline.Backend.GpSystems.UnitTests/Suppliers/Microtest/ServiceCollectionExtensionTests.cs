using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckMicrotestServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterMicrotestServices();
            CheckMicrotestBaseServices(services);
            CheckAllEmisRegisteredServices(services);
        }
        
        public static void CheckAllEmisRegisteredServices(ServiceCollection services)
        {
            Appointments.ServiceCollectionExtensionTests.CheckRegisteredMicrotestAppointmentServices(services);           
            Im1Connection.ServiceCollectionExtensionTests.CheckRegisteredMicrotestIm1ConnectionServices(services);
            Linkage.ServiceCollectionExtensionTests.CheckRegisteredMicrotestLinkageServices(services);
        } 
        
        private void CheckMicrotestBaseServices(ServiceCollection services)
        {
            var registeredServices = services.ToList();

            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(MicrotestHttpClientHandler), typeof(MicrotestHttpClientHandler),ServiceLifetime.Singleton),                
                new ServiceDescriptor(typeof(IGpSystem), typeof(MicrotestGpSystem),ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IMicrotestClient), typeof(MicrotestClient),ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(IMicrotestConfig), typeof(MicrotestConfig), ServiceLifetime.Singleton),
                new ServiceDescriptor(typeof(MicrotestTokenValidationService), typeof(MicrotestTokenValidationService),ServiceLifetime.Transient)
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
            
            registeredServices.Should().Contain(x => x.ServiceType == typeof(MicrotestHttpClient) && x.Lifetime == ServiceLifetime.Transient);
        }

    }
}