using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface ITppMyRecordMapper
    {
        MyRecordResponse Map(Allergies allergies,
            Medications medications,
            TppDcrEvents dcrEvents,
            TestResults testResults,
            PatientDocuments patientDocumentItems);
    }
}