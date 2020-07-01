using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections
{
    public class AllergiesSection : IRecordSection<Allergies>
    {
        public AllergiesSection(VisionAllergyMapper mapper) 
        {
            Mapper = mapper;
        }

        public string ViewName => RecordSectionViews.VpsAllergies;
        public IVisionMapper<Allergies> Mapper { get; }
        public string FormatResponse => ResponseFormats.Html;
    }
}