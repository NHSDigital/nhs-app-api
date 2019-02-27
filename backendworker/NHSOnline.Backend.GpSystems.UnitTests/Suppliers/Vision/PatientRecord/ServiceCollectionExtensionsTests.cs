using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void CheckVisionPatientRecordServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterVisionPatientRecordServices();
            
            CheckRegisteredVisionPatientRecordServices(services);
        }

        public static void CheckRegisteredVisionPatientRecordServices(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();
            
            var dependencies = new List<ServiceDescriptor>
            {
                new ServiceDescriptor(typeof(VisionAllergyMapper), typeof(VisionAllergyMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionImmunisationsMapper), typeof(VisionImmunisationsMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionMedicationMapper), typeof(VisionMedicationMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionProblemsMapper), typeof(VisionProblemsMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionTestResultsMapper), typeof(VisionTestResultsMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionDiagnosisMapper), typeof(VisionDiagnosisMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionExaminationsMapper), typeof(VisionExaminationsMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionProceduresMapper), typeof(VisionProceduresMapper), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(VisionPatientRecordService), typeof(VisionPatientRecordService), ServiceLifetime.Transient),
                new ServiceDescriptor(typeof(IVisionMyRecordMapper), typeof(VisionMyRecordMapper), ServiceLifetime.Transient),
            };

            foreach (var dependency in dependencies)
            {
                registeredServices.Should().ContainEquivalentOf(dependency);
            }
        }
    }
}
