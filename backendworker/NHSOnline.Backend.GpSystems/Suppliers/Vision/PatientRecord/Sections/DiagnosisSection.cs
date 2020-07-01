using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections
{
    public class DiagnosisSection : IRecordSection<Diagnosis>
    {
        public DiagnosisSection(VisionDiagnosisMapper mapper) 
        {
            Mapper = mapper;
        }
        public string ViewName => RecordSectionViews.Diagnosis;
        public IVisionMapper<Diagnosis> Mapper { get; }
        public string FormatResponse => ResponseFormats.Html;
    }
}