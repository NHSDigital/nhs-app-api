using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public interface IEmisMyRecordMapper
    {
        MyRecordResponse Map(Allergies allergies, Medications medications, Immunisations immunisations, TestResults testResults, Problems problems, Consultations consultations, PatientDocuments documents);        
    }
}