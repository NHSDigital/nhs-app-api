using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionMyRecordMapper : IVisionMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, Problems problems, TestResults testResults, Diagnosis diagnosis, Examinations examinations, Procedures procedures)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                Medications = medications,
                Immunisations = immunisations,
                Problems = problems,
                TestResults = testResults,
                Diagnosis = diagnosis,
                Examinations = examinations,
                Procedures = procedures,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false,
                HasDetailedRecordAccess = (
                    (immunisations != null && immunisations.HasAccess) || 
                    (problems != null && problems.HasAccess) || 
                    (testResults != null && testResults.HasAccess) || 
                    (diagnosis != null && diagnosis.HasAccess) ||
                    (examinations != null && examinations.HasAccess) ||
                    (procedures != null && procedures.HasAccess))
            };
        }

        public MyRecordSectionResponse MapSection(IVisionPatientDataModel detailedSection, string sectionName)
        {
            return new MyRecordSectionResponse()
            {
                Markup = detailedSection.RawHtml,
                SectionName = sectionName
            };
        }
    }
}
