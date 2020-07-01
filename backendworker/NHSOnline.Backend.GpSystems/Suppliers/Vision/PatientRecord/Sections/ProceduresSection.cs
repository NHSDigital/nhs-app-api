using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections
{
    public class ProceduresSection : IRecordSection<Procedures>
    {
        public ProceduresSection(VisionProceduresMapper proceduresMapper) 
        {
            Mapper = proceduresMapper;
        }
        public string ViewName => RecordSectionViews.Procedures;
        public IVisionMapper<Procedures> Mapper { get; }
        public string FormatResponse => ResponseFormats.Html;
    }
}