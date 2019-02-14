using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public interface IEmisMedicationMapper
    {
        Medications Map(MedicationRootObject medicationRootObject);          
    }
}