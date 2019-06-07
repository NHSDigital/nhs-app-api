using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    public interface IMicrotestMyRecordMapper
    {
        MyRecordResponse Map(PatientRecordGetResponse patientRecordGetResponse);        

    }
}