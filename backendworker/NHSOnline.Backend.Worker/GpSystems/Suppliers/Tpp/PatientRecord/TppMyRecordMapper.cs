using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class TppMyRecordMapper : ITppMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Medications medications, TppDcrEvents dcrEvents, TestResults testResults)
        {
            return new MyRecordResponse
            {
                Allergies = allergies,
                Medications = medications,
                TppDcrEvents = dcrEvents,
                TestResults = testResults,
                HasSummaryRecordAccess = allergies?.HasAccess ?? false,
                HasDetailedRecordAccess = dcrEvents.HasAccess
            };
        }
    }
}