using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord
{
    public class VisionMyRecordSectionMapper : IVisionMyRecordSectionMapper
    {
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