using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections
{
    public class ProblemsSection : IRecordSection<Problems>
    {
        public ProblemsSection(VisionProblemsMapper mapper) 
        {
            Mapper = mapper;
        }

        public string ViewName => RecordSectionViews.Problems;
        public IVisionMapper<Problems> Mapper { get; }
        public string FormatResponse => ResponseFormats.Xml;
    }
}