using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections
{
    public class ExaminationsSection : IRecordSection<Examinations>
    {
        public ExaminationsSection(VisionExaminationsMapper mapper) 
        {
            Mapper = mapper;
        }
        public string ViewName => RecordSectionViews.ExamFindings;
        public IVisionMapper<Examinations> Mapper { get; }
        public string FormatResponse => ResponseFormats.Html;
    }
}