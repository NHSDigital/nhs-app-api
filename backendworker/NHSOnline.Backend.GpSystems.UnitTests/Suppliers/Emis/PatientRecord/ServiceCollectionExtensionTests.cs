using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void CheckEmisServiceCollectionExtensions()
        {
            var services = new ServiceCollection();

            services.RegisterEmisPatientRecordServices();
            CheckRegisteredPatientRecordService(services);                      
        }

        public static void CheckRegisteredPatientRecordService(ServiceCollection serviceCollection)
        {
            serviceCollection.Should().NotBeNull();

            var registeredServices = serviceCollection.ToList();
            var emisAllergyMapper = new ServiceDescriptor(typeof(EmisAllergyMapper),
                typeof(EmisAllergyMapper), ServiceLifetime.Transient);
            var emisMedicationMapper = new ServiceDescriptor(typeof(IEmisMedicationMapper),
                typeof(EmisMedicationMapper), ServiceLifetime.Transient);
            var emisImmunisationMapper = new ServiceDescriptor(typeof(EmisImmunisationMapper),
                typeof(EmisImmunisationMapper), ServiceLifetime.Transient);
            var emisProblemMapper = new ServiceDescriptor(typeof(EmisProblemMapper),
                typeof(EmisProblemMapper), ServiceLifetime.Transient);
            var emisTestResultMapper = new ServiceDescriptor(typeof(EmisTestResultMapper),
                typeof(EmisTestResultMapper), ServiceLifetime.Transient);
            var emisConsultationMapper = new ServiceDescriptor(typeof(EmisConsultationMapper),
                typeof(EmisConsultationMapper), ServiceLifetime.Transient);
            var emisGetAllergiesTaskChecker = new ServiceDescriptor(typeof(GetAllergiesTaskChecker),
                typeof(GetAllergiesTaskChecker), ServiceLifetime.Transient);
            var emisGetMedicationTaskChecker = new ServiceDescriptor(typeof(GetMedicationsTaskChecker),
                typeof(GetMedicationsTaskChecker), ServiceLifetime.Transient);
            var emisGetImmunisationTaskChecker = new ServiceDescriptor(typeof(GetImmunisationsTaskChecker),
                typeof(GetImmunisationsTaskChecker), ServiceLifetime.Transient);
            var emisGetProblemTaskChecker = new ServiceDescriptor(typeof(GetProblemsTaskChecker),
                typeof(GetProblemsTaskChecker), ServiceLifetime.Transient);
            var emisGetTestResultsTaskChecker = new ServiceDescriptor(typeof(GetTestResultsTaskChecker),
                typeof(GetTestResultsTaskChecker), ServiceLifetime.Transient);
            var emisGetConsultationsTaskChecker = new ServiceDescriptor(typeof(GetConsultationsTaskChecker),
                typeof(GetConsultationsTaskChecker), ServiceLifetime.Transient);
            var emisPatientRecordService = new ServiceDescriptor(typeof(EmisPatientRecordService),
                typeof(EmisPatientRecordService), ServiceLifetime.Transient);
            var emisMyRecordMapper = new ServiceDescriptor(typeof(IEmisMyRecordMapper),
                typeof(EmisMyRecordMapper), ServiceLifetime.Transient);
            
            registeredServices.Should().ContainEquivalentOf(emisAllergyMapper);
            registeredServices.Should().ContainEquivalentOf(emisMedicationMapper);
            registeredServices.Should().ContainEquivalentOf(emisImmunisationMapper);
            registeredServices.Should().ContainEquivalentOf(emisProblemMapper);
            registeredServices.Should().ContainEquivalentOf(emisTestResultMapper);
            registeredServices.Should().ContainEquivalentOf(emisConsultationMapper);
            registeredServices.Should().ContainEquivalentOf(emisGetAllergiesTaskChecker);
            registeredServices.Should().ContainEquivalentOf(emisGetMedicationTaskChecker);
            registeredServices.Should().ContainEquivalentOf(emisGetImmunisationTaskChecker);
            registeredServices.Should().ContainEquivalentOf(emisGetProblemTaskChecker);
            registeredServices.Should().ContainEquivalentOf(emisGetTestResultsTaskChecker);
            registeredServices.Should().ContainEquivalentOf(emisGetConsultationsTaskChecker);
            registeredServices.Should().ContainEquivalentOf(emisPatientRecordService);
            registeredServices.Should().ContainEquivalentOf(emisMyRecordMapper);
        }
    }
}