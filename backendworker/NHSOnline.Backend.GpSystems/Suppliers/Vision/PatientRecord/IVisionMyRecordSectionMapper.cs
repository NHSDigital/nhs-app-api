using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    public interface IVisionMyRecordSectionMapper
    {
        MyRecordSectionResponse MapSection(IVisionPatientDataModel detailedSection, string sectionName);
    }
}