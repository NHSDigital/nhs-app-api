using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections
{
    public class ImmunisationsSection : IRecordSection<Immunisations>
    {
        public ImmunisationsSection(VisionImmunisationsMapper mapper) 
        {
            Mapper = mapper;
        }

        public string ViewName => RecordSectionViews.Procedures;
        public IVisionMapper<Immunisations> Mapper { get; }
        public string FormatResponse => ResponseFormats.Xml;
    }
}