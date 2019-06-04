using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.PatientRecords
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckMicrotestServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterMicrotestPatientRecordServices();
            CheckRegisteredPatientRecordService(services);                      
        }

        public static void CheckRegisteredPatientRecordService(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();

            var microtestPatientRecordService = new ServiceDescriptor(typeof(MicrotestPatientRecordService),
                typeof(MicrotestPatientRecordService), ServiceLifetime.Transient);
            
            registeredServices.Should().ContainEquivalentOf(microtestPatientRecordService);
        }
    }
}