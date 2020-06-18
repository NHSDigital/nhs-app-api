using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    internal sealed class VisionPatientRecordSectionsService
    {
        private readonly PatientRecordSectionResolver _patientRecordSectionResolver;
        private readonly AllergiesSection _allergiesSection;
        private readonly MedicationsSection _medicationsSection;
        private readonly ImmunisationsSection _immunisationsSection;
        private readonly ProblemsSection _problemsSection;
        private readonly TestResultSection _testResultsSection;
        private readonly DiagnosisSection _diagnosisSection;
        private readonly ExaminationsSection _examinationsSection;
        private readonly ProceduresSection _proceduresSection;

        public VisionPatientRecordSectionsService(
            PatientRecordSectionResolver patientRecordSectionResolver,
            AllergiesSection allergiesSection,
            MedicationsSection medicationSection,
            ImmunisationsSection immunisationsSection,
            ProblemsSection problemsSection,
            TestResultSection testResultsSection,
            DiagnosisSection diagnosisSection,
            ExaminationsSection examinationsSection,
            ProceduresSection proceduresSection)
        {
            _patientRecordSectionResolver = patientRecordSectionResolver;
            _allergiesSection = allergiesSection;
            _medicationsSection = medicationSection;
            _immunisationsSection = immunisationsSection;
            _problemsSection = problemsSection;
            _testResultsSection = testResultsSection;
            _diagnosisSection = diagnosisSection;
            _examinationsSection = examinationsSection;
            _proceduresSection = proceduresSection;
        }

        internal async Task<VisionPatientRecordData> GetPatientRecordData(VisionUserSession userSession)
        {
            var allergiesTask = _patientRecordSectionResolver.GetPatientData(userSession, _allergiesSection);
            var medicationsTask = _patientRecordSectionResolver.GetPatientData(userSession, _medicationsSection);
            var immunisationsTask = _patientRecordSectionResolver.GetPatientData(userSession, _immunisationsSection);
            var problemsTask = _patientRecordSectionResolver.GetPatientData(userSession, _problemsSection);
            var testResultsTask = _patientRecordSectionResolver.GetPatientData(userSession, _testResultsSection);
            var diagnosisTask = _patientRecordSectionResolver.GetPatientData(userSession, _diagnosisSection);
            var examinationsTask = _patientRecordSectionResolver.GetPatientData(userSession, _examinationsSection);
            var proceduresTask = _patientRecordSectionResolver.GetPatientData(userSession, _proceduresSection);

            await Task.WhenAll(
                allergiesTask,
                medicationsTask,
                immunisationsTask,
                problemsTask,
                diagnosisTask,
                testResultsTask,
                examinationsTask,
                proceduresTask);

            return new VisionPatientRecordData(
                await allergiesTask,
                await medicationsTask,
                await immunisationsTask,
                await problemsTask,
                await testResultsTask,
                await diagnosisTask,
                await examinationsTask,
                await proceduresTask);
        }
    }
}