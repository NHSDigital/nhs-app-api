using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public interface IEmisMyRecordMapper
    {
        MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, TestResults testResults, Problems problems);        
    }
}