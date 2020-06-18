using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    internal sealed class VisionPatientRecordData
    {
        public VisionPatientRecordData(
            Allergies allergies,
            Medications medications,
            Immunisations immunisations,
            Problems problems,
            TestResults testResults,
            Diagnosis diagnosis,
            Examinations examinations,
            Procedures procedures)
        {
            Allergies = allergies;
            Medications = medications;
            Immunisations = immunisations;
            Problems = problems;
            TestResults = testResults;
            Diagnosis = diagnosis;
            Examinations = examinations;
            Procedures = procedures;
        }

        public bool HasSummaryRecordAccess => AllergiesHasAccess;

        public bool HasDetailedRecordAccess =>
            ImmunisationsHasAccess ||
            ProblemsHasAccess ||
            TestResultsHasAccess ||
            DiagnosisHasAccess ||
            ExaminationsHasAccess ||
            ProceduresHasAccess;

        public Allergies Allergies { get; }
        public bool AllergiesHasAccess => Allergies?.HasAccess ?? false;

        public Medications Medications { get; }

        public Immunisations Immunisations { get; }
        public bool ImmunisationsHasAccess => Immunisations?.HasAccess ?? false;

        public Problems Problems { get; }
        public bool ProblemsHasAccess => Problems?.HasAccess ?? false;

        public TestResults TestResults { get; }
        public bool TestResultsHasAccess => TestResults?.HasAccess ?? false;

        public Diagnosis Diagnosis { get; }
        public bool DiagnosisHasAccess => Diagnosis?.HasAccess ?? false;

        public Examinations Examinations { get; }
        public bool ExaminationsHasAccess => Examinations?.HasAccess ?? false;

        public Procedures Procedures { get; }
        public bool ProceduresHasAccess => Procedures?.HasAccess ?? false;

    }
}