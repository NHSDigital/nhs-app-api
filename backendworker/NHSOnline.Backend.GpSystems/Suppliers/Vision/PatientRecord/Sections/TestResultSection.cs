using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections
{
    public class TestResultSection : IRecordSection<TestResults>
    {
        public TestResultSection(VisionTestResultsMapper mapper) 
        {
            Mapper = mapper;
        }
        public string ViewName => RecordSectionViews.TestResults;
        public IVisionMapper<TestResults> Mapper { get; }
        public string FormatResponse => ResponseFormats.Html;
    }
}