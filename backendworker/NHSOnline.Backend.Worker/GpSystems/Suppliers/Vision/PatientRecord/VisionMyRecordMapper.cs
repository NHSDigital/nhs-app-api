using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionMyRecordMapper : IVisionMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, Problems problems, TestResults testResults, Diagnosis diagnosis)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                Medications = medications,
                Immunisations = immunisations,
                Problems = problems,
                TestResults = testResults,
                Diagnosis = diagnosis,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false,
                HasDetailedRecordAccess = (
                    (immunisations != null && immunisations.HasAccess) || 
                    (problems != null && problems.HasAccess) || 
                    (testResults != null && testResults.HasAccess) || 
                    (diagnosis != null && diagnosis.HasAccess))
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
