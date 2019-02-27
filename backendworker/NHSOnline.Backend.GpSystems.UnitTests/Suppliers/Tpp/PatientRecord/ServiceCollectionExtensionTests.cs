using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckTppServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterTppPatientRecordServices();
            CheckRegisteredTppPatientRecordServices(services);
        }

        public static void CheckRegisteredTppPatientRecordServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredService = serviceCollection.ToList();

            var tppPatientRecordService = new ServiceDescriptor(typeof(TppPatientRecordService),
                typeof(TppPatientRecordService), ServiceLifetime.Transient);
            var tppMyRecordMapper = new ServiceDescriptor(typeof(ITppMyRecordMapper),
                typeof(TppMyRecordMapper), ServiceLifetime.Transient);
            var tppGetPatientDcrEventsTaskChecker = new ServiceDescriptor(typeof(IGetPatientDcrEventsTaskChecker),
                typeof(GetPatientDcrEventsTaskChecker), ServiceLifetime.Transient);
            var tppGetPatientOverviewTaskChecker = new ServiceDescriptor(typeof(IGetPatientOverviewTaskChecker),
                typeof(GetPatientOverviewTaskChecker), ServiceLifetime.Transient);
            var tppGetPatientTestResultsTaskChecker = new ServiceDescriptor(typeof(IGetPatientTestResultsTaskChecker),
                typeof(GetPatientTestResultsTaskChecker), ServiceLifetime.Transient);
            var tppGetTppDetailedTestResultChecker = new ServiceDescriptor(typeof(IGetTppDetailedTestResultChecker),
                typeof(GetTppDetailedTestResultChecker), ServiceLifetime.Transient);
            var tppDcrEventsMapper = new ServiceDescriptor(typeof(ITppDcrEventsMapper),
                typeof(TppDcrEventsMapper), ServiceLifetime.Transient);
            var tppDcrEventItemsMapper = new ServiceDescriptor(typeof(ITppDcrEventItemsMapper),
                typeof(TppDcrEventItemsMapper), ServiceLifetime.Transient);
            var tppDetailedTestResultMapper = new ServiceDescriptor(typeof(ITppDetailedTestResultMapper),
                typeof(TppDetailedTestResultMapper), ServiceLifetime.Transient);
            var tppTestResultsMapper = new ServiceDescriptor(typeof(ITppTestResultsMapper),
                typeof(TppTestResultsMapper), ServiceLifetime.Transient);
            var tppPatientOverviewMapper = new ServiceDescriptor(typeof(TppPatientOverviewMapper),
                typeof(TppPatientOverviewMapper), ServiceLifetime.Transient);


            registeredService.Should().ContainEquivalentOf(tppPatientRecordService);
            registeredService.Should().ContainEquivalentOf(tppMyRecordMapper);
            registeredService.Should().ContainEquivalentOf(tppGetPatientDcrEventsTaskChecker);
            registeredService.Should().ContainEquivalentOf(tppGetPatientOverviewTaskChecker);
            registeredService.Should().ContainEquivalentOf(tppGetPatientTestResultsTaskChecker);
            registeredService.Should().ContainEquivalentOf(tppGetTppDetailedTestResultChecker);
            registeredService.Should().ContainEquivalentOf(tppDcrEventsMapper);
            registeredService.Should().ContainEquivalentOf(tppDcrEventItemsMapper);
            registeredService.Should().ContainEquivalentOf(tppDetailedTestResultMapper);
            registeredService.Should().ContainEquivalentOf(tppTestResultsMapper);
            registeredService.Should().ContainEquivalentOf(tppPatientOverviewMapper);
        }
    }
}