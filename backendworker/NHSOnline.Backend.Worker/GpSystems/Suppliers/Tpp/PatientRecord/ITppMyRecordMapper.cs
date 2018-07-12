using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface ITppMyRecordMapper
    {       
        MyRecordResponse Map(Allergies allergies, Medications medications, TppDcrEvents dcrEvents, TestResults testResults);  
    }
}