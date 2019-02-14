using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    public interface IVisionMyRecordMapper
    {
        MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, Problems problems, TestResults testResults, Diagnosis diagnosis, Examinations examinations, Procedures procedures);
        MyRecordSectionResponse MapSection(IVisionPatientDataModel detailedSection, string sectionName);
    }
}
