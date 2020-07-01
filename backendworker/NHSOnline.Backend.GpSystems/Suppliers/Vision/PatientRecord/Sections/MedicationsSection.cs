using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections
{
    public class MedicationsSection : IRecordSection<Medications>
    {
        public MedicationsSection(VisionMedicationMapper mapper) 
        {
            Mapper = mapper;
        }

        public string ViewName => RecordSectionViews.VpsMedications;
        public IVisionMapper<Medications> Mapper { get; }
        public string FormatResponse => ResponseFormats.Xml;
    }
}